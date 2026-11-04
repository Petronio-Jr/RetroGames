import { Link, Route, Routes } from 'react-router';

function Inicio() {
  return (
    <main className="container py-5">
      <h1 className="mb-3">RetroGames</h1>
      <p className="lead">Catálogo de jogos retrô.</p>
      <p>O front-end está configurado e pronto para receber as páginas do catálogo.</p>
      <Link className="btn btn-primary" to="/">Ir para o catálogo</Link>
    </main>
  );
}

function NaoEncontrada() {
  return (
    <main className="container py-5">
      <h1>Página não encontrada</h1>
      <Link to="/">Voltar ao catálogo</Link>
    </main>
  );
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Inicio />} />
      <Route path="*" element={<NaoEncontrada />} />
    </Routes>
  );
}
