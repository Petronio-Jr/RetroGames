import { useState } from 'react'

export function gerarSlug(nome) {
  return nome
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '')
}

function escolherTom(texto = '') {
  let soma = 0
  for (const letra of texto) soma += letra.charCodeAt(0)
  return soma % 5
}

function CapaJogo({ jogo, className = '' }) {
  const [semFoto, setSemFoto] = useState(false)
  const slug = gerarSlug(jogo.nome)
  const tom = escolherTom(jogo.nomePlataforma)

  return (
    <div className={`capa capa-tom-${tom} ${className}`}>
      {semFoto ? (
        <div className="capa-cartucho" aria-hidden="true">
          <span className="capa-cartucho-plataforma">
            {jogo.nomePlataforma}
          </span>
          <span className="capa-cartucho-nome">{jogo.nome}</span>
        </div>
      ) : (
        <img
          src={`/capas/${slug}.jpg`}
          alt={`Capa do jogo ${jogo.nome}`}
          loading="lazy"
          onError={() => setSemFoto(true)}
        />
      )}
    </div>
  )
}

export default CapaJogo