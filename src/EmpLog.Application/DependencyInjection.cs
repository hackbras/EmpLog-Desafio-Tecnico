using EmpLog.Application.Motoristas.Commands.CadastrarMotorista;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmpLog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CadastrarMotoristaCommandValidator>();
        services.AddScoped<CadastrarMotoristaHandler>();
        return services;
    }
}
