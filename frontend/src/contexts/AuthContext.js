import { createContext, useContext, useState, useEffect } from 'react'
import { jwtDecode } from 'jwt-decode'
import { authAPI } from '../services/authAPI'

const AuthContext = createContext(null)

// essas urls sao os nomes dos claims que o .NET coloca no token jwt
// demorei pra descobrir isso kk
const CLAIM_ID    = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
const CLAIM_NOME  = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
const CLAIM_EMAIL = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'
const CLAIM_ROLE  = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'

export function AuthProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  // quando abre a pagina, verifica se ja tem token salvo
  useEffect(() => {
    const token = localStorage.getItem('hc_token')
    if (token) {
      try {
        const decoded = jwtDecode(token)
        // checa se o token ainda nao expirou
        if (decoded.exp * 1000 > Date.now()) {
          setUsuario(JSON.parse(localStorage.getItem('hc_usuario') || 'null'))
        } else {
          localStorage.removeItem('hc_token')
          localStorage.removeItem('hc_usuario')
        }
      } catch {
        localStorage.removeItem('hc_token')
        localStorage.removeItem('hc_usuario')
      }
    }
    setCarregando(false)
  }, [])

  async function entrar(email, senha) {
    const res = await authAPI.entrar(email, senha)
    const token = res.data.token
    const d = jwtDecode(token)
    const usuarioData = {
      id: d[CLAIM_ID],
      nome: d[CLAIM_NOME],
      email: d[CLAIM_EMAIL],
      role: d[CLAIM_ROLE],
    }
    localStorage.setItem('hc_token', token)
    localStorage.setItem('hc_usuario', JSON.stringify(usuarioData))
    console.log('usuario logado:', usuarioData.nome, usuarioData.role)
    setUsuario(usuarioData)
    return usuarioData
  }

  function sair() {
    localStorage.removeItem('hc_token')
    localStorage.removeItem('hc_usuario')
    setUsuario(null)
  }

  return (
    <AuthContext.Provider value={{
      usuario,
      carregando,
      entrar,
      sair,
      isAdmin: usuario?.role === 'Administrador',
      isFuncionario: usuario?.role === 'Funcionario',
      isCliente: usuario?.role === 'Cliente',
    }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => useContext(AuthContext)
