import { useState } from 'react'
import { useAuth } from '../auth/AuthContext'
import ReportParams from './components/ReportParams'
import BalanceChart from './components/BalanceChart'
import DailyTable, { Daily } from './components/DailyTable'

export default function ReportPage() {
  const { api } = useAuth()
  const today = new Date().toISOString().slice(0,10)
  const [de, setDe] = useState(today)
  const [ate, setAte] = useState(today)
  const [saldoInicial, setSaldoInicial] = useState('0')
  const [rows, setRows] = useState<Daily[]>([])
  const [loading, setLoading] = useState(false)

  async function gerar() {
    setLoading(true)
    const res = await api.fetch(`/api/v1/relatorios/saldo-diario?de=${de}&ate=${ate}&saldoInicial=${Number(saldoInicial)}`)
    setLoading(false)
    if (!res.ok) { alert('Falha ao gerar'); return }
    setRows(await res.json())
  }

  const labels = rows.map(r => r.data)
  const data = rows.map(r => r.saldoAcumulado)

  return (
    <div className="space-y-6">
      <ReportParams
        de={de}
        ate={ate}
        saldoInicial={saldoInicial}
        onChange={(n) => { if (n.de !== undefined) setDe(n.de); if (n.ate !== undefined) setAte(n.ate); if (n.saldoInicial !== undefined) setSaldoInicial(n.saldoInicial) }}
        onGenerate={gerar}
        loading={loading}
      />
      <BalanceChart labels={labels} data={data} />
      <DailyTable rows={rows} />
    </div>
  )
}
