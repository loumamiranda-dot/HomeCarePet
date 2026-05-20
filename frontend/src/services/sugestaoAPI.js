import client from './client'

// fiz diferente dos outros services pq so tem um endpoint mesmo
export function minhasSugestoes() {
  return client.get('/sugestoes/minhas-sugestoes')
}

export const sugestaoAPI = {
  minhasSugestoes,
}
