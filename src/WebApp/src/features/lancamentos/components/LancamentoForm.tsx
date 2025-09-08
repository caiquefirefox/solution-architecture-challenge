import { useState } from 'react'
import Button from '../../../components/ui/Button'
import Card, { CardHeader } from '../../../components/ui/Card'
import Input from '../../../components/ui/Input'
import Label from '../../../components/ui/Label'
import Select from '../../../components/ui/Select'

export type TipoLancamento = 1 | 2

export interface LancamentoFormValues {
  data: string
  tipo: TipoLancamento
  valor: number
  descricao: string
}

export default function LancamentoForm({ initialDate, onSubmit }: { initialDate: string; onSubmit: (v: LancamentoFormValues) => Promise<boolean> }) {
  const [form, setForm] = useState({ data: initialDate, tipo: 1 as TipoLancamento, valor: '', descricao: '' })
  function set<K extends keyof typeof form>(k: K, v: any) { setForm({ ...form, [k]: v }) }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    const ok = await onSubmit({ data: form.data, tipo: form.tipo, valor: Number(form.valor), descricao: form.descricao })
    if (ok) setForm({ data: initialDate, tipo: 1, valor: '', descricao: '' } as any)
  }

  return (
    <Card>
      <CardHeader title="Novo lançamento" />
      <form onSubmit={handleSubmit} className="grid md:grid-cols-5 gap-3">
        <div>
          <Label>Data</Label>
          <Input type="date" value={form.data} onChange={(e) => set('data', e.target.value)} />
        </div>
        <div>
          <Label>Tipo</Label>
          <Select value={form.tipo} onChange={(e) => set('tipo', Number(e.target.value) as any)}>
            <option value={1}>Débito</option>
            <option value={2}>Crédito</option>
          </Select>
        </div>
        <div>
          <Label>Valor</Label>
          <Input placeholder="0,00" value={form.valor} onChange={(e) => set('valor', e.target.value)} />
        </div>
        <div className="md:col-span-2">
          <Label>Descrição</Label>
          <Input placeholder="Descrição" value={form.descricao} onChange={(e) => set('descricao', e.target.value)} />
        </div>
        <div className="md:col-span-5">
          <Button type="submit">Adicionar</Button>
        </div>
      </form>
    </Card>
  )
}
