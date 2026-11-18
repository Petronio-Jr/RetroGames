
using FluentValidation;
using RetroGames.WebApi.ApiModels.Requests;

namespace RetroGames.WebApi.Validators.Jogo;

public class InserirJogoRequestValidator
    : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Informe o nome do jogo.")
            .MaximumLength(150)
            .WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000)
            .WithMessage("A descrição deve ter no máximo 1000 caracteres.");

        RuleFor(x => x.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("O ano deve estar entre 1970 e 2100.");

        RuleFor(x => x.Preco)
            .Must(preco => preco > 0)
            .WithMessage("O preço deve ser maior que zero.")
            .Must(preco => decimal.Round(preco, 2) == preco)
            .WithMessage("O preço deve ter no máximo duas casas decimais.");

        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.");

        RuleFor(x => x.IdPlataforma)
            .GreaterThan(0)
            .WithMessage("Informe uma plataforma válida.");

        RuleFor(x => x.IdCondicaoJogo)
            .GreaterThan(0)
            .WithMessage("Informe uma condição de jogo válida.");
    }
}
