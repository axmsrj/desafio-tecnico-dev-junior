using EncontreUmProfessor.Application.DTOs;
using EncontreUmProfessor.Application.Interfaces;
using EncontreUmProfessor.Api.Validation;

namespace EncontreUmProfessor.Api.Endpoints;

public static class ProfessoresEndpoint
{
    public static void MapProfessoresEndpoint(this WebApplication app)
    {
        app.MapPost("/api/professores/busca", async (BuscarProfessoresRequest request, IBuscaProfessorService service,
            CancellationToken cancellationToken) =>
        {
            var erro = BuscarProfessoresValidator.Validar(request);

            if (erro is not null)
            {
                return Results.BadRequest(new { mensagem = erro });
            }

            var response = await service.BuscarProfessoresAsync(request, cancellationToken);
            return Results.Ok(response);
        });
    }
}
