using EncontreUmProfessor.Domain.Entities;

namespace EncontreUmProfessor.Application.Interfaces;

public interface IProfessorRepository
{
    Task<IReadOnlyList<Professor>> BuscarProfessoresAsync(CancellationToken cancellationToken = default);
}