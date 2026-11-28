// Seção de abertura do catálogo (título pixelado, joystick, portátil e formas flutuantes).
// Tudo aqui é decorativo e desenhado em SVG/CSS, sem imagens externas.

function HeroCatalogo() {
  function irParaOsJogos(evento) {
    evento.preventDefault()
    document.getElementById('jogos')?.scrollIntoView({ behavior: 'smooth', block: 'start' })
  }

  return (
    <section className="hero" aria-labelledby="titulo-hero">
      {/* Formas flutuantes */}
      <span className="hero-forma hero-estrela hero-estrela-1" aria-hidden="true" />
      <span className="hero-forma hero-estrela hero-estrela-2" aria-hidden="true" />
      <span className="hero-forma hero-anel hero-anel-1" aria-hidden="true" />
      <span className="hero-forma hero-anel hero-anel-2" aria-hidden="true" />
      <span className="hero-forma hero-tetris-s" aria-hidden="true" />
      <span className="hero-forma hero-tetris-t" aria-hidden="true" />

      {/* Joystick */}
      <svg className="hero-forma hero-joystick" viewBox="0 0 120 150" aria-hidden="true">
        <defs>
          <radialGradient id="joyBola" cx="0.35" cy="0.3" r="0.8">
            <stop offset="0" stopColor="#ff9bf0" />
            <stop offset="1" stopColor="#7b2cff" />
          </radialGradient>
          <linearGradient id="joyBase" x1="0" y1="0" x2="0" y2="1">
            <stop offset="0" stopColor="#6a3fd0" />
            <stop offset="1" stopColor="#2b1368" />
          </linearGradient>
        </defs>
        <ellipse cx="60" cy="132" rx="48" ry="12" fill="#12073a" opacity="0.7" />
        <path d="M16 112 Q16 98 60 98 Q104 98 104 112 L104 124 Q104 138 60 138 Q16 138 16 124Z" fill="url(#joyBase)" />
        <ellipse cx="60" cy="112" rx="44" ry="13" fill="#8a5cf0" />
        <path d="M54 102 L72 38 L83 42 L68 104Z" fill="#cdbcff" />
        <circle cx="80" cy="28" r="21" fill="url(#joyBola)" />
        <ellipse cx="73" cy="20" rx="6" ry="4" fill="#fff" opacity="0.45" />
      </svg>

      {/* Portátil */}
      <svg className="hero-forma hero-portatil" viewBox="0 0 240 110" aria-hidden="true">
        <defs>
          <clipPath id="portatilTela">
            <rect x="86" y="22" width="68" height="66" rx="5" />
          </clipPath>
        </defs>
        <rect x="2" y="8" width="236" height="94" rx="47" fill="rgba(165,140,240,0.5)" stroke="rgba(255,255,255,0.35)" strokeWidth="2" />
        <rect x="82" y="18" width="76" height="74" rx="8" fill="#2a1070" stroke="#cbb9ff" strokeWidth="3" />
        <g clipPath="url(#portatilTela)">
          <rect x="86" y="22" width="68" height="66" fill="#1a0a52" />
          <path d="M84 92 L112 18 L124 18 L96 92Z" fill="#7a3df0" />
          <path d="M112 92 L140 18 L152 18 L124 92Z" fill="#7a3df0" />
        </g>
        <path d="M38 42h10v8h8v10h-8v8H38v-8h-8V50h8z" fill="#3ee6ff" />
        <circle cx="196" cy="46" r="5" fill="#3ee6ff" />
        <circle cx="184" cy="57" r="5" fill="#3ee6ff" />
        <circle cx="208" cy="57" r="5" fill="#3ee6ff" />
        <circle cx="196" cy="68" r="5" fill="#3ee6ff" />
      </svg>

      <div className="hero-conteudo">
        <h1 id="titulo-hero" className="hero-titulo">
          <span>RETRO</span>
          <span>GAMES</span>
        </h1>

        <p className="hero-texto">Explore nossa coleção de jogos retrô.</p>

        <a href="#jogos" className="hero-botao" onClick={irParaOsJogos}>
          Comprar agora
        </a>
      </div>
    </section>
  )
}

export default HeroCatalogo
