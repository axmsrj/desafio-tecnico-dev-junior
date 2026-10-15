namespace EncontreUmProfessor.Domain.Entities;

public class AulaMarcada
{
    public required string Dia { get; set; }
    public required string Inicio { get; set; }
    public int DuracaoMinutos { get; set; }
}