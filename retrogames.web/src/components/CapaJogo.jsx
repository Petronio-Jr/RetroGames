
function escolherTom(texto = '') {
  let soma = 0

  for (const letra of texto) {
    soma += letra.charCodeAt(0)
  }

  return soma % 5
}

function CapaJogo({ jogo, className = '' }) {
  const tom = escolherTom(jogo.nomePlataforma)

  return (
    <div className={`capa capa-tom-${tom} ${className}`}>
      <div className="capa-cartucho">
        <span className="capa-cartucho-plataforma">
          {jogo.nomePlataforma}
        </span>

        <span className="capa-cartucho-nome">
          {jogo.nome}
        </span>
      </div>
    </div>
  )
}

export default CapaJogo
