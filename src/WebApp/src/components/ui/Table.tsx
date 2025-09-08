import React from 'react'
import { clsx } from 'clsx'

export function Table({ className, children }: { className?: string; children: React.ReactNode }) {
  return <table className={clsx('w-full text-sm', className)}>{children}</table>
}
export function THead({ children }: { children: React.ReactNode }) {
  return <thead className="bg-slate-100 text-slate-700 font-semibold">{children}</thead>
}
export function TR({ children, className }: { children: React.ReactNode; className?: string }) {
  return <tr className={className}>{children}</tr>
}
export function TH({ children, className }: { children: React.ReactNode; className?: string }) {
  return <th className={clsx('text-left px-3 py-2', className)}>{children}</th>
}
export function TD({ children, className }: { children: React.ReactNode; className?: string }) {
  return <td className={clsx('border-t border-slate-200 px-3 py-2', className)}>{children}</td>
}
export function TBody({ children }: { children: React.ReactNode }) {
  return <tbody>{children}</tbody>
}
export function TFoot({ children }: { children: React.ReactNode }) {
  return <tfoot className="bg-slate-50 font-semibold">{children}</tfoot>
}
