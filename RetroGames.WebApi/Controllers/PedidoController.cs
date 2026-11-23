using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.Services.ServiceModels;
using RetroGames.WebApi.ApiModels.Requests;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidoController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    // POST: api/Pedido
    [HttpPost]
    public async Task<IActionResult> Finalizar(
        [FromBody] FinalizarPedidoRequestApiModel request)
    {
        var idUsuario = User.ObterIdUsuario();

        if (idUsuario is null)
        {
            var erro = ResponseCodes.NAO_AUTENTICADO;

            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = erro.Code,
                ErrorMessage = erro.DefaultMessage
            });
        }

        var itensSolicitados = request.Itens
            .Select(item => new ItemPedidoSolicitadoServiceModel
            {
                IdJogo = item.IdJogo,
                Quantidade = item.Quantidade
            })
            .ToList();

        var pedido = await _pedidoService.FinalizarAsync(
            idUsuario.Value,
            itensSolicitados
        );

        var resposta = new RetroGamesResponseApiModel<
            FinalizarPedidoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,

            Data = new FinalizarPedidoResponseApiModel
            {
                Id = pedido.Id,
                DataPedido = pedido.DataPedido,
                Total = pedido.Total,
                IdSituacaoPedido = pedido.IdSituacaoPedido
            },

            ErrorMessage = null
        };

        return StatusCode(StatusCodes.Status201Created, resposta);
    }

     // GET: api/Pedido/meus
    [HttpGet("meus")]
    public async Task<IActionResult> ListarMeus()
    {
        var idUsuario = User.ObterIdUsuario();

        if (idUsuario is null)
        {
            var erro = ResponseCodes.NAO_AUTENTICADO;

            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = erro.Code,
                ErrorMessage = erro.DefaultMessage
            });
        }

        var pedidos = await _pedidoService.ListarMeusAsync(
            idUsuario.Value
        );

        var resposta = pedidos.Select(p => new PedidoClienteResponseApiModel
        {
            Id = p.Id,
            DataPedido = p.DataPedido,
            Total = p.Total,
            IdSituacaoPedido = p.IdSituacaoPedido,
            Situacao = p.Situacao,

            Itens = p.Itens.Select(i => new ItemPedidoClienteResponseApiModel
            {
                IdJogo = i.IdJogo,
                NomeJogo = i.NomeJogo,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario
            }).ToList()
        }).ToList();

        return Ok(new RetroGamesResponseApiModel<List<PedidoClienteResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,
            Data = resposta,
            ErrorMessage = null
        });
    }   
}