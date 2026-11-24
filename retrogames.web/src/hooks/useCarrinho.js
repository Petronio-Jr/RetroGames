import { useContext } from 'react'
import CarrinhoContext from '../contexts/carrinhoContext'

function useCarrinho() {
  const contexto = useContext(CarrinhoContext)

  if (!contexto) {
    throw new Error(
      'useCarrinho deve ser utilizado dentro de CarrinhoProvider.'
    )
  }

  return contexto
}

export default useCarrinho