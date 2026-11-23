import { Routes, Route } from 'react-router-dom'

import Layout from './components/Layout'
import RotaProtegida from './components/RotaProtegida'

import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import Login from './pages/Login/Login'
import NaoEncontrada from './pages/NaoEncontrada'

import Jogos from './pages/Admin/Jogos'
import FormularioJogo from './pages/Admin/FormularioJogo'
import Plataformas from './pages/Admin/Plataformas'

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        {/* Rotas públicas */}
        <Route path="/" element={<Catalogo />} />
        <Route path="/jogos/:id" element={<DetalheJogo />} />
        <Route path="/login" element={<Login />} />

        {/* Rotas administrativas */}
        <Route
          path="/admin"
          element={<RotaProtegida somenteAdministrador />}
        >
          <Route path="jogos" element={<Jogos />} />
          <Route path="jogos/novo" element={<FormularioJogo />} />
          <Route path="jogos/:id/editar" element={<FormularioJogo />} />
          <Route path="plataformas" element={<Plataformas />} />
        </Route>

        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
