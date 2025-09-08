import { Link, useLocation } from 'react-router-dom'
import Button from '../ui/Button'
import { clsx } from 'clsx'

export interface NavbarProps {
  isAuthenticated: boolean
  userName: string | null
  onLogout: (allDevices?: boolean) => Promise<void>
}

export default function Navbar({ isAuthenticated, userName, onLogout }: NavbarProps) {
  const loc = useLocation()

  return (
    <header className="border-b bg-white">
      <div className="container h-16 flex items-center justify-between">
        <div className="flex items-center gap-6">
          <Link to="/" className="text-lg font-semibold text-brand-700">Fluxo de Caixa</Link>
          {isAuthenticated && (
            <nav className="flex items-center gap-4">
              <Link
                to="/lancamentos"
                className={clsx('text-sm', loc.pathname.startsWith('/lancamentos') ? 'text-brand-700 font-semibold' : 'text-slate-600 hover:text-slate-900')}
              >
                Lançamentos
              </Link>
              <Link
                to="/relatorio"
                className={clsx('text-sm', loc.pathname.startsWith('/relatorio') ? 'text-brand-700 font-semibold' : 'text-slate-600 hover:text-slate-900')}
              >
                Relatório
              </Link>
            </nav>
          )}
        </div>

        <div className="flex items-center gap-2">
          {!isAuthenticated ? (
            <div className="flex items-center gap-2">
              <Link to="/login"><Button variant="secondary">Entrar</Button></Link>
              <Link to="/registrar"><Button>Registrar</Button></Link>
            </div>
          ) : (
            <div className="flex items-center gap-2">
              <span className="text-sm text-slate-600">Olá, <strong>{userName}</strong></span>
              <Button variant="secondary" onClick={() => onLogout(false)}>Logout</Button>
              <Button variant="secondary" onClick={() => onLogout(true)}>Logout todos</Button>
            </div>
          )}
        </div>
      </div>
    </header>
  )
}
