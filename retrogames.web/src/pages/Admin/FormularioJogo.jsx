import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'

import {
  jogoApi,
  plataformaApi,
  generoApi,
  condicaoJogoApi,
} from '../../api/servicos'

import {
  obterMensagemErro,
  obterErrosDosCampos,
} from '../../api/erros'

import {
  Carregando,
} from '../../components/MensagensEstado'

function FormularioJogo() {
  const { id } = useParams()
  const navigate = useNavigate()

  const estaEditando = Boolean(id)

  const [nome, setNome] = useState('')
  const [descricao, setDescricao] = useState('')
  const [anoLancamento, setAnoLancamento] = useState('')
  const [preco, setPreco] = useState('')
  const [estoque, setEstoque] = useState('')
  const [idPlataforma, setIdPlataforma] = useState('')
  const [idCondicaoJogo, setIdCondicaoJogo] = useState('')
  const [idsGeneros, setIdsGeneros] = useState([])
  const [ativo, setAtivo] = useState(true)

  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [condicoes, setCondicoes] = useState([])

  const [carregando, setCarregando] = useState(true)
  const [salvando, setSalvando] = useState(false)

  const [errosCampos, setErrosCampos] = useState({})
  const [erroFormulario, setErroFormulario] = useState('')

  useEffect(() => {
    async function carregarDados() {
      setCarregando(true)
      setErroFormulario('')

      try {
        const [
          listaPlataformas,
          listaGeneros,
          listaCondicoes,
        ] = await Promise.all([
          plataformaApi.listar(),
          generoApi.listar(),
          condicaoJogoApi.listar(),
        ])

        setPlataformas(listaPlataformas)
        setGeneros(listaGeneros)
        setCondicoes(listaCondicoes)

        if (estaEditando) {
          const jogo = await jogoApi.buscarPorId(id)

          setNome(jogo.nome)
          setDescricao(jogo.descricao || '')
          setAnoLancamento(String(jogo.ano))
          setPreco(String(jogo.preco))
          setEstoque(String(jogo.estoque))
          setIdPlataforma(String(jogo.idPlataforma))
          setIdCondicaoJogo(String(jogo.idCondicaoJogo))
          setAtivo(jogo.ativo)

          setIdsGeneros(
            jogo.generos.map((genero) => String(genero.id))
          )
        }
      } catch (erro) {
        setErroFormulario(obterMensagemErro(erro))
      } finally {
        setCarregando(false)
      }
    }

    carregarDados()
  }, [id, estaEditando])

  function alternarGenero(idGenero) {
    setIdsGeneros((selecionados) => {
      if (selecionados.includes(idGenero)) {
        return selecionados.filter((id) => id !== idGenero)
      }

      return [...selecionados, idGenero]
    })
  }

  async function salvarJogo(evento) {
    evento.preventDefault()

    setErrosCampos({})
    setErroFormulario('')
    setSalvando(true)

    const dados = {
      nome,
      descricao: descricao.trim() || null,
      anoLancamento: Number(anoLancamento),
      preco: Number(preco),
      estoque: Number(estoque),
      idPlataforma: Number(idPlataforma),
      idCondicaoJogo: Number(idCondicaoJogo),
      idsGeneros: idsGeneros.map((idGenero) => Number(idGenero)),
    }

    try {
      if (estaEditando) {
        await jogoApi.alterar(id, {
          ...dados,
          ativo,
        })
      } else {
        await jogoApi.inserir(dados)
      }

      navigate('/admin/jogos')
    } catch (erro) {
      setErrosCampos(obterErrosDosCampos(erro))
      setErroFormulario(obterMensagemErro(erro))
    } finally {
      setSalvando(false)
    }
  }

  if (carregando) {
    return <Carregando />
  }

  return (
    <div>
      <div className="mb-4">
        <Link
          to="/admin/jogos"
          className="btn btn-outline-secondary mb-3"
        >
          Voltar para jogos
        </Link>

        <h1 className="h2">
          {estaEditando ? 'Alterar jogo' : 'Novo jogo'}
        </h1>

        <p className="text-muted">
          Preencha os dados do jogo abaixo.
        </p>
      </div>

      {erroFormulario && (
        <div className="alert alert-danger" role="alert">
          {erroFormulario}
        </div>
      )}

      <form onSubmit={salvarJogo}>
        <div className="card mb-4">
          <div className="card-body">
            <h2 className="h5 mb-3">Informações do jogo</h2>

            <div className="mb-3">
              <label htmlFor="nome" className="form-label">
                Nome
              </label>

              <input
                id="nome"
                type="text"
                className={`form-control ${errosCampos.Nome || errosCampos.nome ? 'is-invalid' : ''}`}
                value={nome}
                onChange={(evento) => setNome(evento.target.value)}
              />

              {(errosCampos.Nome || errosCampos.nome) && (
                <div className="invalid-feedback">
                  {errosCampos.Nome || errosCampos.nome}
                </div>
              )}
            </div>

            <div className="mb-3">
              <label htmlFor="descricao" className="form-label">
                Descrição
              </label>

              <textarea
                id="descricao"
                className={`form-control ${errosCampos.Descricao || errosCampos.descricao ? 'is-invalid' : ''}`}
                rows="4"
                value={descricao}
                onChange={(evento) => setDescricao(evento.target.value)}
              />

              {(errosCampos.Descricao || errosCampos.descricao) && (
                <div className="invalid-feedback">
                  {errosCampos.Descricao || errosCampos.descricao}
                </div>
              )}
            </div>

            <div className="row">
              <div className="col-md-4 mb-3">
                <label htmlFor="anoLancamento" className="form-label">
                  Ano de lançamento
                </label>

                <input
                  id="anoLancamento"
                  type="number"
                  inputMode="numeric"
                  className={`form-control ${errosCampos.AnoLancamento || errosCampos.anoLancamento ? 'is-invalid' : ''}`}
                  value={anoLancamento}
                  onChange={(evento) => setAnoLancamento(evento.target.value)}
                />

                {(errosCampos.AnoLancamento || errosCampos.anoLancamento) && (
                  <div className="invalid-feedback">
                    {errosCampos.AnoLancamento || errosCampos.anoLancamento}
                  </div>
                )}
              </div>

              <div className="col-md-4 mb-3">
                <label htmlFor="preco" className="form-label">
                  Preço
                </label>

                <input
                  id="preco"
                  type="number"
                  inputMode="decimal"
                  step="0.01"
                  className={`form-control ${errosCampos.Preco || errosCampos.preco ? 'is-invalid' : ''}`}
                  value={preco}
                  onChange={(evento) => setPreco(evento.target.value)}
                />

                {(errosCampos.Preco || errosCampos.preco) && (
                  <div className="invalid-feedback">
                    {errosCampos.Preco || errosCampos.preco}
                  </div>
                )}
              </div>

              <div className="col-md-4 mb-3">
                <label htmlFor="estoque" className="form-label">
                  Estoque
                </label>

                <input
                  id="estoque"
                  type="number"
                  inputMode="numeric"
                  className={`form-control ${errosCampos.Estoque || errosCampos.estoque ? 'is-invalid' : ''}`}
                  value={estoque}
                  onChange={(evento) => setEstoque(evento.target.value)}
                />

                {(errosCampos.Estoque || errosCampos.estoque) && (
                  <div className="invalid-feedback">
                    {errosCampos.Estoque || errosCampos.estoque}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>

        <div className="card mb-4">
          <div className="card-body">
            <h2 className="h5 mb-3">Classificação</h2>

            <div className="mb-3">
              <label htmlFor="plataforma" className="form-label">
                Plataforma
              </label>

              <select
                id="plataforma"
                className={`form-select ${errosCampos.IdPlataforma || errosCampos.idPlataforma ? 'is-invalid' : ''}`}
                value={idPlataforma}
                onChange={(evento) => setIdPlataforma(evento.target.value)}
              >
                <option value="">Selecione uma plataforma</option>

                {plataformas.map((plataforma) => (
                  <option key={plataforma.id} value={plataforma.id}>
                    {plataforma.nome}
                  </option>
                ))}
              </select>

              {(errosCampos.IdPlataforma || errosCampos.idPlataforma) && (
                <div className="invalid-feedback">
                  {errosCampos.IdPlataforma || errosCampos.idPlataforma}
                </div>
              )}
            </div>

            <div className="mb-3">
              <label htmlFor="condicao" className="form-label">
                Condição do jogo
              </label>

              <select
                id="condicao"
                className={`form-select ${errosCampos.IdCondicaoJogo || errosCampos.idCondicaoJogo ? 'is-invalid' : ''}`}
                value={idCondicaoJogo}
                onChange={(evento) => setIdCondicaoJogo(evento.target.value)}
              >
                <option value="">Selecione uma condição</option>

                {condicoes.map((condicao) => (
                  <option key={condicao.id} value={condicao.id}>
                    {condicao.descricao}
                  </option>
                ))}
              </select>

              {(errosCampos.IdCondicaoJogo || errosCampos.idCondicaoJogo) && (
                <div className="invalid-feedback">
                  {errosCampos.IdCondicaoJogo || errosCampos.idCondicaoJogo}
                </div>
              )}
            </div>

            <div className="mb-3">
              <label className="form-label d-block">
                Gêneros
              </label>

              {generos.length === 0 ? (
                <p className="text-muted">
                  Nenhum gênero cadastrado.
                </p>
              ) : (
                <div className="row">
                  {generos.map((genero) => (
                    <div className="col-sm-6 col-md-4 mb-2" key={genero.id}>
                      <div className="form-check">
                        <input
                          id={`genero-${genero.id}`}
                          type="checkbox"
                          className="form-check-input"
                          checked={idsGeneros.includes(String(genero.id))}
                          onChange={() => alternarGenero(String(genero.id))}
                        />

                        <label
                          htmlFor={`genero-${genero.id}`}
                          className="form-check-label"
                        >
                          {genero.nome}
                        </label>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </div>

        {estaEditando && (
          <div className="form-check form-switch mb-4">
            <input
              id="ativo"
              type="checkbox"
              className="form-check-input"
              checked={ativo}
              onChange={(evento) => setAtivo(evento.target.checked)}
            />

            <label htmlFor="ativo" className="form-check-label">
              Jogo ativo
            </label>
          </div>
        )}

        <div className="d-flex gap-2 mb-4">
          <button
            type="submit"
            className="btn btn-primary"
            disabled={salvando}
          >
            {salvando ? 'Salvando...' : 'Salvar jogo'}
          </button>

          <Link to="/admin/jogos" className="btn btn-outline-secondary">
            Cancelar
          </Link>
        </div>
      </form>
    </div>
  )
}

export default FormularioJogo
