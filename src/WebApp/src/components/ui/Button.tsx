import React from 'react'
import { clsx } from 'clsx'

type Variant = 'primary' | 'secondary' | 'ghost'
type Size = 'sm' | 'md' | 'lg'

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant
  size?: Size
}

export default function Button({ className, variant = 'primary', size = 'md', ...props }: ButtonProps) {
  const base = 'inline-flex items-center justify-center rounded-xl font-medium transition'
  const sizes: Record<Size, string> = {
    sm: 'px-3 py-1.5 text-sm',
    md: 'px-4 py-2 text-sm',
    lg: 'px-5 py-2.5 text-base',
  }
  const variants: Record<Variant, string> = {
    primary: 'bg-brand-600 text-white hover:bg-brand-700 active:bg-brand-800 disabled:opacity-60',
    secondary: 'bg-white border border-slate-300 hover:bg-slate-50 disabled:opacity-60',
    ghost: 'bg-transparent hover:bg-slate-100',
  }
  return <button className={clsx(base, sizes[size], variants[variant], className)} {...props} />
}
