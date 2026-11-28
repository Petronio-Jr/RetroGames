// Cartucho RetroGames: identidade visual criada somente com cores e iluminação neon.
// Não utiliza imagens, símbolos, ícones, triângulos ou ornamentos geométricos.

export function gerarSlug(nome = '') {
  return nome
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '')
}

// Cada jogo possui uma cor neon principal e outra complementar.
// Jogos não cadastrados recebem a paleta padrão Synthwave.
const JOGOS = {
  'pac-man': { cor: '#ffe34f', secundaria: '#ff4fd8' },
  'space-invaders': { cor: '#62ff9b', secundaria: '#3ee6ff' },
  pitfall: { cor: '#a6ff65', secundaria: '#ffb347' },
  'super-mario-bros': { cor: '#ff4b5c', secundaria: '#ffd23f' },
  'the-legend-of-zelda': { cor: '#ffd75e', secundaria: '#5ff0a0' },
  metroid: { cor: '#ff754d', secundaria: '#ffd23f' },
  'mega-man-2': { cor: '#3ee6ff', secundaria: '#8a4dff' },
  'alex-kidd-in-miracle-world': { cor: '#ffad4d', secundaria: '#ff5fa2' },
  'wonder-boy-iii-the-dragon-s-trap': { cor: '#ff805c', secundaria: '#b77aff' },
  'phantasy-star': { cor: '#65dfff', secundaria: '#e879ff' },
  'sonic-the-hedgehog': { cor: '#3ee6ff', secundaria: '#ffd23f' },
  'streets-of-rage-2': { cor: '#ff5c9a', secundaria: '#3ee6ff' },
  'golden-axe': { cor: '#ffd15c', secundaria: '#ff805c' },
  'shinobi-iii-return-of-the-ninja-master': { cor: '#ff4f70', secundaria: '#65dfff' },
  'super-mario-world': { cor: '#5ff0a0', secundaria: '#ffd23f' },
  'the-legend-of-zelda-a-link-to-the-past': { cor: '#7dff9b', secundaria: '#ffd75e' },
  'street-fighter-ii': { cor: '#ff5c5c', secundaria: '#ffd23f' },
  'f-zero': { cor: '#3ee6ff', secundaria: '#ff4fd8' },
  'crash-bandicoot': { cor: '#ff9b45', secundaria: '#5ff0a0' },
  'gran-turismo': { cor: '#8aa7ff', secundaria: '#ff5c9a' },
}

function CapaJogo({ jogo, className = '' }) {
  const nome = jogo?.nome || 'Jogo retrô'
  const slug = gerarSlug(nome)
  const tema = JOGOS[slug] || {
    cor: '#55eaff',
    secundaria: '#ff4fd8',
  }

  const estilo = {
    '--capa-destaque': tema.cor,
    '--capa-secundaria': tema.secundaria,
  }

  return (
    <div
      className={`capa capa-neon capa-jogo-${slug} ${className}`.trim()}
      style={estilo}
      role="img"
      aria-label={`Cartucho de ${nome}`}
    >
      <div className="capa-cartucho">
        <span className="capa-cartucho-plataforma">
          {jogo?.nomePlataforma || 'RETRO GAMES'}
        </span>

        <div className="capa-cartucho-arte" aria-hidden="true" />

        <span className="capa-cartucho-nome">{nome}</span>
      </div>
    </div>
  )
}

export default CapaJogo
