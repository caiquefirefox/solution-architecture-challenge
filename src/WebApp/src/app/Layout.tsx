import React from 'react'
import Navbar from '../components/layout/Navbar'
import { useAuth } from '../features/auth/AuthContext'

export default function Layout({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, logout, userName } = useAuth()
  return (
    <div>
      <Navbar isAuthenticated={isAuthenticated} userName={userName} onLogout={logout} />
      <main className="container py-8">{children}</main>
    </div>
  )
}
