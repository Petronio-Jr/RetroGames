using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Services;

namespace RetroGames.WebApi.Services;

public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtualService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Usuario =>
        _httpContextAccessor.HttpContext?.User;

    public bool EstaAutenticado =>
        Usuario?.Identity?.IsAuthenticated == true;

    public int? IdUsuario
    {
        get
        {
            if (!EstaAutenticado)
                return null;

            var id = Usuario?.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

            return int.TryParse(id, out var idUsuario)
                ? idUsuario
                : null;
        }
    }

    public bool EhAdministrador =>
        EstaAutenticado &&
        Usuario!.IsInRole(Perfis.Administrador);
}
