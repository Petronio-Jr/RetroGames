import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'

import useAutenticacao from '../../hooks/useAutenticacao'
import { obterMensagemErro } from '../../api/erros'

function Cadastro() {
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  const { cadastrar } = useAutenticacao()
  const navegar = useNavigate()

  async function lidarComEnvio(evento) {
    evento.preventDefault()

    setErro('')
    setEnviando(true)

    try {
      await cadastrar(nome, email, senha)
      navegar('/')
    } catch (erro) {
      setErro(obterMensagemErro(erro))
    } finally {
      setEnviando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-md-6 col-lg-4">
        <div className="card shadow-sm mt-4">
          <div className="card-body p-4">

            <h2 className="text-center mb-4">
              Criar conta
            </h2>

            <form onSubmit={lidarComEnvio}>
              <div className="mb-3">
                <label htmlFor="nome" className="form-label">
                  Nome
                </label>

                <input
                  type="text"
                  className="form-control"
                  id="nome"
                  value={nome}
                  onChange={(evento) => setNome(evento.target.value)}
                  required
                />
              </div>

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
                  maxLength={254}
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
                {enviando ? 'Cadastrando...' : 'Criar conta'}
              </button>
            </form>

            <hr />

            <p className="text-center mb-0">
              Já possui uma conta?{' '}
              <Link to="/login">Entrar</Link>
            </p>

          </div>
        </div>
      </div>
    </div>
  )
}

export default Cadastro
