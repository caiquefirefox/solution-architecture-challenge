import React from 'react'
import { clsx } from 'clsx'

export default function Label({ className, children, ...props }: React.HTMLAttributes<HTMLLabelElement>) {
  return (
    <label className={clsx('text-sm font-medium text-slate-700', className)} {...props}>
      {children}
    </label>
  )
}
