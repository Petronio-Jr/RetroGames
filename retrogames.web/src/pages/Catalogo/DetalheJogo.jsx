import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'

import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'
import {
  Carregando,
  MensagemErro,
} from '../../components/MensagensEstado'
import useCarrinho from '../../hooks/useCarrinho'
import useAutenticacao from '../../hooks/useAutenticacao'
import CapaJogo from '../../components/CapaJogo'

function DetalheJogo() {
  const { id } = useParams()

  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [mensagemCarrinho, setMensagemCarrinho] = useState('')

  const { adicionarItem, itens } = useCarrinho()

  const {
    ehAdministrador,
    carregando: carregandoAutenticacao,
  } = useAutenticacao()

  useEffect(() => {
    async function carregarJogo() {
      try {
        setCarregando(true)
        setErro('')
        setJogo(null)

        const dados = await jogoApi.buscarPorId(id)
        setJogo(dados)
      } catch (erro) {
        setErro(obterMensagemErro(erro))
      } finally {
        setCarregando(false)
      }
    }

    carregarJogo()
  }, [id])

  if (carregando) {
    return <Carregando texto="Carregando jogo..." />
  }

  if (erro) {
    return (
      <div>
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={() => window.location.reload()}
        />

        <Link to="/" className="btn btn-outline-primary mt-3">
          Voltar ao catálogo
        </Link>
      </div>
    )
  }

  if (!jogo) {
    return null
  }

  function lidarComAdicao() {
    if (carregandoAutenticacao) {
      setMensagemCarrinho(
        'Aguarde enquanto verificamos sua sessão.'
      )
      return
    }

    if (ehAdministrador) {
      setMensagemCarrinho(
        'Ação não permitida: contas administrativas não podem adicionar produtos ao carrinho. Entre com uma conta de cliente para realizar compras.'
      )
      return
    }

    const itemExistente = itens.find(
      item => item.id === jogo.id
    )

    const quantidadeAtual = itemExistente
      ? itemExistente.quantidade
      : 0

    if (jogo.estoque <= 0) {
      setMensagemCarrinho('Este jogo está esgotado.')
      return
    }

    if (
      quantidadeAtual >= 10 ||
      quantidadeAtual >= jogo.estoque
    ) {
      setMensagemCarrinho(
        `Você já atingiu o limite de ${Math.min(10, jogo.estoque)} unidade(s) deste jogo.`
      )
      return
    }

    const adicionado = adicionarItem(jogo)

    setMensagemCarrinho(
      adicionado
        ? 'Jogo adicionado ao carrinho!'
        : 'Não foi possível adicionar o jogo.'
    )
  }

  return (
    <div>
      <Link to="/" className="btn btn-outline-secondary mb-4">
        ← Voltar ao catálogo
      </Link>

      <div className="card shadow-sm">
        <CapaJogo jogo={jogo} className="capa-detalhe" />

        <div className="card-body p-4">
          <div className="d-flex justify-content-between align-items-start flex-wrap gap-3 mb-3">
            <div>
              <p className="text-muted mb-1">
                {jogo.nomePlataforma}
              </p>

              <h1 className="h2 mb-0">{jogo.nome}</h1>
            </div>

            <span className="badge bg-secondary fs-6">
              {jogo.descricaoCondicao}
            </span>
          </div>

          <hr />

          <div className="row g-3">
            <div className="col-md-6">
              <strong>Ano de lançamento:</strong>
              <p>{jogo.ano}</p>
            </div>

            <div className="col-md-6">
              <strong>Gêneros:</strong>
              <p>
                {jogo.generos?.length
                  ? jogo.generos
                      .map(genero =>
                        typeof genero === 'string'
                          ? genero
                          : genero.nome
                      )
                      .join(', ')
                  : 'Não informado'}
              </p>
            </div>

            <div className="col-12">
              <strong>Descrição:</strong>
              <p>
                {jogo.descricao ||
                  'Nenhuma descrição cadastrada.'}
              </p>
            </div>
          </div>

          <hr />

          <div className="d-flex justify-content-between align-items-center flex-wrap gap-3">
            <div>
              <strong>Preço:</strong>
              <p className="text-success fs-3 fw-bold mb-0">
                {formatarPreco(jogo.preco)}
              </p>
            </div>

            <div className="text-end">
              <strong>Estoque:</strong>

              {jogo.estoque > 0 ? (
                <p className="mb-0">
                  {jogo.estoque} unidade(s)
                </p>
              ) : (
                <p className="text-danger fw-bold mb-0">
                  Esgotado
                </p>
              )}
            </div>
          </div>

          <hr />

          <div className="barra-acao-fixa">
            <button
              className="btn btn-success w-100"
              disabled={
                jogo.estoque <= 0 ||
                carregandoAutenticacao
              }
              onClick={lidarComAdicao}
            >
              {jogo.estoque > 0
                ? 'Adicionar ao carrinho'
                : 'Produto esgotado'}
            </button>

            {mensagemCarrinho && (
              <div
                className={`alert ${
                  ehAdministrador
                    ? 'alert-warning'
                    : 'alert-info'
                } text-center mt-3 mb-0`}
              >
                {mensagemCarrinho}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}

export default DetalheJogo