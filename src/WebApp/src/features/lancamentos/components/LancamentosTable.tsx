import Card, { CardHeader } from '../../../components/ui/Card'
import { Table, THead, TH, TR, TD, TBody } from '../../../components/ui/Table'
import Button from '../../../components/ui/Button'

export type TipoLancamento = 1 | 2

export interface Lancamento {
  id: string
  data: string
  tipo: TipoLancamento
  valor: number
  descricao: string
}

export default function LancamentosTable({ items, onDelete }: { items: Lancamento[]; onDelete?: (id: string) => Promise<void> }) {
  return (
    <Card>
      <CardHeader title="Lançamentos" />
      <div className="overflow-x-auto">
        <Table>
          <THead>
            <TR>
              <TH>Data</TH>
              <TH>Tipo</TH>
              <TH>Descrição</TH>
              <TH className="text-right">Valor</TH>
              {onDelete && <TH className="text-right">Ações</TH>}
            </TR>
          </THead>
          <TBody>
            {items.map((l) => (
              <TR key={l.id}>
                <TD>{l.data}</TD>
                <TD>{l.tipo === 1 ? 'Débito' : 'Crédito'}</TD>
                <TD>{l.descricao}</TD>
                <TD className="text-right">{l.valor.toFixed(2)}</TD>
                {onDelete && (
                  <TD className="text-right">
                    <Button variant="secondary" size="sm" onClick={() => onDelete(l.id)}>Excluir</Button>
                  </TD>
                )}
              </TR>
            ))}
          </TBody>
        </Table>
      </div>
    </Card>
  )
}
