namespace EncontreUmProfessor.Application.DTOs;

public class BuscarProfessoresRequest
{
    public required string Idioma { get; set; }
    public string[]? Dias { get; set; }
    public required string InicioFaixa { get; set; }
    public required string FimFaixa { get; set; }
    public int DuracaoMinutos { get; set; }
}