
import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import useAutenticacao from '../../hooks/useAutenticacao'
import { obterMensagemErro } from '../../api/erros'

function Login() {
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  const [parametros] = useSearchParams()
  const navegar = useNavigate()

  const { entrar } = useAutenticacao()

  async function lidarComEnvio(evento) {
    evento.preventDefault()

    setErro('')
    setEnviando(true)

    try {
      await entrar(email, senha)

      const voltarPara = parametros.get('voltarPara')

      const destinoValido =
        voltarPara &&
        voltarPara.startsWith('/') &&
        !voltarPara.startsWith('//')

      navegar(destinoValido ? voltarPara : '/')
    } catch (erro) {
      setErro(obterMensagemErro(erro))
    } finally {
      setEnviando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-md-6 col-lg-4">
        <div className="card shadow-sm mt-5">
          <div className="card-body p-4">

            <h2 className="text-center mb-4">
              Entrar
            </h2>

            <form onSubmit={lidarComEnvio}>
              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  E-mail
                </label>

                <input
                  type="email"
                  className="form-control"
                  id="email"
                  value={email}
                  onChange={(evento) => setEmail(evento.target.value)}
                  required
                />
              </div>

              <div className="mb-3">
                <label htmlFor="senha" className="form-label">
                  Senha
                </label>

                <input
                  type="password"
                  className="form-control"
                  id="senha"
                  value={senha}
                  onChange={(evento) => setSenha(evento.target.value)}
                  required
                />
              </div>

              {erro && (
                <div className="alert alert-danger" role="alert">
                  {erro}
                </div>
              )}

              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={enviando}
              >
                {enviando ? 'Entrando...' : 'Entrar'}
              </button>
            </form>

          </div>
        </div>
      </div>
    </div>
  )
}

export default Login
