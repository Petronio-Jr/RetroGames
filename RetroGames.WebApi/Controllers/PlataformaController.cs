using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels.Requests;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _plataformaService;

    public PlataformaController(
        IPlataformaService plataformaService)
    {
        _plataformaService = plataformaService;
    }

    // GET: api/Plataforma
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var plataformas =
            await _plataformaService.ListarAsync();

        var response =
            new RetroGamesResponseApiModel<
                List<ListarPlataformaResponseApiModel>>
            {
                ResponseCode = ResponseCodes.SUCESSO.Code,
                Data = plataformas
                    .Select(plataforma => plataforma.ParaResponse())
                    .ToList(),
                ErrorMessage = null
            };

        return Ok(response);
    }

    // GET: api/Plataforma/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ListarPorId(int id)
    {
        var plataforma =
            await _plataformaService.BuscarPorIdAsync(id);

        var response =
            new RetroGamesResponseApiModel<
                ListarPlataformaResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO.Code,
                Data = plataforma.ParaResponse(),
                ErrorMessage = null
            };

        return Ok(response);
    }

    // POST: api/Plataforma
    [HttpPost]
    public async Task<IActionResult> Inserir(
        [FromBody] InserirPlataformaRequestApiModel request)
    {
        var criada = await _plataformaService.InserirAsync(
            request.ParaModel()
        );

        var response =
            new RetroGamesResponseApiModel<
                ListarPlataformaResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO.Code,
                Data = criada.ParaResponse(),
                ErrorMessage = null
            };

        return CreatedAtAction(
            nameof(ListarPorId),
            new { id = criada.Id },
            response
        );
    }

    // PUT: api/Plataforma/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Alterar(
        int id,
        [FromBody] AlterarPlataformaRequestApiModel request)
    {
        var alterada = await _plataformaService.AlterarAsync(
            request.ParaModel(id)
        );

        var response =
            new RetroGamesResponseApiModel<
                ListarPlataformaResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO.Code,
                Data = alterada.ParaResponse(),
                ErrorMessage = null
            };

        return Ok(response);
    }

    // DELETE: api/Plataforma/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _plataformaService.ExcluirAsync(id);

        var response = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,
            ErrorMessage = null
        };

        return Ok(response);
    }
}
