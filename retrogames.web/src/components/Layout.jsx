
import { useEffect } from 'react'
import { Link, NavLink, Outlet, useLocation, useNavigate } from 'react-router-dom'
import useAutenticacao from '../hooks/useAutenticacao'
import useCarrinho from '../hooks/useCarrinho'
import FundoInterativo from './FundoInterativo'

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
  const { pathname } = useLocation()
  const { itens } = useCarrinho()

  const quantidadeNoCarrinho = itens.reduce(
    (soma, item) => soma + (Number(item.quantidade) || 0),
    0
  )

  // O menu do Bootstrap não sabe que a página mudou: fechamos aqui
  // para ele não ficar aberto por cima do conteúdo no celular.
  useEffect(() => {
    document.getElementById('menuNavegacao')?.classList.remove('show')
  }, [pathname])

  async function lidarComSaida() {
    try {
      await sair()
    } finally {
      navegar('/')
    }
  }

  return (
    <>
      <FundoInterativo />

      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand fw-bold" to="/">
            RetroGames
          </Link>

          {/* Atalho do carrinho: fica sempre visível, inclusive no celular */}
          {!ehAdministrador && (
            <Link
              to="/carrinho"
              className="carrinho-atalho ms-auto me-2 d-lg-none"
              aria-label={`Carrinho, ${quantidadeNoCarrinho} item(ns)`}
            >
              <span aria-hidden="true">🛒</span>

              {quantidadeNoCarrinho > 0 && (
                <span className="carrinho-contador">
                  {quantidadeNoCarrinho}
                </span>
              )}
            </Link>
          )}

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

              {/* Visitantes e clientes podem acessar o carrinho */}
              {!ehAdministrador && (
                <li className="nav-item d-none d-lg-block">
                  <NavLink
                    className={({ isActive }) =>
                      `nav-link ${isActive ? 'active' : ''}`
                    }
                    to="/carrinho"
                  >
                    Carrinho
                    {quantidadeNoCarrinho > 0 && (
                      <span className="carrinho-contador ms-2">
                        {quantidadeNoCarrinho}
                      </span>
                    )}
                  </NavLink>
                </li>
              )}

              {/* Área exclusiva do cliente */}
              {ehCliente && (
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
              )}

              {/* Área exclusiva do administrador */}
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
                        className="btn btn-outline-light btn-sm botao-sair"
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

      <main className="container py-4">
        <Outlet />
      </main>

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
