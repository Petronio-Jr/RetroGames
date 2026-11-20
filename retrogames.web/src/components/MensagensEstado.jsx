export function Carregando() {
  return (
    <div className="text-center py-4" role="status">
      <div className="spinner-border text-primary" aria-hidden="true"></div>
      <p className="mt-2">Carregando...</p>
    </div>
  )
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger" role="alert">
      <p>{mensagem}</p>

      <button
        type="button"
        className="btn btn-outline-danger"
        onClick={aoTentarNovamente}
      >
        Tentar novamente
      </button>
    </div>
  )
}

export function EstadoVazio({ titulo, children }) {
  return (
    <div className="alert alert-secondary text-center">
      <h2 className="h5">{titulo}</h2>
      {children}
    </div>
  )
}