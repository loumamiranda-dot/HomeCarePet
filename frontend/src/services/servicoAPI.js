import client from './client'

export const servicoAPI = {
  listar: () => client.get('/servicos'),
  obter: (id) => client.get(`/servicos/${id}`),
  criar: (dados) => client.post('/servicos', dados),
  atualizar: (id, dados) => client.put(`/servicos/${id}`, dados),
  deletar: (id) => client.delete(`/servicos/${id}`),
}
