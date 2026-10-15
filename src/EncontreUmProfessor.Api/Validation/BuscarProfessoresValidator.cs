using System.Globalization;
using EncontreUmProfessor.Application.DTOs;

namespace EncontreUmProfessor.Api.Validation;

public static class BuscarProfessoresValidator
{
    private static readonly HashSet<string> DiasValidos = new(
        [
            "segunda",
            "terca",
            "quarta",
            "quinta",
            "sexta",
            "sabado",
            "domingo"
        ],
        StringComparer.OrdinalIgnoreCase);

    private static readonly HashSet<int> DuracoesValidas = [30, 60, 90];

    public static string? Validar(BuscarProfessoresRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Idioma))
        {
            return "O idioma é obrigatório.";
        }

        if (request.Dias is null || request.Dias.Length == 0)
        {
            return "Informe pelo menos um dia da semana.";
        }

        var possuiDiaInvalido = request.Dias.Any(dia =>
            string.IsNullOrWhiteSpace(dia) || !DiasValidos.Contains(dia));

        if (possuiDiaInvalido)
        {
            return "Um ou mais dias da semana são inválidos.";
        }

        if (!TimeOnly.TryParseExact(
                request.InicioFaixa,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var inicio))
        {
            return "O horário inicial deve estar no formato HH:mm.";
        }

        if (!TimeOnly.TryParseExact(
                request.FimFaixa,
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var fim))
        {
            return "O horário final deve estar no formato HH:mm.";
        }

        if (fim <= inicio)
        {
            return "O horário final deve ser posterior ao horário inicial.";
        }
        return DuracoesValidas.Contains(request.DuracaoMinutos) ? null : "A duração deve ser de 30, 60 ou 90 minutos.";
    }
}
