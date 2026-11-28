import { Navigate, Outlet } from 'react-router-dom'
import useAutenticacao from '../hooks/useAutenticacao'

function RotaCarrinho() {
  const { carregando, ehAdministrador } = useAutenticacao()

  if (carregando) {
    return <p>Verificando sessão...</p>
  }

  if (ehAdministrador) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}

export default RotaCarrinho
