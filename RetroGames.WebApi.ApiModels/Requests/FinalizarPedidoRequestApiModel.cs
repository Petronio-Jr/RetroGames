namespace RetroGames.WebApi.ApiModels.Requests;

public class FinalizarPedidoRequestApiModel
{
    public List<ItemPedidoRequestApiModel> Itens { get; set; } = new();
}

public class ItemPedidoRequestApiModel
{
    public int IdJogo { get; set; }

    public int Quantidade { get; set; }
}