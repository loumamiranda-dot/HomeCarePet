import client from './client'

export const petAPI = {
  listar: () => client.get('/pets'),
  obter: (id) => client.get(`/pets/${id}`),
  meusPets: () => client.get('/pets/meus-pets'),
  criar: (dados) => client.post('/pets', dados),
  atualizar: (id, dados) => client.put(`/pets/${id}`, dados),
  deletar: (id) => client.delete(`/pets/${id}`),
}
