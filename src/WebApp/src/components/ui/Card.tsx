import React from 'react'
import { clsx } from 'clsx'

export default function Card({ className, children }: { className?: string; children: React.ReactNode }) {
  return <div className={clsx('bg-white rounded-2xl shadow-sm border border-slate-200 p-4', className)}>{children}</div>
}

export function CardHeader({ title, description }: { title: string; description?: string }) {
  return (
    <div className="mb-3">
      <h2 className="text-lg font-semibold">{title}</h2>
      {description && <p className="text-sm text-slate-600">{description}</p>}
    </div>
  )
}

export function CardContent({ children }: { children: React.ReactNode }) {
  return <div>{children}</div>
}
