import client from './client'

export const authAPI = {
  entrar: (email, senha) =>
    client.post('/autenticacao/entrar', { email, senha }),

  registrar: (dados) =>
    client.post('/autenticacao/registrar', dados),

  registrarAdmin: (dados) =>
    client.post('/autenticacao/registrar-admin', dados),
}
