using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels.Requests;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AutenticacaoController(
        IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    // POST: api/Autenticacao/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestApiModel request)
    {
        var usuario = await _autenticacaoService.AutenticarAsync(
            request.Email,
            request.Senha
        );

        if (usuario is null)
        {
            var erro = ResponseCodes.CREDENCIAIS_INVALIDAS;

            var respostaErro = new RetroGamesResponseApiModel
            {
                ResponseCode = erro.Code,
                ErrorMessage = erro.DefaultMessage
            };

            return Unauthorized(respostaErro);
        }

        var identidade = usuario.CriarClaimsPrincipal();

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            identidade
        );

        var resposta = new RetroGamesResponseApiModel<
            UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,

            Data = new UsuarioLogadoResponseApiModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,

                Perfil = usuario.IdPerfil == PerfilIds.Administrador
                    ? Perfis.Administrador
                    : Perfis.Cliente
            },

            ErrorMessage = null
        };

        return Ok(resposta);
    }

    // POST: api/Autenticacao/logout
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,
            ErrorMessage = null
        };

        return Ok(resposta);
    }

    // GET: api/Autenticacao/eu
    [Authorize]
    [HttpGet("eu")]
    public IActionResult Eu()
    {
        var id = User.ObterIdUsuario();

        if (id is null)
        {
            var erro = ResponseCodes.NAO_AUTENTICADO;

            return Unauthorized(
                new RetroGamesResponseApiModel
                {
                    ResponseCode = erro.Code,
                    ErrorMessage = erro.DefaultMessage
                }
            );
        }

        var resposta = new RetroGamesResponseApiModel<
            UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO.Code,

            Data = new UsuarioLogadoResponseApiModel
            {
                Id = id.Value,
                Nome = User.Identity?.Name ?? string.Empty,
                Email = User.FindFirst(ClaimTypes.Email)?.Value
                    ?? string.Empty,
                Perfil = User.ObterPerfil() ?? string.Empty
            },

            ErrorMessage = null
        };

        return Ok(resposta);
    }
}
