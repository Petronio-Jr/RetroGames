import { useEffect, useRef } from 'react'

// Fundo retrô em canvas:
//  - grade de pontos que acende perto do mouse
//  - sprites de 8 bits que flutuam e fogem do ponteiro
//  - clique solta uma explosão de pixels
// Tudo roda atrás do site (pointer-events: none), então não atrapalha cliques.

const CORES = ['#d02a35', '#f7c948', '#39c46a', '#3a6fe0', '#ffb000']

// Desenhos de 7x7 pixels ('#' = pixel aceso)
const SPRITES = [
  ['.##.##.', '#######', '#######', '#######', '.#####.', '..###..', '...#...'], // coração
  ['...#...', '...#...', '#######', '.#####.', '..###..', '.##.##.', '.#...#.'], // estrela
  ['..###..', '..###..', '#######', '#######', '#######', '..###..', '..###..'], // direcional
  ['...#...', '..###..', '.#####.', '#######', '.#####.', '..###..', '...#...'], // diamante
  ['..###..', '.#...#.', '#.....#', '#.....#', '#.....#', '.#...#.', '..###..'], // anel
]

const ESPACO_GRADE = 34
const RAIO_MOUSE = 150

function FundoInterativo() {
  const canvasRef = useRef(null)

  useEffect(() => {
    const canvas = canvasRef.current
    const ctx = canvas.getContext('2d')
    const reduzirMovimento = window.matchMedia('(prefers-reduced-motion: reduce)').matches

    let largura = 0
    let altura = 0
    let sprites = []
    let particulas = []
    let idQuadro = 0
    const mouse = { x: -9999, y: -9999 }

    function criarSprite(aleatorioY = true) {
      const escala = 3 + Math.floor(Math.random() * 3) // 3 a 5 px por pixel do desenho
      return {
        forma: SPRITES[Math.floor(Math.random() * SPRITES.length)],
        cor: CORES[Math.floor(Math.random() * CORES.length)],
        escala,
        x: Math.random() * largura,
        y: aleatorioY ? Math.random() * altura : altura + 40,
        baseVx: (Math.random() - 0.5) * 0.25,
        baseVy: -(0.12 + Math.random() * 0.3), // sobem devagar
        vx: 0,
        vy: 0,
      }
    }

    function ajustarTamanho() {
      const dpr = Math.min(window.devicePixelRatio || 1, 2)
      largura = window.innerWidth
      altura = window.innerHeight
      canvas.width = largura * dpr
      canvas.height = altura * dpr
      ctx.setTransform(dpr, 0, 0, dpr, 0, 0)

      // menos sprites em telas pequenas
      const quantidade = Math.max(10, Math.min(34, Math.round((largura * altura) / 52000)))
      sprites = Array.from({ length: quantidade }, () => criarSprite(true))

      if (reduzirMovimento) desenhar()
    }

    function desenharSprite(s) {
      ctx.fillStyle = s.cor
      const x0 = Math.round(s.x)
      const y0 = Math.round(s.y)
      s.forma.forEach((linha, r) => {
        for (let c = 0; c < linha.length; c++) {
          if (linha[c] === '#') {
            ctx.fillRect(x0 + c * s.escala, y0 + r * s.escala, s.escala, s.escala)
          }
        }
      })
    }

    function desenhar() {
      ctx.clearRect(0, 0, largura, altura)

      // brilho âmbar que segue o mouse
      if (mouse.x > -1000) {
        const brilho = ctx.createRadialGradient(mouse.x, mouse.y, 0, mouse.x, mouse.y, 260)
        brilho.addColorStop(0, 'rgba(255, 176, 0, 0.16)')
        brilho.addColorStop(1, 'rgba(255, 176, 0, 0)')
        ctx.fillStyle = brilho
        ctx.fillRect(0, 0, largura, altura)
      }

      // grade de pontos; os próximos do mouse acendem e crescem
      for (let x = ESPACO_GRADE / 2; x < largura; x += ESPACO_GRADE) {
        for (let y = ESPACO_GRADE / 2; y < altura; y += ESPACO_GRADE) {
          const dx = x - mouse.x
          const dy = y - mouse.y
          const distancia = Math.sqrt(dx * dx + dy * dy)

          if (distancia < RAIO_MOUSE) {
            const forca = 1 - distancia / RAIO_MOUSE
            const lado = 2 + Math.round(forca * 4)
            ctx.fillStyle = `rgba(255, 176, 0, ${0.25 + forca * 0.75})`
            ctx.fillRect(Math.round(x - lado / 2), Math.round(y - lado / 2), lado, lado)
          } else {
            ctx.fillStyle = 'rgba(179, 169, 204, 0.16)'
            ctx.fillRect(x, y, 2, 2)
          }
        }
      }

      sprites.forEach(desenharSprite)

      particulas.forEach((p) => {
        ctx.globalAlpha = Math.max(p.vida, 0)
        ctx.fillStyle = p.cor
        ctx.fillRect(Math.round(p.x), Math.round(p.y), p.tamanho, p.tamanho)
      })
      ctx.globalAlpha = 1
    }

    function atualizar() {
      sprites.forEach((s) => {
        const dx = s.x - mouse.x
        const dy = s.y - mouse.y
        const distancia = Math.sqrt(dx * dx + dy * dy)

        // foge do mouse
        if (distancia < RAIO_MOUSE && distancia > 0) {
          const forca = (1 - distancia / RAIO_MOUSE) * 1.6
          s.vx += (dx / distancia) * forca
          s.vy += (dy / distancia) * forca
        }

        // volta aos poucos para a velocidade de flutuação
        s.vx += (s.baseVx - s.vx) * 0.04
        s.vy += (s.baseVy - s.vy) * 0.04

        s.x += s.vx
        s.y += s.vy

        // saiu da tela: reaparece do outro lado
        if (s.y < -50) Object.assign(s, criarSprite(false), { x: Math.random() * largura })
        if (s.x < -50) s.x = largura + 40
        if (s.x > largura + 50) s.x = -40
      })

      particulas.forEach((p) => {
        p.vy += 0.18 // gravidade
        p.x += p.vx
        p.y += p.vy
        p.vida -= 0.02
      })
      particulas = particulas.filter((p) => p.vida > 0)
    }

    function quadro() {
      atualizar()
      desenhar()
      idQuadro = requestAnimationFrame(quadro)
    }

    function explodir(x, y) {
      for (let i = 0; i < 18; i++) {
        const angulo = Math.random() * Math.PI * 2
        const velocidade = 1.5 + Math.random() * 4
        particulas.push({
          x,
          y,
          vx: Math.cos(angulo) * velocidade,
          vy: Math.sin(angulo) * velocidade - 1.5,
          tamanho: 4 + Math.floor(Math.random() * 3) * 2,
          cor: CORES[Math.floor(Math.random() * CORES.length)],
          vida: 1,
        })
      }
    }

    function aoMoverMouse(evento) {
      mouse.x = evento.clientX
      mouse.y = evento.clientY
      if (reduzirMovimento) desenhar()
    }

    function aoSairDaJanela() {
      mouse.x = -9999
      mouse.y = -9999
    }

    function aoClicar(evento) {
      if (!reduzirMovimento) explodir(evento.clientX, evento.clientY)
    }

    // pausa quando a aba está escondida (economiza bateria)
    function aoMudarVisibilidade() {
      cancelAnimationFrame(idQuadro)
      if (!document.hidden && !reduzirMovimento) idQuadro = requestAnimationFrame(quadro)
    }

    ajustarTamanho()
    if (!reduzirMovimento) idQuadro = requestAnimationFrame(quadro)

    window.addEventListener('resize', ajustarTamanho)
    window.addEventListener('pointermove', aoMoverMouse)
    window.addEventListener('pointerdown', aoClicar)
    document.addEventListener('mouseleave', aoSairDaJanela)
    document.addEventListener('visibilitychange', aoMudarVisibilidade)

    return () => {
      cancelAnimationFrame(idQuadro)
      window.removeEventListener('resize', ajustarTamanho)
      window.removeEventListener('pointermove', aoMoverMouse)
      window.removeEventListener('pointerdown', aoClicar)
      document.removeEventListener('mouseleave', aoSairDaJanela)
      document.removeEventListener('visibilitychange', aoMudarVisibilidade)
    }
  }, [])

  return <canvas ref={canvasRef} className="fundo-interativo" aria-hidden="true" />
}

export default FundoInterativo
