
import { Routes, Route } from 'react-router-dom'

import Layout from './components/Layout'
import RotaProtegida from './components/RotaProtegida'
import RotaCarrinho from './components/RotaCarrinho'

import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import Login from './pages/Login/Login'
import Cadastro from './pages/Cadastro/Cadastro'
import NaoEncontrada from './pages/NaoEncontrada'

import Jogos from './pages/Admin/Jogos'
import FormularioJogo from './pages/Admin/FormularioJogo'
import Plataforma from './pages/Auxiliares/Plataforma'

import Carrinho from './pages/Carrinho/Carrinho'
import MeusPedidos from './pages/MeusPedidos/MeusPedidos'

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        {/* Rotas públicas */}
        <Route path="/" element={<Catalogo />} />
        <Route path="/jogos/:id" element={<DetalheJogo />} />
        <Route path="/login" element={<Login />} />
        <Route path="/cadastro" element={<Cadastro />} />

        {/* Carrinho: visitante e cliente */}
        <Route path="/carrinho" element={<RotaCarrinho />}>
          <Route index element={<Carrinho />} />
        </Route>

        {/* Rotas do cliente */}
        <Route path="/meus-pedidos" element={<RotaProtegida />}>
          <Route index element={<MeusPedidos />} />
        </Route>

        {/* Rotas administrativas */}
        <Route
          path="/admin"
          element={<RotaProtegida somenteAdministrador />}
        >
          <Route path="jogos" element={<Jogos />} />
          <Route path="jogos/novo" element={<FormularioJogo />} />
          <Route path="jogos/:id/editar" element={<FormularioJogo />} />
          <Route path="plataformas" element={<Plataforma />} />
        </Route>

        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App
