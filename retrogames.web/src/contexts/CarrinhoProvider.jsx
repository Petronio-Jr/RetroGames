import { useEffect, useState } from 'react'
import CarrinhoContext from './carrinhoContext'

const CHAVE_CARRINHO = 'retrogames-carrinho'

function obterCarrinhoSalvo() {
  try {
    const dados = localStorage.getItem(CHAVE_CARRINHO)
    return dados ? JSON.parse(dados) : []
  } catch {
    return []
  }
}

function CarrinhoProvider({ children }) {
  const [itens, setItens] = useState(obterCarrinhoSalvo)

  // Salva o carrinho sempre que os itens mudarem.
  useEffect(() => {
    localStorage.setItem(CHAVE_CARRINHO, JSON.stringify(itens))
  }, [itens])

  function adicionarItem(jogo) {
    if (jogo.estoque <= 0) {
      return false
    }

    const itemExistente = itens.find(
      item => item.id === jogo.id
    )

    if (itemExistente) {
      if (
        itemExistente.quantidade >= 10 ||
        itemExistente.quantidade >= jogo.estoque
      ) {
        return false
      }

      setItens(itens.map(item =>
        item.id === jogo.id
          ? { ...item, quantidade: item.quantidade + 1 }
          : item
      ))

      return true
    }

    setItens([
      ...itens,
      {
        id: jogo.id,
        nome: jogo.nome,
        preco: jogo.preco,
        estoque: jogo.estoque,
        quantidade: 1,
      },
    ])

    return true
  }

  function alterarQuantidade(idJogo, quantidade) {
    const item = itens.find(item => item.id === idJogo)

    if (!item || quantidade < 1 || quantidade > 10) {
      return
    }

    if (quantidade > item.estoque) {
      return
    }

    setItens(itens.map(item =>
      item.id === idJogo
        ? { ...item, quantidade }
        : item
    ))
  }

  function removerItem(idJogo) {
    setItens(itens.filter(item => item.id !== idJogo))
  }

  function limparCarrinho() {
    setItens([])
  }

  const quantidadeItens = itens.reduce(
    (total, item) => total + item.quantidade,
    0
  )

  const total = itens.reduce(
    (soma, item) => soma + item.preco * item.quantidade,
    0
  )

  const valor = {
    itens,
    quantidadeItens,
    total,
    adicionarItem,
    alterarQuantidade,
    removerItem,
    limparCarrinho,
  }

  return (
    <CarrinhoContext.Provider value={valor}>
      {children}
    </CarrinhoContext.Provider>
  )
}

export default CarrinhoProvider
