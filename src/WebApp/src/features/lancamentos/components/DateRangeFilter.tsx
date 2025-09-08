import Button from '../../../components/ui/Button'
import Card, { CardHeader } from '../../../components/ui/Card'
import Input from '../../../components/ui/Input'
import Label from '../../../components/ui/Label'

export interface DateRangeFilterProps {
  de: string
  ate: string
  onChange: (next: { de?: string; ate?: string }) => void
  onApply: () => void
  loading?: boolean
}

export default function DateRangeFilter({ de, ate, onChange, onApply, loading }: DateRangeFilterProps) {
  return (
    <Card>
      <CardHeader title="Filtro" />
      <div className="flex flex-wrap items-end gap-3">
        <div>
          <Label>De</Label>
          <Input type="date" value={de} onChange={(e) => onChange({ de: e.target.value })} />
        </div>
        <div>
          <Label>Até</Label>
          <Input type="date" value={ate} onChange={(e) => onChange({ ate: e.target.value })} />
        </div>
        <Button variant="secondary" onClick={onApply} disabled={loading}>
          {loading ? 'Carregando...' : 'Carregar'}
        </Button>
      </div>
    </Card>
  )
}
