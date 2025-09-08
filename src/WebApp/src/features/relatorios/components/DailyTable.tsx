import Card, { CardHeader } from '../../../components/ui/Card'
import { Table, THead, TH, TR, TD, TBody } from '../../../components/ui/Table'

export interface Daily {
  data: string
  debitos: number
  creditos: number
  saldoDoDia: number
  saldoAcumulado: number
}

export default function DailyTable({ rows }: { rows: Daily[] }) {
  return (
    <Card>
      <CardHeader title="Detalhes por dia" />
      <div className="overflow-x-auto">
        <Table>
          <THead>
            <TR>
              <TH>Data</TH>
              <TH className="text-right">Débitos</TH>
              <TH className="text-right">Créditos</TH>
              <TH className="text-right">Saldo do dia</TH>
              <TH className="text-right">Acumulado</TH>
            </TR>
          </THead>
          <TBody>
            {rows.map((r) => (
              <TR key={r.data}>
                <TD>{r.data}</TD>
                <TD className="text-right">{r.debitos.toFixed(2)}</TD>
                <TD className="text-right">{r.creditos.toFixed(2)}</TD>
                <TD className="text-right">{r.saldoDoDia.toFixed(2)}</TD>
                <TD className="text-right">{r.saldoAcumulado.toFixed(2)}</TD>
              </TR>
            ))}
          </TBody>
        </Table>
      </div>
    </Card>
  )
}
