using FluentValidation;

namespace EmpLog.Application.Motoristas.Commands.CadastrarMotorista;

public sealed class CadastrarMotoristaCommandValidator : AbstractValidator<CadastrarMotoristaCommand>
{
    public CadastrarMotoristaCommandValidator()
    {
        RuleFor(comando => comando.NomeCompleto)
            .NotEmpty()
            .WithMessage("Nome completo é obrigatório.")
            .OverridePropertyName("nomeCompleto");

        RuleFor(comando => comando.Cpf)
            .NotEmpty()
            .WithMessage("CPF é obrigatório.")
            .OverridePropertyName("cpf");

        RuleFor(comando => comando.Email)
            .NotEmpty()
            .WithMessage("E-mail é obrigatório.")
            .OverridePropertyName("email");

        RuleFor(comando => comando.Telefone)
            .NotEmpty()
            .WithMessage("Telefone é obrigatório.")
            .OverridePropertyName("telefone");

        RuleFor(comando => comando.Cnh)
            .NotNull()
            .WithMessage("CNH é obrigatória.")
            .OverridePropertyName("cnh");

        When(comando => comando.Cnh is not null, () =>
        {
            RuleFor(comando => comando.Cnh!.Numero)
                .NotEmpty()
                .WithMessage("Número da CNH é obrigatório.")
                .OverridePropertyName("cnh.numero");

            RuleFor(comando => comando.Cnh!.Categoria)
                .NotEmpty()
                .WithMessage("Categoria da CNH é obrigatória.")
                .OverridePropertyName("cnh.categoria");

            RuleFor(comando => comando.Cnh!.Validade)
                .NotNull()
                .WithMessage("Validade da CNH é obrigatória.")
                .OverridePropertyName("cnh.validade");
        });
    }
}
