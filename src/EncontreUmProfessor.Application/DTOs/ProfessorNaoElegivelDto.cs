namespace EncontreUmProfessor.Application.DTOs;

public class ProfessorNaoElegivelDto
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string Motivo { get; set; }
}