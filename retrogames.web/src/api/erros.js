export function obterMensagemErro(erro) {
  if (!erro.response) {
    return 'Não foi possível conectar ao servidor.'
  }

  return (
    erro.response.data?.errorMessage ||
    'Ocorreu um erro inesperado.'
  )
}

export function obterErrosDosCampos(erro) {
  const resposta = erro.response?.data

  if (resposta?.responseCode !== 3 || !resposta.data) {
    return {}
  }

  const erros = {}

  for (const campo in resposta.data) {
    erros[campo] = resposta.data[campo][0]
  }

  return erros
}
