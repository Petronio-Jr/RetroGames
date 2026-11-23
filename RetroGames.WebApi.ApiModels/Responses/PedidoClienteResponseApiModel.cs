namespace RetroGames.WebApi.ApiModels.Responses;

public class PedidoClienteResponseApiModel
{
    public int Id { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal Total { get; set; }
    public int IdSituacaoPedido { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public List<ItemPedidoClienteResponseApiModel> Itens { get; set; } = new();
}

public class ItemPedidoClienteResponseApiModel
{
    public int IdJogo { get; set; }
    public string NomeJogo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}