using EncontreUmProfessor.Application.DTOs;

namespace EncontreUmProfessor.Application.Interfaces;

public interface IBuscaProfessorService
{ 
    Task<BuscarProfessoresResponse> BuscarProfessoresAsync(BuscarProfessoresRequest request, CancellationToken cancellationToken = default);
}