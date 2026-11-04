export function obterMensagemErro(erro) {
  return erro?.response?.data?.errorMessage ?? 'Ocorreu um erro inesperado.';
}

export function obterErrosDosCampos(erro) {
  const resposta = erro?.response?.data;
  return resposta?.responseCode === 3 ? (resposta.data ?? {}) : {};
}
