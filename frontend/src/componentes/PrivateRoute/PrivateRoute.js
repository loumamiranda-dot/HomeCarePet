import { Navigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'

export default function PrivateRoute({ children, roles }) {
  const { usuario, carregando } = useAuth()

  if (carregando) {
    return (
      <div className="loading-full">
        <div className="spinner" />
      </div>
    )
  }

  if (!usuario) return <Navigate to="/login" replace />

  if (roles && !roles.includes(usuario.role)) {
    // Admin tentando acessar /app → redireciona para /admin
    if (usuario.role === 'Administrador') return <Navigate to="/admin/dashboard" replace />
    // Outros tentando acessar /admin → redireciona para /app
    return <Navigate to="/app/dashboard" replace />
  }

  return children
}
