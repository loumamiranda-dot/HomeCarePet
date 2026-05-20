import axios from 'axios'

// configura o axios pra sempre mandar pra /api
const client = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
})

// coloca o token em toda requisicao
client.interceptors.request.use((config) => {
  const token = localStorage.getItem('hc_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// se der 401 redireciona pro login
client.interceptors.response.use(
  (res) => res,
  (err) => {
    console.log('erro na requisicao:', err.response?.status, err.config?.url)
    if (err.response?.status === 401) {
      localStorage.removeItem('hc_token')
      localStorage.removeItem('hc_usuario')
      window.location.href = '/login'
    }
    return Promise.reject(err)
  }
)

export default client
