import client from './client'

export const clienteAPI = {
  listar: (pagina = 1, tamanhoPagina = 100) =>
    client.get('/clientes', { params: { pagina, tamanhoPagina } }),
  obter: (id) => client.get(`/clientes/${id}`),
  meuPerfil: () => client.get('/clientes/meu-perfil'),
  criar: (dados) => client.post('/clientes', dados),
  atualizar: (id, dados) => client.put(`/clientes/${id}`, dados),
  deletar: (id) => client.delete(`/clientes/${id}`),
}
