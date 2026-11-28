import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'

import useCarrinho from '../../hooks/useCarrinho'
import useAutenticacao from '../../hooks/useAutenticacao'

import { pedidoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'

function Carrinho() {
  const {
    itens,
    total,
    alterarQuantidade,
    removerItem,
    limparCarrinho,
  } = useCarrinho()

  const { estaAutenticado, carregando } = useAutenticacao()

  const [erro, setErro] = useState('')
  const [finalizando, setFinalizando] = useState(false)

  const navegar = useNavigate()

  async function finalizarCompra() {
    setErro('')

    // O visitante precisa entrar ou criar uma conta.
    if (!estaAutenticado) {
      navegar('/login?voltarPara=%2Fcarrinho')
      return
    }

    setFinalizando(true)

    try {
      await pedidoApi.finalizar(itens)

      limparCarrinho()
      navegar('/meus-pedidos')
    } catch (erro) {
      setErro(obterMensagemErro(erro))
    } finally {
      setFinalizando(false)
    }
  }

  return (
    <div className="pagina-carrinho">
      <div className="mb-4">
        <h1>Meu carrinho</h1>

        <p className="text-muted">
          Confira seus jogos antes de finalizar a compra.
        </p>
      </div>

      {!estaAutenticado && !carregando && (
        <div className="alert alert-info" role="status">
          Você está navegando como visitante. Seus itens serão
          mantidos no carrinho deste navegador. Para finalizar
          a compra, será necessário entrar ou criar uma conta.
        </div>
      )}

      {erro && (
        <div className="alert alert-danger" role="alert">
          {erro}
        </div>
      )}

      {itens.length === 0 ? (
        <div className="alert alert-secondary text-center py-4">
          <h2 className="h5">Seu carrinho está vazio</h2>

          <p className="mb-3">
            Você ainda não adicionou nenhum jogo.
          </p>

          <Link to="/" className="btn btn-primary">
            Explorar jogos
          </Link>
        </div>
      ) : (
        <>
          <Link to="/" className="btn btn-outline-secondary mb-3 d-md-none">
            ← Continuar comprando
          </Link>

          <div className="card shadow-sm mb-4">
            <div className="card-body">
              {itens.map(item => (
                <div
                  key={item.id}
                  className="row align-items-center border-bottom py-3"
                >
                  <div className="col-md-4">
                    <h2 className="h6 mb-1">
                      {item.nome}
                    </h2>

                    <span className="text-muted">
                      {formatarPreco(item.preco)} por unidade
                    </span>
                  </div>

                  <div className="col-md-3 my-2 my-md-0">
                    <label
                      htmlFor={`quantidade-${item.id}`}
                      className="form-label mb-1"
                    >
                      Quantidade
                    </label>

                    <select
                      id={`quantidade-${item.id}`}
                      className="form-select"
                      value={item.quantidade}
                      disabled={finalizando}
                      onChange={evento =>
                        alterarQuantidade(
                          item.id,
                          Number(evento.target.value)
                        )
                      }
                    >
                      {Array.from(
                        { length: Math.min(10, item.estoque) },
                        (_, indice) => indice + 1
                      ).map(quantidade => (
                        <option
                          key={quantidade}
                          value={quantidade}
                        >
                          {quantidade}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="col-md-3 my-2 my-md-0">
                    <span className="text-muted">
                      Subtotal
                    </span>

                    <p className="fw-bold mb-0">
                      {formatarPreco(
                        item.preco * item.quantidade
                      )}
                    </p>
                  </div>

                  <div className="col-md-2 text-md-end">
                    <button
                      type="button"
                      className="btn btn-outline-danger btn-sm"
                      disabled={finalizando}
                      onClick={() => removerItem(item.id)}
                    >
                      Remover
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="card shadow-sm barra-acao-fixa">
            <div className="card-body">
              <div className="barra-carrinho">
                <div className="barra-carrinho-total">
                  <span className="text-muted small">
                    Total da compra
                  </span>

                  <span className="fs-4 fw-bold">
                    {formatarPreco(total)}
                  </span>
                </div>

                <div className="barra-carrinho-acoes">
                  <Link
                    to="/"
                    className="btn btn-outline-secondary d-none d-md-inline-flex align-items-center"
                  >
                    Continuar comprando
                  </Link>

                  <button
                    type="button"
                    className="btn btn-primary"
                    disabled={
                      finalizando ||
                      carregando ||
                      itens.length === 0
                    }
                    onClick={finalizarCompra}
                  >
                    {finalizando
                      ? 'Finalizando...'
                      : 'Finalizar compra'}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  )
}

export default Carrinho