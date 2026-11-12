
using System.Net;

namespace RetroGames.Services;

public sealed record BusinessError(
    int Codigo,
    string Mensagem,
    HttpStatusCode StatusHttp
);

public static class BusinessErrors
{
    public static readonly BusinessError PlataformaNaoEncontrada =
        new(100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static readonly BusinessError PlataformaNomeDuplicado =
        new(101, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    public static readonly BusinessError PlataformaPossuiJogos =
        new(102, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    public static readonly BusinessError JogoNaoEncontrado =
        new(103, "O id do jogo não existe.", HttpStatusCode.NotFound);

    public static readonly BusinessError CondicaoNaoEncontrada =
        new(104, "A condição de jogo não existe.", HttpStatusCode.NotFound);

    public static readonly BusinessError GeneroNaoEncontrado =
        new(105, "O gênero do jogo não existe.", HttpStatusCode.NotFound);
}
