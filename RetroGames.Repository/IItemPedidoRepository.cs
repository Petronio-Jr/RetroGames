using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IItemPedidoRepository
{
    Task InserirAsync(List<ItemPedido> itens);

    Task<List<ItemPedido>> ListarPorPedidosAsync(List<int> idsPedidos);
}
