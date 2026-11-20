import { Link } from 'react-router-dom'

export default function NaoEncontrada() {
  return (
    <div className="text-center py-5">
      <h1>404 - Página não encontrada</h1>

      <p className="lead">
        A página que você está procurando não existe.
      </p>

      <Link to="/" className="btn btn-primary">
        Voltar para o catálogo
      </Link>
    </div>
  )
}