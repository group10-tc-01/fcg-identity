using FluentValidation;

namespace Fcg.Identity.Application.UseCases.Items.CreateItem;

public sealed class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    public CreateItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(120);

        RuleFor(x => x.Price)
            .GreaterThan(0);
    }
}
