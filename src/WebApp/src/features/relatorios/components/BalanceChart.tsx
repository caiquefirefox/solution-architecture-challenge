import Card, { CardHeader } from '../../../components/ui/Card'
import { Line } from 'react-chartjs-2'
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Tooltip, Legend } from 'chart.js'
ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Tooltip, Legend)

export default function BalanceChart({ labels, data }: { labels: string[]; data: number[] }) {
  const chartData = { labels, datasets: [ { label: 'Saldo acumulado', data, tension: 0.3 } ] }
  return (
    <Card>
      <CardHeader title="Saldo acumulado" />
      <div className="bg-white rounded-lg">
        <Line data={chartData} options={{ responsive: true, plugins: { legend: { display: true } } }} />
      </div>
    </Card>
  )
}
