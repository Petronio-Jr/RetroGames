
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import useAutenticacao from '../hooks/useAutenticacao'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    sair,
  } = useAutenticacao()

  const ehCliente = usuario?.perfil === 'Cliente'

  const navegar = useNavigate()

  async function lidarComSaida() {
    try {
      await sair()
    } finally {
      navegar('/')
    }
  }

  return (
    <>
      {/* Barra de navegação */}
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">

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

              {/* Menu exclusivo do cliente */}
              {ehCliente && (
                <>
                  <li className="nav-item">
                    <NavLink
                      className={({ isActive }) =>
                        `nav-link ${isActive ? 'active' : ''}`
                      }
                      to="/carrinho"
                    >
                      Carrinho
                    </NavLink>
                  </li>

                  <li className="nav-item">
                    <NavLink
                      className={({ isActive }) =>
                        `nav-link ${isActive ? 'active' : ''}`
                      }
                      to="/meus-pedidos"
                    >
                      Meus pedidos
                    </NavLink>
                  </li>
                </>
              )}

              {/* Menu exclusivo do administrador */}
              {ehAdministrador && (
                <li className="nav-item dropdown">
                  <button
                    className="nav-link dropdown-toggle btn btn-link"
                    type="button"
                    data-bs-toggle="dropdown"
                    aria-expanded="false"
                  >
                    Administração
                  </button>

                  <ul className="dropdown-menu">
                    <li>
                      <Link className="dropdown-item" to="/admin/jogos">
                        Jogos
                      </Link>
                    </li>

                    <li>
                      <Link className="dropdown-item" to="/admin/plataformas">
                        Plataformas
                      </Link>
                    </li>
                  </ul>
                </li>
              )}
            </ul>

            {/* Área do usuário */}
            <ul className="navbar-nav">
              {!carregando && (
                estaAutenticado ? (
                  <>
                    <li className="nav-item">
                      <span className="navbar-text me-3">
                        Olá, {usuario.nome}
                      </span>
                    </li>

                    <li className="nav-item">
                      <button
                        className="btn btn-outline-light btn-sm"
                        onClick={lidarComSaida}
                      >
                        Sair
                      </button>
                    </li>
                  </>
                ) : (
                  <li className="nav-item">
                    <NavLink className="nav-link" to="/login">
                      Entrar
                    </NavLink>
                  </li>
                )
              )}
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
