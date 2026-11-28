import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'

import {
  jogoApi,
  plataformaApi,
  generoApi,
} from '../../api/servicos'

import { obterMensagemErro } from '../../api/erros'

import CardJogo from '../../components/CardJogo'
import Paginacao from '../../components/Paginacao'
import HeroCatalogo from '../../components/HeroCatalogo'

import {
  Carregando,
  MensagemErro,
  EstadoVazio,
} from '../../components/MensagensEstado'

function Catalogo() {
  const [searchParams, setSearchParams] = useSearchParams()

  const [jogos, setJogos] = useState([])
  const [dadosPaginacao, setDadosPaginacao] = useState({
    pagina: 1,
    totalPaginas: 1,
    totalItens: 0,
  })

  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])

  const [busca, setBusca] = useState(searchParams.get('busca') || '')

  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')

  const [erroFiltros, setErroFiltros] = useState('')

  const idPlataforma = searchParams.get('idPlataforma') || ''
  const idGenero = searchParams.get('idGenero') || ''
  const pagina = Number(searchParams.get('pagina')) || 1

  // Mantém os campos atualizados quando a URL muda.
  useEffect(() => {
    setBusca(searchParams.get('busca') || '')
  }, [searchParams])

  // Carrega as plataformas uma vez.
  useEffect(() => {
    async function carregarPlataformas() {
      try {
        const resultado = await plataformaApi.listar()
        setPlataformas(resultado)
      } catch (erro) {
        setErroFiltros(obterMensagemErro(erro))
      }
    }

    carregarPlataformas()
  }, [])

  // Carrega os gêneros uma vez.
  useEffect(() => {
    async function carregarGeneros() {
      try {
        const resultado = await generoApi.listar()
        setGeneros(resultado)
      } catch (erro) {
        setErroFiltros(obterMensagemErro(erro))
      }
    }

    carregarGeneros()
  }, [])

  // Busca os jogos sempre que os parâmetros da URL mudarem.
  useEffect(() => {
    let ativo = true

    async function carregarJogos() {
      setCarregando(true)
      setErro('')

      try {
        const filtros = {
          pagina,
          tamanhoPagina: 12,
        }

        const buscaAtual = searchParams.get('busca')
        const plataformaAtual = searchParams.get('idPlataforma')
        const generoAtual = searchParams.get('idGenero')

        if (buscaAtual) {
          filtros.busca = buscaAtual
        }

        if (plataformaAtual) {
          filtros.idPlataforma = plataformaAtual
        }

        if (generoAtual) {
          filtros.idGenero = generoAtual
        }

        const resultado = await jogoApi.listar(filtros)

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
          setErro(obterMensagemErro(erro))
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
  }, [searchParams, pagina])

  // Atualiza a URL e volta para a primeira página.
  function atualizarFiltros(novosFiltros) {
    const parametros = new URLSearchParams(searchParams)

    Object.entries(novosFiltros).forEach(([chave, valor]) => {
      if (valor) {
        parametros.set(chave, valor)
      } else {
        parametros.delete(chave)
      }
    })

    parametros.set('pagina', '1')

    setSearchParams(parametros)
  }

  function pesquisar(evento) {
    evento.preventDefault()

    atualizarFiltros({
      busca,
    })
  }

  function mudarPagina(novaPagina) {
    const parametros = new URLSearchParams(searchParams)

    parametros.set('pagina', String(novaPagina))

    setSearchParams(parametros)

    document
      .getElementById('jogos')
      ?.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }

  function limparFiltros() {
    setBusca('')
    setSearchParams({})
  }

  return (
    <div>
      <HeroCatalogo />

      <form id="jogos" onSubmit={pesquisar} className="row g-3 mb-4">
        <div className="col-12">
          <label htmlFor="busca" className="form-label">
            Buscar jogo
          </label>

          <input
            id="busca"
            type="search"
            enterKeyHint="search"
            autoComplete="off"
            className="form-control"
            placeholder="Digite o nome do jogo..."
            value={busca}
            onChange={(evento) => setBusca(evento.target.value)}
          />
        </div>

        <div className="col-12 d-md-none">
          <button
            type="button"
            className="btn btn-outline-secondary w-100"
            data-bs-toggle="collapse"
            data-bs-target="#filtrosAvancados"
            aria-controls="filtrosAvancados"
            aria-expanded="false"
          >
            Filtros (plataforma e gênero)
          </button>
        </div>

        <div id="filtrosAvancados" className="col-12 collapse">
        <div className="row g-3">
        <div className="col-md-6">
          <label htmlFor="plataforma" className="form-label">
            Plataforma
          </label>

          <select
            id="plataforma"
            className="form-select"
            value={idPlataforma}
            onChange={(evento) =>
              atualizarFiltros({
                idPlataforma: evento.target.value,
              })
            }
          >
            <option value="">Todas as plataformas</option>

            {plataformas.map((plataforma) => (
              <option key={plataforma.id} value={plataforma.id}>
                {plataforma.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-md-6">
          <label htmlFor="genero" className="form-label">
            Gênero
          </label>

          <select
            id="genero"
            className="form-select"
            value={idGenero}
            onChange={(evento) =>
              atualizarFiltros({
                idGenero: evento.target.value,
              })
            }
          >
            <option value="">Todos os gêneros</option>

            {generos.map((genero) => (
              <option key={genero.id} value={genero.id}>
                {genero.nome}
              </option>
            ))}
          </select>
        </div>

        </div>
        </div>

        <div className="col-12 d-flex gap-2">
          <button type="submit" className="btn btn-primary flex-grow-1">
            Buscar
          </button>

          <button
            type="button"
            className="btn btn-outline-secondary"
            onClick={limparFiltros}
          >
            Limpar
          </button>
        </div>
      </form>

      {erroFiltros && (
        <div className="alert alert-warning">
          Não foi possível carregar os filtros: {erroFiltros}
        </div>
      )}

      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2 className="h4 mb-0">Jogos disponíveis</h2>

        {!carregando && !erro && (
          <span className="text-muted">
            {dadosPaginacao.totalItens} jogos encontrados
          </span>
        )}
      </div>

      {carregando ? (
        <Carregando />
      ) : erro ? (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={() => {
            setSearchParams(new URLSearchParams(searchParams))
          }}
        />
      ) : jogos.length === 0 ? (
        <EstadoVazio titulo="Nenhum jogo encontrado">
          <p className="text-muted">
            Não encontramos jogos com os filtros selecionados.
          </p>

          <button
            className="btn btn-outline-primary"
            onClick={limparFiltros}
          >
            Limpar filtros
          </button>
        </EstadoVazio>
      ) : (
        <>
          <div className="row g-2 g-md-4" id="lista-jogos">
            {jogos.map((jogo) => (
              <div className="col-6 col-md-4 col-xl-3" key={jogo.id}>
                <CardJogo jogo={jogo} />
              </div>
            ))}
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

export default Catalogo