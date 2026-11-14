namespace RetroGames.WebApi.ApiModels.Responses;

public sealed record ResponseCode(
    int Code,
    string DefaultMessage
);

public static class ResponseCodes
{
    public static readonly ResponseCode SUCESSO =
        new(0, string.Empty);

    public static readonly ResponseCode ERRO =
        new(1, "Ocorreu um erro inesperado.");
}
