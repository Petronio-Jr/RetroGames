import { Routes, Route } from 'react-router-dom'

import Layout from './components/Layout'
import Catalogo from './pages/Catalogo'
import DetalheJogo from './pages/DetalheJogo'
import NaoEncontrada from './pages/NaoEncontrada'

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<Catalogo />} />
        <Route path="/jogos/:id" element={<DetalheJogo />} />
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}