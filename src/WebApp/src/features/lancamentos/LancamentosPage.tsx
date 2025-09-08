import { useEffect, useMemo, useState } from 'react'
import { useAuth } from '../auth/AuthContext'
import DateRangeFilter from './components/DateRangeFilter'
import LancamentoForm, { LancamentoFormValues } from './components/LancamentoForm'
import LancamentosTable, { Lancamento } from './components/LancamentosTable'

function todayISO() { return new Date().toISOString().slice(0,10) }

export default function LancamentosPage() {
  const { api } = useAuth()
  const today = useMemo(() => todayISO(), [])
  const [de, setDe] = useState(today)
  const [ate, setAte] = useState(today)
  const [items, setItems] = useState<Lancamento[]>([])
  const [loading, setLoading] = useState(false)

  async function carregar() {
    setLoading(true)
    const res = await api.fetch(`/api/v1/lancamentos?de=${de}&ate=${ate}`)
    setLoading(false)
    if (!res.ok) { alert('Falha ao carregar'); return }
    const data = await res.json()
    setItems(data)
  }

  useEffect(() => { carregar() }, [])

  async function onCreate(v: LancamentoFormValues) {
    const res = await api.fetch('/api/v1/lancamentos', { method: 'POST', body: JSON.stringify(v) })
    if (!res.ok) { alert('Falha ao adicionar'); return false }
    await carregar()
    return true
  }

  async function onDelete(id: string) {
    const res = await api.fetch(`/api/v1/lancamentos/${id}`, { method: 'DELETE' })
    if (!res.ok) { alert('Falha ao excluir'); return }
    await carregar()
  }

  return (
    <div className="space-y-6">
      <DateRangeFilter
        de={de}
        ate={ate}
        loading={loading}
        onChange={(n) => { if (n.de) setDe(n.de); if (n.ate) setAte(n.ate) }}
        onApply={carregar}
      />
      <LancamentoForm initialDate={today} onSubmit={onCreate} />
      <LancamentosTable items={items} onDelete={onDelete} />
    </div>
  )
}
