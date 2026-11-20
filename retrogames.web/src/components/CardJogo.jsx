import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

function CardJogo({ jogo }) {
  const esgotado = jogo.estoque <= 0

  return (
    <div className="card h-100 shadow-sm">
      <div className="card-body d-flex flex-column">
        <span className="badge text-bg-secondary align-self-start mb-2">
          {jogo.nomePlataforma}
        </span>

        <h5 className="card-title">{jogo.nome}</h5>

        <p className="text-muted mb-2">
          Ano: {jogo.ano}
        </p>

        <p className="mb-2">
          Condição: {jogo.descricaoCondicao}
        </p>

        <p className="mb-2">
          Gêneros: {jogo.generos.join(', ')}
        </p>

        <h5 className="text-success mt-2">
          {formatarPreco(jogo.preco)}
        </h5>

        {esgotado ? (
          <span className="badge text-bg-danger align-self-start mt-2">
            Esgotado
          </span>
        ) : (
          <span className="badge text-bg-success align-self-start mt-2">
            Em estoque: {jogo.estoque}
          </span>
        )}

        <Link
          to={`/jogos/${jogo.id}`}
          className="btn btn-primary mt-auto mt-3"
        >
          Ver detalhes
        </Link>
      </div>
    </div>
  )
}

export default CardJogo