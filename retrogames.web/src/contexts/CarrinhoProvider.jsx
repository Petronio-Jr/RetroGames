import { useEffect, useState } from 'react'
import CarrinhoContext from './carrinhoContext'
import useAutenticacao from '../hooks/useAutenticacao'

const CHAVE_CARRINHO_VISITANTE = 'retrogames-carrinho-visitante'

function obterChaveCarrinhoUsuario(idUsuario) {
  return `retrogames-carrinho-usuario-${idUsuario}`
}

function obterCarrinhoSalvo(chave) {
  try {
    const dados = localStorage.getItem(chave)

    if (!dados) {
      return []
    }

    const carrinho = JSON.parse(dados)

    return Array.isArray(carrinho) ? carrinho : []
  } catch {
    return []
  }
}

function combinarCarrinhos(carrinhoConta, carrinhoVisitante) {
  const resultado = [...carrinhoConta]

  carrinhoVisitante.forEach(itemVisitante => {
    const itemExistente = resultado.find(
      item => item.id === itemVisitante.id
    )

    const estoque = Math.max(
      0,
      Number(itemVisitante.estoque) || 0
    )

    if (estoque <= 0) {
      return
    }

    if (itemExistente) {
      const quantidadeAtual = Number(itemExistente.quantidade) || 0
      const quantidadeVisitante = Number(itemVisitante.quantidade) || 0

      itemExistente.estoque = Math.min(
        Number(itemExistente.estoque) || estoque,
        estoque
      )

      itemExistente.quantidade = Math.min(
        10,
        itemExistente.estoque,
        quantidadeAtual + quantidadeVisitante
      )

      return
    }

    resultado.push({
      ...itemVisitante,
      quantidade: Math.min(
        10,
        estoque,
        Number(itemVisitante.quantidade) || 1
      ),
    })
  })

  return resultado.filter(
    item => item.quantidade > 0 && item.estoque > 0
  )
}

function CarrinhoProvider({ children }) {
  const { usuario, carregando } = useAutenticacao()

  const chaveAtual = usuario
    ? obterChaveCarrinhoUsuario(usuario.id)
    : CHAVE_CARRINHO_VISITANTE

  const [itens, setItens] = useState([])
  const [chaveCarregada, setChaveCarregada] = useState(null)

  // Carrega o carrinho correspondente à identidade atual.
  useEffect(() => {
    if (carregando) {
      return
    }

    let carrinho = obterCarrinhoSalvo(chaveAtual)

    // Ao entrar em uma conta, combina o carrinho de visitante
    // com o carrinho que já pertence àquela conta.
    if (usuario) {
      const carrinhoVisitante = obterCarrinhoSalvo(
        CHAVE_CARRINHO_VISITANTE
      )

      if (carrinhoVisitante.length > 0) {
        carrinho = combinarCarrinhos(
          carrinho,
          carrinhoVisitante
        )

        localStorage.setItem(
          chaveAtual,
          JSON.stringify(carrinho)
        )

        localStorage.removeItem(CHAVE_CARRINHO_VISITANTE)
      }
    }

    setItens(carrinho)
    setChaveCarregada(chaveAtual)
  }, [carregando, usuario?.id, chaveAtual])

  // Só salva depois que o carrinho da identidade atual foi carregado.
  // Isso evita gravar os itens da conta anterior na chave da conta nova.
  useEffect(() => {
    if (carregando || chaveCarregada !== chaveAtual) {
      return
    }

    localStorage.setItem(
      chaveAtual,
      JSON.stringify(itens)
    )
  }, [itens, chaveAtual, chaveCarregada, carregando])

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

      setItens(itensAtuais =>
        itensAtuais.map(item =>
          item.id === jogo.id
            ? {
                ...item,
                quantidade: item.quantidade + 1,
                estoque: jogo.estoque,
                preco: jogo.preco,
                nome: jogo.nome,
              }
            : item
        )
      )

      return true
    }

    setItens(itensAtuais => [
      ...itensAtuais,
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

    setItens(itensAtuais =>
      itensAtuais.map(item =>
        item.id === idJogo
          ? { ...item, quantidade }
          : item
      )
    )
  }

  function removerItem(idJogo) {
    setItens(itensAtuais =>
      itensAtuais.filter(item => item.id !== idJogo)
    )
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