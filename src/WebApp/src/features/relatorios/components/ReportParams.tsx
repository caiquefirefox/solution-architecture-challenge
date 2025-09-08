import Button from '../../../components/ui/Button'
import Card, { CardHeader } from '../../../components/ui/Card'
import Input from '../../../components/ui/Input'
import Label from '../../../components/ui/Label'

export interface ReportParamsProps {
  de: string
  ate: string
  saldoInicial: string
  onChange: (next: { de?: string; ate?: string; saldoInicial?: string }) => void
  onGenerate: () => void
  loading?: boolean
}

export default function ReportParams({ de, ate, saldoInicial, onChange, onGenerate, loading }: ReportParamsProps) {
  return (
    <Card>
      <CardHeader title="Parâmetros" />
      <div className="grid md:grid-cols-5 gap-3 items-end">
        <div>
          <Label>De</Label>
          <Input type="date" value={de} onChange={(e) => onChange({ de: e.target.value })} />
        </div>
        <div>
          <Label>Até</Label>
          <Input type="date" value={ate} onChange={(e) => onChange({ ate: e.target.value })} />
        </div>
        <div>
          <Label>Saldo inicial</Label>
          <Input value={saldoInicial} onChange={(e) => onChange({ saldoInicial: e.target.value })} />
        </div>
        <div className="md:col-span-2">
          <Button onClick={onGenerate} disabled={loading}>
            {loading ? 'Gerando...' : 'Gerar relatório'}
          </Button>
        </div>
      </div>
    </Card>
  )
}
