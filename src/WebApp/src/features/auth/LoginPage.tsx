import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from './AuthContext'

export default function LoginPage() {
  const { login } = useAuth()
  const [userOrEmail, setUserOrEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const nav = useNavigate()

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    setLoading(true)
    const ok = await login(userOrEmail, password)
    setLoading(false)
    if (ok) nav('/lancamentos')
    else alert('Login inválido')
  }

  return (
    <div className="max-w-md mx-auto">
      <div className="card p-6">
        <h1 className="text-xl font-semibold mb-4">Entrar</h1>

        <form onSubmit={submit} className="space-y-3">
          <div>
            <label className="label">Usuário ou Email</label>
            <input
              className="input"
              value={userOrEmail}
              onChange={(e) => setUserOrEmail(e.target.value)}
              required
            />
          </div>

          <div>
            <label className="label">Senha</label>
            <input
              className="input"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          <button className="btn btn-primary w-full" disabled={loading}>
            {loading ? 'Entrando...' : 'Entrar'}
          </button>
        </form>

        <div className="text-sm text-slate-600 mt-4">
          Não tem conta?{' '}
          <Link to="/registrar" className="text-brand-700 hover:underline">
            Registrar
          </Link>
        </div>
      </div>
    </div>
  )
}
