using EmpLog.Domain.Entities;
using EmpLog.Domain.ValueObjects;

namespace EmpLog.Application.Motoristas.DTOs;

public sealed record CnhDto(string Numero, string Categoria, DateOnly Validade);

public sealed record MotoristaDto(
    Guid Id,
    string NomeCompleto,
    string Cpf,
    string Email,
    string Telefone,
    CnhDto Cnh,
    string Status,
    DateTime CriadoEmUtc)
{
    public static MotoristaDto From(Motorista motorista) =>
        new(
            motorista.Id,
            motorista.NomeCompleto,
            motorista.Cpf.Valor,
            motorista.Email.Valor,
            motorista.Telefone.Valor,
            new CnhDto(motorista.Cnh.Numero, motorista.Cnh.Categoria.ToString(), motorista.Cnh.Validade),
            motorista.Status.ToString(),
            motorista.CriadoEmUtc);
}
