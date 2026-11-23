using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services;

public interface IPedidoService
{
    Task<Pedido> FinalizarAsync(
        int idUsuario,
        List<ItemPedidoSolicitadoServiceModel> itensSolicitados);
    
    Task<List<PedidoClienteServiceModel>> ListarMeusAsync(int idUsuario);
}