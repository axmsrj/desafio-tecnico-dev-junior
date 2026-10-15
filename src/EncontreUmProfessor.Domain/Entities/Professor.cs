namespace EncontreUmProfessor.Domain.Entities;

public class Professor
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public required string[] Idiomas { get; set; }
    public bool AgendaBloqueada { get; set; }
    public required Disponibilidade[] Disponibilidade { get; set; }
    public required AulaMarcada[] AulasMarcadas { get; set; }
}