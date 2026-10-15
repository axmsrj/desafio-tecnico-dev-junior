using System.Text.Json;
using EncontreUmProfessor.Application.Interfaces;
using EncontreUmProfessor.Domain.Entities;

namespace EncontreUmProfessor.Infrastructure.Data;

public class ProfessorRepository : IProfessorRepository
{
    private readonly string _arquivoPath;

    public ProfessorRepository(string arquivoPath)
    {
        _arquivoPath = arquivoPath;
    }
    
    public async Task<IReadOnlyList<Professor>> BuscarProfessoresAsync(CancellationToken cancellationToken = default)
    {
        await using var arquivo = File.OpenRead(_arquivoPath);

        var data = await JsonSerializer.DeserializeAsync<ProfessoresJson>(arquivo, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            },
            cancellationToken);

        return data?.Professores ?? [];
    }
}