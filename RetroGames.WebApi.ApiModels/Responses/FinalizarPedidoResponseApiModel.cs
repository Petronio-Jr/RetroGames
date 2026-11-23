namespace RetroGames.WebApi.ApiModels.Responses;

public class FinalizarPedidoResponseApiModel
{
    public int Id { get; set; }

    public DateTime DataPedido { get; set; }

    public decimal Total { get; set; }

    public int IdSituacaoPedido { get; set; }
}