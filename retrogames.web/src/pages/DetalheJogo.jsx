import { useParams } from 'react-router-dom'

export default function DetalheJogo() {
  const { id } = useParams()

  return (
    <section>
      <h1>Detalhes do jogo</h1>
      <p>ID do jogo: {id}</p>
    </section>
  )
}