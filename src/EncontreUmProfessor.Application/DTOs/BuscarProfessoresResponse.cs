namespace EncontreUmProfessor.Application.DTOs;

public class BuscarProfessoresResponse
{
    public required ProfessorElegivelDto[] Professores { get; set; }
    public required ProfessorNaoElegivelDto[] NaoElegiveis { get; set; }
}