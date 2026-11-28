import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'

import Paginacao from '../../components/Paginacao'

import {
  Carregando,
  MensagemErro,
  EstadoVazio,
} from '../../components/MensagensEstado'

function Jogos() {
  const [jogos, setJogos] = useState([])

  const [busca, setBusca] = useState('')
  const [buscaAplicada, setBuscaAplicada] = useState('')

  const [pagina, setPagina] = useState(1)
  const [dadosPaginacao, setDadosPaginacao] = useState({
    pagina: 1,
    totalPaginas: 1,
    totalItens: 0,
  })

  const [carregando, setCarregando] = useState(true)
  const [erroLista, setErroLista] = useState('')
  const [erroAcao, setErroAcao] = useState('')
  const [idDesativando, setIdDesativando] = useState(null)

  useEffect(() => {
    let ativo = true

    async function carregarJogos() {
      setCarregando(true)
      setErroLista('')

      try {
        const resultado = await jogoApi.listar({
          busca: buscaAplicada || undefined,
          pagina,
          tamanhoPagina: 12,
          incluirInativos: true,
        })

        if (ativo) {
          setJogos(resultado.itens)
          setDadosPaginacao({
            pagina: resultado.pagina,
            totalPaginas: resultado.totalPaginas,
            totalItens: resultado.totalItens,
          })
        }
      } catch (erro) {
        if (ativo) {
          setErroLista(obterMensagemErro(erro))
        }
      } finally {
        if (ativo) {
          setCarregando(false)
        }
      }
    }

    carregarJogos()

    return () => {
      ativo = false
    }
  }, [buscaAplicada, pagina])

  function pesquisar(evento) {
    evento.preventDefault()
    setPagina(1)
    setBuscaAplicada(busca.trim())
  }

  function mudarPagina(novaPagina) {
    setPagina(novaPagina)
  }

  async function desativarJogo(jogo) {
    const confirmou = window.confirm(
      `Deseja realmente desativar o jogo "${jogo.nome}"?`
    )

    if (!confirmou) {
      return
    }

    setErroAcao('')
    setIdDesativando(jogo.id)

    try {
      await jogoApi.desativar(jogo.id)

      // Recarrega a lista após a desativação.
      const resultado = await jogoApi.listar({
        busca: buscaAplicada || undefined,
        pagina,
        tamanhoPagina: 12,
        incluirInativos: true,
      })

      setJogos(resultado.itens)
      setDadosPaginacao({
        pagina: resultado.pagina,
        totalPaginas: resultado.totalPaginas,
        totalItens: resultado.totalItens,
      })
    } catch (erro) {
      setErroAcao(obterMensagemErro(erro))
    } finally {
      setIdDesativando(null)
    }
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
        <div>
          <h1 className="h2 mb-1">Administração de Jogos</h1>
          <p className="text-muted mb-0">
            Gerencie os jogos cadastrados no sistema.
          </p>
        </div>

        <Link to="/admin/jogos/novo" className="btn btn-primary">
          Novo jogo
        </Link>
      </div>

      <form onSubmit={pesquisar} className="row g-2 mb-4">
        <div className="col-md-8">
          <input
            type="text"
            className="form-control"
            placeholder="Buscar jogo pelo nome..."
            value={busca}
            onChange={(evento) => setBusca(evento.target.value)}
          />
        </div>

        <div className="col-auto">
          <button type="submit" className="btn btn-outline-primary">
            Buscar
          </button>
        </div>

        <div className="col-auto">
          <button
            type="button"
            className="btn btn-outline-secondary"
            onClick={() => {
              setBusca('')
              setBuscaAplicada('')
              setPagina(1)
            }}
          >
            Limpar
          </button>
        </div>
      </form>

      {erroAcao && (
        <div className="alert alert-danger" role="alert">
          {erroAcao}
        </div>
      )}

      {carregando ? (
        <Carregando />
      ) : erroLista ? (
        <MensagemErro
          mensagem={erroLista}
          aoTentarNovamente={() => window.location.reload()}
        />
      ) : jogos.length === 0 ? (
        <EstadoVazio titulo="Nenhum jogo encontrado">
          Não há jogos cadastrados com esse filtro.
        </EstadoVazio>
      ) : (
        <>
          <div className="d-flex justify-content-between align-items-center mb-3">
            <h2 className="h5 mb-0">Jogos cadastrados</h2>

            <span className="text-muted">
              {dadosPaginacao.totalItens} jogo(s)
            </span>
          </div>

          <div className="table-responsive">
            <table className="table table-striped table-hover align-middle tabela-cartoes">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Plataforma</th>
                  <th>Ano</th>
                  <th>Preço</th>
                  <th>Estoque</th>
                  <th>Situação</th>
                  <th>Ações</th>
                </tr>
              </thead>

              <tbody>
                {jogos.map((jogo) => (
                  <tr key={jogo.id}>
                    <td data-label="Nome">{jogo.nome}</td>
                    <td data-label="Plataforma">{jogo.nomePlataforma}</td>
                    <td data-label="Ano">{jogo.ano}</td>
                    <td data-label="Preço">{formatarPreco(jogo.preco)}</td>
                    <td data-label="Estoque">{jogo.estoque}</td>

                    <td data-label="Situação">
                      {jogo.ativo ? (
                        <span className="badge bg-success">
                          Ativo
                        </span>
                      ) : (
                        <span className="badge bg-secondary">
                          Inativo
                        </span>
                      )}
                    </td>

                    <td data-label="Ações">
                      <div className="d-flex gap-2">
                        <Link
                          to={`/admin/jogos/${jogo.id}/editar`}
                          className="btn btn-sm btn-outline-primary"
                        >
                          Editar
                        </Link>

                        {jogo.ativo && (
                          <button
                            type="button"
                            className="btn btn-sm btn-outline-danger"
                            disabled={idDesativando === jogo.id}
                            onClick={() => desativarJogo(jogo)}
                          >
                            {idDesativando === jogo.id
                              ? 'Desativando...'
                              : 'Desativar'}
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <Paginacao
            pagina={dadosPaginacao.pagina}
            totalPaginas={dadosPaginacao.totalPaginas}
            aoMudar={mudarPagina}
          />
        </>
      )}
    </div>
  )
}

export default Jogos
