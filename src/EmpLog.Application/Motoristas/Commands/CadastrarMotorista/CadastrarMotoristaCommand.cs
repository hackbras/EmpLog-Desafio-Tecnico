namespace EmpLog.Application.Motoristas.Commands.CadastrarMotorista;

public sealed record CnhCadastro
{
    public string? Numero { get; init; }

    public string? Categoria { get; init; }

    public DateOnly? Validade { get; init; }
}

public sealed record CadastrarMotoristaCommand
{
    public string? NomeCompleto { get; init; }

    public string? Cpf { get; init; }

    public string? Email { get; init; }

    public string? Telefone { get; init; }

    public CnhCadastro? Cnh { get; init; }
}
