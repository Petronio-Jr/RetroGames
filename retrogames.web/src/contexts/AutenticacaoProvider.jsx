import { useCallback, useEffect, useState } from 'react'
import AutenticacaoContext from './autenticacaoContext'
import { autenticacaoApi } from '../api/servicos'

function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  const buscarUsuarioAtual = useCallback(async () => {
    try {
      const dadosUsuario = await autenticacaoApi.usuarioAtual()
      setUsuario(dadosUsuario)
    } catch (erro) {
      setUsuario(null)
    } finally {
      setCarregando(false)
    }
  }, [])

  useEffect(() => {
    buscarUsuarioAtual()

    function tratarSessaoExpirada() {
      setUsuario(null)
      setCarregando(false)
    }

    window.addEventListener('sessao:expirada', tratarSessaoExpirada)

    return () => {
      window.removeEventListener('sessao:expirada', tratarSessaoExpirada)
    }
  }, [buscarUsuarioAtual])

  async function entrar(email, senha) {
    await autenticacaoApi.entrar({ email, senha })
    await buscarUsuarioAtual()
  }

  async function cadastrar(nome, email, senha) {
    await autenticacaoApi.cadastrar({ nome, email, senha })
    await buscarUsuarioAtual()
  }

  async function sair() {
    try {
      await autenticacaoApi.sair()
    } finally {
      setUsuario(null)
    }
  }

  const estaAutenticado = Boolean(usuario)
  const ehAdministrador = usuario?.perfil === 'Administrador'

  const valor = {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    entrar,
    cadastrar,
    sair,
  }

  return (
    <AutenticacaoContext.Provider value={valor}>
      {children}
    </AutenticacaoContext.Provider>
  )
}

export default AutenticacaoProvider