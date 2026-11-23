
import { Link, NavLink, Outlet } from 'react-router-dom'

function Layout() {
  return (
    <>
      {/* Barra de navegação */}
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">

          {/* Marca */}
          <Link className="navbar-brand fw-bold" to="/">
            RetroGames
          </Link>

          {/* Botão do menu para telas menores */}
          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#menuNavegacao"
            aria-controls="menuNavegacao"
            aria-expanded="false"
            aria-label="Abrir menu de navegação"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          {/* Itens da navegação */}
          <div className="collapse navbar-collapse" id="menuNavegacao">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                <NavLink
                  className={({ isActive }) =>
                    `nav-link ${isActive ? 'active' : ''}`
                  }
                  to="/"
                  end
                >
                  Catálogo
                </NavLink>
              </li>
            </ul>
          </div>

        </div>
      </nav>

      {/* Conteúdo da página */}
      <main className="container py-4">
        <Outlet />
      </main>

      {/* Rodapé */}
      <footer className="bg-dark text-light py-3 mt-auto">
        <div className="container text-center">
          <p className="mb-0">
            RetroGames - Catálogo de jogos retrô
          </p>
        </div>
      </footer>
    </>
  )
}

export default Layout
