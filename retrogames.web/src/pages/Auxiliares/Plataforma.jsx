import { useEffect, useState } from 'react'

import { plataformaApi } from '../../api/servicos'
import {
  obterMensagemErro,
  obterErrosDosCampos,
} from '../../api/erros'

import {
  Carregando,
  MensagemErro,
  EstadoVazio,
} from '../../components/MensagensEstado'

function Plataforma() {
  const [plataformas, setPlataformas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erroLista, setErroLista] = useState('')

  const [idEmEdicao, setIdEmEdicao] = useState(null)
  const [nome, setNome] = useState('')
  const [fabricante, setFabricante] = useState('')
  const [anoLancamento, setAnoLancamento] = useState('')

  const [errosCampos, setErrosCampos] = useState({})
  const [erroFormulario, setErroFormulario] = useState('')
  const [salvando, setSalvando] = useState(false)

  async function carregarPlataformas() {
    setCarregando(true)
    setErroLista('')

    try {
      const dados = await plataformaApi.listar()
      setPlataformas(dados)
    } catch (erro) {
      setErroLista(obterMensagemErro(erro))
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregarPlataformas()
  }, [])

  function limparFormulario() {
    setIdEmEdicao(null)
    setNome('')
    setFabricante('')
    setAnoLancamento('')
    setErrosCampos({})
    setErroFormulario('')
  }

  function editarPlataforma(plataforma) {
    setIdEmEdicao(plataforma.id)
    setNome(plataforma.nome)
    setFabricante(plataforma.fabricante)
    setAnoLancamento(String(plataforma.anoLancamento))
    setErrosCampos({})
    setErroFormulario('')
  }

  async function lidarComEnvio(evento) {
    evento.preventDefault()

    setErrosCampos({})
    setErroFormulario('')
    setSalvando(true)

    const dados = {
      nome,
      fabricante,
      anoLancamento: Number(anoLancamento),
    }

    try {
      if (idEmEdicao) {
        await plataformaApi.alterar(idEmEdicao, dados)
      } else {
        await plataformaApi.inserir(dados)
      }

      limparFormulario()
      await carregarPlataformas()
    } catch (erro) {
      setErrosCampos(obterErrosDosCampos(erro))
      setErroFormulario(obterMensagemErro(erro))
    } finally {
      setSalvando(false)
    }
  }

  async function excluirPlataforma(plataforma) {
    const confirmou = window.confirm(
      `Deseja realmente excluir a plataforma ${plataforma.nome}?`
    )

    if (!confirmou) {
      return
    }

    setErroFormulario('')

    try {
      await plataformaApi.excluir(plataforma.id)
      await carregarPlataformas()
    } catch (erro) {
      setErroFormulario(obterMensagemErro(erro))
    }
  }

  return (
    <>
      <h2 className="mb-4">Administração de Plataformas</h2>

      <div className="card mb-4">
        <div className="card-body">
          <h3 className="h5 mb-3">
            {idEmEdicao ? 'Alterar plataforma' : 'Nova plataforma'}
          </h3>

          <form onSubmit={lidarComEnvio}>
            <div className="mb-3">
              <label htmlFor="nome" className="form-label">
                Nome
              </label>
              <input
                type="text"
                id="nome"
                className={`form-control ${errosCampos.nome ? 'is-invalid' : ''}`}
                value={nome}
                onChange={(evento) => setNome(evento.target.value)}
              />
              {errosCampos.nome && (
                <div className="invalid-feedback">
                  {errosCampos.nome}
                </div>
              )}
            </div>

            <div className="mb-3">
              <label htmlFor="fabricante" className="form-label">
                Fabricante
              </label>
              <input
                type="text"
                id="fabricante"
                className={`form-control ${errosCampos.fabricante ? 'is-invalid' : ''}`}
                value={fabricante}
                onChange={(evento) => setFabricante(evento.target.value)}
              />
              {errosCampos.fabricante && (
                <div className="invalid-feedback">
                  {errosCampos.fabricante}
                </div>
              )}
            </div>

            <div className="mb-3">
              <label htmlFor="anoLancamento" className="form-label">
                Ano de lançamento
              </label>
              <input
                type="number"
                id="anoLancamento"
                className={`form-control ${errosCampos.anoLancamento ? 'is-invalid' : ''}`}
                value={anoLancamento}
                onChange={(evento) => setAnoLancamento(evento.target.value)}
              />
              {errosCampos.anoLancamento && (
                <div className="invalid-feedback">
                  {errosCampos.anoLancamento}
                </div>
              )}
            </div>

            {erroFormulario && (
              <div className="alert alert-danger" role="alert">
                {erroFormulario}
              </div>
            )}

            <button
              type="submit"
              className="btn btn-primary me-2"
              disabled={salvando}
            >
              {salvando ? 'Salvando...' : 'Salvar'}
            </button>

            {idEmEdicao && (
              <button
                type="button"
                className="btn btn-secondary"
                onClick={limparFormulario}
              >
                Cancelar
              </button>
            )}
          </form>
        </div>
      </div>

      <h3 className="h5 mb-3">Plataformas cadastradas</h3>

      {carregando && <Carregando />}

      {!carregando && erroLista && (
        <MensagemErro
          mensagem={erroLista}
          aoTentarNovamente={carregarPlataformas}
        />
      )}

      {!carregando && !erroLista && plataformas.length === 0 && (
        <EstadoVazio titulo="Nenhuma plataforma cadastrada">
          Cadastre uma plataforma usando o formulário acima.
        </EstadoVazio>
      )}

      {!carregando && !erroLista && plataformas.length > 0 && (
        <div className="table-responsive">
          <table className="table table-striped table-hover tabela-cartoes">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Fabricante</th>
                <th>Ano</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {plataformas.map((plataforma) => (
                <tr key={plataforma.id}>
                  <td data-label="Nome">{plataforma.nome}</td>
                  <td data-label="Fabricante">{plataforma.fabricante}</td>
                  <td data-label="Ano">{plataforma.anoLancamento}</td>
                  <td data-label="Ações">
                    <div className="d-flex gap-2">
                    <button
                      type="button"
                      className="btn btn-sm btn-outline-primary"
                      onClick={() => editarPlataforma(plataforma)}
                    >
                      Editar
                    </button>

                    <button
                      type="button"
                      className="btn btn-sm btn-outline-danger"
                      onClick={() => excluirPlataforma(plataforma)}
                    >
                      Excluir
                    </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  )
}

export default Plataforma
