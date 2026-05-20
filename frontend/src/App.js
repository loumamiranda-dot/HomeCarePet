import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './contexts/AuthContext'
import PrivateRoute from './componentes/PrivateRoute/PrivateRoute'
import Layout from './componentes/Layout/Layout'
import AdminLayout from './componentes/AdminLayout/AdminLayout'
import Home from './paginas/Home/Home'
import Login from './paginas/Login/Login'
import Dashboard from './paginas/Dashboard/Dashboard'
import Clientes from './paginas/Clientes/Clientes'
import Pets from './paginas/Pets/Pets'
import Agendamentos from './paginas/Agendamentos/Agendamentos'
import Servicos from './paginas/Servicos/Servicos'

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* paginas que qualquer um pode acessar */}
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />

          {/* rotas do admin */}
          <Route
            path="/admin"
            element={
              <PrivateRoute roles={['Administrador']}>
                <AdminLayout />
              </PrivateRoute>
            }
          >
            <Route index element={<Navigate to="/admin/dashboard" replace />} />
            <Route path="dashboard" element={<Dashboard />} />
            <Route path="clientes" element={<Clientes />} />
            <Route path="pets" element={<Pets />} />
            <Route path="agendamentos" element={<Agendamentos />} />
            <Route path="servicos" element={<Servicos />} />
          </Route>

          {/* rotas do funcionario e cliente */}
          <Route
            path="/app"
            element={
              <PrivateRoute roles={['Funcionario', 'Cliente']}>
                <Layout />
              </PrivateRoute>
            }
          >
            <Route index element={<Navigate to="/app/dashboard" replace />} />
            <Route path="dashboard" element={<Dashboard />} />
            <Route path="pets" element={<Pets />} />
            <Route path="agendamentos" element={<Agendamentos />} />
            <Route
              path="clientes"
              element={
                <PrivateRoute roles={['Funcionario']}>
                  <Clientes />
                </PrivateRoute>
              }
            />
            <Route
              path="servicos"
              element={
                <PrivateRoute roles={['Funcionario']}>
                  <Servicos />
                </PrivateRoute>
              }
            />
          </Route>

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}
