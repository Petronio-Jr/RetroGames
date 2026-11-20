function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) {
    return null
  }

  const paginas = Array.from(
    { length: totalPaginas },
    (_, indice) => indice + 1
  )

  return (
    <nav aria-label="Navegação do catálogo">
      <ul className="pagination justify-content-center mt-4">
        <li className={`page-item ${pagina === 1 ? 'disabled' : ''}`}>
          <button
            className="page-link"
            onClick={() => aoMudar(pagina - 1)}
            disabled={pagina === 1}
          >
            Anterior
          </button>
        </li>

        {paginas.map((numero) => (
          <li
            key={numero}
            className={`page-item ${pagina === numero ? 'active' : ''}`}
          >
            <button
              className="page-link"
              onClick={() => aoMudar(numero)}
            >
              {numero}
            </button>
          </li>
        ))}

        <li
          className={`page-item ${
            pagina === totalPaginas ? 'disabled' : ''
          }`}
        >
          <button
            className="page-link"
            onClick={() => aoMudar(pagina + 1)}
            disabled={pagina === totalPaginas}
          >
            Próxima
          </button>
        </li>
      </ul>
    </nav>
  )
}

export default Paginacao