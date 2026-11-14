namespace RetroGames.WebApi.ApiModels.Responses;

public class RetroGamesResponseApiModel<T>
{
    public int ResponseCode { get; set; }

    public T? Data { get; set; }

    public string? ErrorMessage { get; set; }
}
