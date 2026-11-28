using FluentValidation;
using RetroGames.WebApi.ApiModels.Requests;

namespace RetroGames.WebApi.Validators.Pedido;

public class FinalizarPedidoRequestValidator
    : AbstractValidator<FinalizarPedidoRequestApiModel>
{
    public FinalizarPedidoRequestValidator()
    {
        RuleFor(x => x.Itens)
            .NotEmpty()
            .WithMessage("O pedido deve possuir pelo menos um item.");

        RuleForEach(x => x.Itens)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.IdJogo)
                    .GreaterThan(0)
                    .WithMessage("Informe um jogo válido.");

                item.RuleFor(x => x.Quantidade)
                    .InclusiveBetween(1, 10)
                    .WithMessage("A quantidade deve estar entre 1 e 10.");
            });

        RuleFor(x => x.Itens)
            .Must(itens =>
                itens == null ||
                itens.Select(item => item.IdJogo)
                    .Distinct()
                    .Count() == itens.Count)
            .WithMessage("Não repita jogos no mesmo pedido.");
    }
}