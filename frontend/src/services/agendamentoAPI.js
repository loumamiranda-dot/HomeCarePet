import client from './client'

export const agendamentoAPI = {
  listar: (pagina = 1, tamanhoPagina = 100) =>
    client.get('/agendamentos', { params: { pagina, tamanhoPagina } }),
  obter: (id) => client.get(`/agendamentos/${id}`),
  meusAgendamentos: () => client.get('/agendamentos/meus-agendamentos'),
  porStatus: (status) => client.get(`/agendamentos/por-status/${status}`),
  hoje: () => client.get('/agendamentos/hoje'),
  detalhados: () => client.get('/agendamentos/detalhados'),
  criar: (dados) => client.post('/agendamentos', dados),
  atualizar: (id, dados) => client.put(`/agendamentos/${id}`, dados),
  cancelar: (id) => client.put(`/agendamentos/${id}/cancelar`),
  deletar: (id) => client.delete(`/agendamentos/${id}`),
}
