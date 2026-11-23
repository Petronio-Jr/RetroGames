import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

api.interceptors.response.use(
  (resposta) => resposta,
  (erro) => {
    if (erro.response?.status === 401) {
      const rota = erro.config?.url || ''

      const ehConsultaUsuario = rota.includes('/Autenticacao/eu')
      const ehLogin = rota.includes('/Autenticacao/login')

      if (!ehConsultaUsuario && !ehLogin) {
        window.dispatchEvent(new Event('sessao:expirada'))
      }
    }

    return Promise.reject(erro)
  }
)

export default api