namespace RetroGames.Services.ServiceModels;

public class PedidoClienteServiceModel
{
    public int Id { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal Total { get; set; }
    public int IdSituacaoPedido { get; set; }
    public string Situacao { get; set; } = string.Empty;
    public List<ItemPedidoClienteServiceModel> Itens { get; set; } = new();
}