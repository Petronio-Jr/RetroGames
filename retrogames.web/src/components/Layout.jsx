
import { Link, NavLink, Outlet } from 'react-router-dom'

export default function Layout() {
  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand fw-bold" to="/">
            RetroGames
          </Link>

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

          <div className="collapse navbar-collapse" id="menuNavegacao">
            <div className="navbar-nav">
              <NavLink
                className={({ isActive }) =>
                  `nav-link ${isActive ? 'active' : ''}`
                }
                to="/"
                end
              >
                Catálogo
              </NavLink>
            </div>
          </div>
        </div>
      </nav>

      <main className="container py-4">
        <Outlet />
      </main>

      <footer className="bg-dark text-light text-center py-3 mt-auto">
        <div className="container">
          <p className="mb-0">
            RetroGames - Catálogo de jogos retrô
          </p>
        </div>
      </footer>
    </>
  )
}
