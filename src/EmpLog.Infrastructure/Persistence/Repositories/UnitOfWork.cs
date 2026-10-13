using EmpLog.Domain.Common;
using EmpLog.Domain.Interfaces;
using EmpLog.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace EmpLog.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly EmpLogDbContext _context;

    public UnitOfWork(EmpLogDbContext context)
    {
        _context = context;
    }

    public async Task<ResultadoSalvamento> SalvarAlteracoesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return ResultadoSalvamento.Salvo();
        }
        catch (DbUpdateException excecao) when (EhViolacaoDeUnicidade(excecao))
        {
            _context.ChangeTracker.Clear();
            return ResultadoSalvamento.ConflitoDe(CampoDoConflito(excecao));
        }
    }

    private static bool EhViolacaoDeUnicidade(Exception excecao)
    {
        for (var atual = excecao; atual is not null; atual = atual.InnerException)
        {
            if (atual.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string CampoDoConflito(Exception excecao)
    {
        var mensagem = excecao.ToString();
        if (mensagem.Contains("CnhNumero", StringComparison.OrdinalIgnoreCase))
        {
            return "cnh";
        }

        return "cpf";
    }
}
