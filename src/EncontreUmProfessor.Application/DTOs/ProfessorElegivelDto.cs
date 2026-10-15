namespace EncontreUmProfessor.Application.DTOs;

public class ProfessorElegivelDto
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public int AulasNaSemana { get; set; }
    public required string[] Horarios { get; set; }
}