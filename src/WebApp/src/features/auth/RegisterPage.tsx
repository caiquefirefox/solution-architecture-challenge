import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from './AuthContext'
import { z } from 'zod'

const schema = z.object({
  userName: z.string().min(3).max(50),
  email: z.string().email(),
  password: z.string().min(6),
})

export default function RegisterPage() {
  const { register } = useAuth()
  const nav = useNavigate()
  const [form, setForm] = useState({ userName: '', email: '', password: '' })
  const [loading, setLoading] = useState(false)

  function set<K extends keyof typeof form>(k: K, v: string) {
    setForm({ ...form, [k]: v })
  }

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    const parsed = schema.safeParse(form)
    if (!parsed.success) {
      alert('Verifique os campos')
      return
    }
    setLoading(true)
    const ok = await register(form.userName, form.email, form.password)
    setLoading(false)
    if (ok) {
      alert('Usuário criado, faça login.')
      nav('/login')
    } else {
      alert('Falha ao registrar')
    }
  }

  return (
    <div className="max-w-md mx-auto">
      <div className="card p-6">
        <h1 className="text-xl font-semibold mb-4">Registrar</h1>

        <form onSubmit={submit} className="space-y-3">
          <div>
            <label className="label">Usuário</label>
            <input
              className="input"
              value={form.userName}
              onChange={(e) => set('userName', e.target.value)}
              required
            />
          </div>

          <div>
            <label className="label">Email</label>
            <input
              className="input"
              value={form.email}
              onChange={(e) => set('email', e.target.value)}
              required
            />
          </div>

          <div>
            <label className="label">Senha</label>
            <input
              className="input"
              type="password"
              value={form.password}
              onChange={(e) => set('password', e.target.value)}
              required
            />
          </div>

          <button className="btn btn-primary w-full" disabled={loading}>
            {loading ? 'Salvando...' : 'Registrar'}
          </button>
        </form>

        <div className="text-sm text-slate-600 mt-4">
          Já tem conta?{' '}
          <Link to="/login" className="text-brand-700 hover:underline">
            Entrar
          </Link>
        </div>
      </div>
    </div>
  )
}
