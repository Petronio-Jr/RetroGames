import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

import { pedidoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'

import {
  Carregando,
  MensagemErro,
  EstadoVazio,
} from '../../components/MensagensEstado'

function MeusPedidos() {
  const [pedidos, setPedidos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')

  async function carregarPedidos() {
    setCarregando(true)
    setErro('')

    try {
      const resultado = await pedidoApi.listarMeus()
      setPedidos(resultado)
    } catch (erro) {
      setErro(obterMensagemErro(erro))
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregarPedidos()
  }, [])

  function formatarData(data) {
    return new Date(data).toLocaleString('pt-BR')
  }

  return (
    <div>
      <div className="mb-4">
        <h1>Meus pedidos</h1>
        <p className="text-muted">
          Acompanhe suas compras na RetroGames.
        </p>
      </div>

      {carregando ? (
        <Carregando />
      ) : erro ? (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={carregarPedidos}
        />
      ) : pedidos.length === 0 ? (
        <EstadoVazio titulo="Você ainda não possui pedidos">
          <p className="mb-3">
            Quando realizar uma compra, ela aparecerá aqui.
          </p>

          <Link to="/" className="btn btn-primary">
            Explorar jogos
          </Link>
        </EstadoVazio>
      ) : (
        <div className="d-flex flex-column gap-4">
          {pedidos.map((pedido) => (
            <div className="card shadow-sm" key={pedido.id}>
              <div className="card-body">
                <div className="d-flex flex-wrap justify-content-between align-items-start gap-2 mb-3">
                  <div>
                    <h2 className="h5 mb-1">
                      Pedido #{pedido.id}
                    </h2>

                    <small className="text-muted">
                      {formatarData(pedido.dataPedido)}
                    </small>
                  </div>

                  <span className="badge bg-secondary">
                    {pedido.situacao}
                  </span>
                </div>

                <hr />

                <h3 className="h6 mb-3">Itens do pedido</h3>

                <div className="table-responsive">
                  <table className="table align-middle tabela-cartoes">
                    <thead>
                      <tr>
                        <th>Jogo</th>
                        <th>Quantidade</th>
                        <th>Valor unitário</th>
                        <th>Subtotal</th>
                      </tr>
                    </thead>

                    <tbody>
                      {pedido.itens.map((item) => (
                        <tr key={item.idJogo}>
                          <td data-label="Jogo">{item.nomeJogo}</td>
                          <td data-label="Quantidade">{item.quantidade}</td>
                          <td data-label="Valor unitário">{formatarPreco(item.valorUnitario)}</td>
                          <td data-label="Subtotal">
                            {formatarPreco(
                              item.quantidade * item.valorUnitario
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                <div className="text-end">
                  <span className="text-muted me-2">
                    Total do pedido:
                  </span>

                  <strong className="fs-5">
                    {formatarPreco(pedido.total)}
                  </strong>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

export default MeusPedidos
