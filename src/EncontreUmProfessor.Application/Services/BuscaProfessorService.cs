using EncontreUmProfessor.Application.DTOs;
using EncontreUmProfessor.Application.Interfaces;
using EncontreUmProfessor.Domain.Entities;

namespace EncontreUmProfessor.Application.Services;

public class BuscaProfessorService : IBuscaProfessorService
{
    private readonly IProfessorRepository _professorRepository;

    public BuscaProfessorService(IProfessorRepository professorRepository)
    {
        _professorRepository = professorRepository;
    }
    
    public async Task<BuscarProfessoresResponse> BuscarProfessoresAsync(BuscarProfessoresRequest request, CancellationToken cancellationToken = default)
    {
        var professores = await _professorRepository.BuscarProfessoresAsync(cancellationToken);
        
        var elegiveis = new List<ProfessorElegivelDto>();
        var naoElegiveis = new List<ProfessorNaoElegivelDto>();
        var inicio = TimeOnly.Parse(request.InicioFaixa);
        var fim = TimeOnly.Parse(request.FimFaixa);

        foreach (var prof in professores)
        {
            if (!prof.Idiomas.Contains(request.Idioma, StringComparer.OrdinalIgnoreCase))
            {
                naoElegiveis.Add(AddNaoElegivel(prof, "Não ensina o idioma"));
                continue;
            }

            if (prof.AgendaBloqueada)
            {
                naoElegiveis.Add(AddNaoElegivel(prof, "Agenda bloqueada"));
                continue;
            }

            var horarios = FindHorariosEmComum(prof, inicio, fim, request.Dias!, request.DuracaoMinutos);

            if (horarios.Count == 0)
            {
                naoElegiveis.Add(AddNaoElegivel(prof, "Sem horário em comum nos dias pedidos"));
                continue;
            }
            
            elegiveis.Add(new ProfessorElegivelDto
            {
                Id = prof.Id,
                Nome = prof.Nome,
                AulasNaSemana = prof.AulasMarcadas.Length,
                Horarios = horarios.Select(horario => horario.ToString("HH:mm")).ToArray()
            });
        }

        var professoresResponse = new BuscarProfessoresResponse
        {
            Professores = elegiveis.OrderBy(p => p.AulasNaSemana).ThenBy(p => p.Nome).ToArray(),
            NaoElegiveis = naoElegiveis.ToArray()
        };
        return professoresResponse;
    }

    private static ProfessorNaoElegivelDto AddNaoElegivel(Professor professor, string motivo)
    {
        return new ProfessorNaoElegivelDto
        {
            Id = professor.Id,
            Nome = professor.Nome,
            Motivo = motivo
        };
    }

    private static List<TimeOnly> FindHorariosEmComum(Professor professor, TimeOnly inicio,
        TimeOnly fim, IEnumerable<string> dias, int minutos)
    {
        var horarios = new List<TimeOnly>();
        var horarioEmPrimeiro = inicio;
        
        // arredondando aki pro proximo horario
        if (inicio.Minute is > 0 and < 30)
        {
            horarioEmPrimeiro = new TimeOnly(inicio.Hour, 30);
        }
        else if (inicio.Minute > 30)
        {
            horarioEmPrimeiro = new TimeOnly(inicio.Hour, 0).AddHours(1);
        }
        
        if (horarioEmPrimeiro < inicio)
        {
            return horarios;
        }

        for (var inicioDaAula = horarioEmPrimeiro;
             inicioDaAula.ToTimeSpan() + TimeSpan.FromMinutes(minutos) <= fim.ToTimeSpan();
             inicioDaAula = inicioDaAula.AddMinutes(30))
        {
            var disponivelTodosDias = dias.All(dia => IsDisponivel(professor, inicioDaAula, dia, minutos));

            if (disponivelTodosDias)
            {
                horarios.Add(inicioDaAula);
            }
        }
        return horarios;
    }

    private static bool IsDisponivel(Professor professor, TimeOnly inicio, string dia, int minutos)
    {
        var fim = inicio.AddMinutes(minutos);
        
        var disponibilidade = professor.Disponibilidade
            .Where(disp => string.Equals(disp.Dia, dia, StringComparison.OrdinalIgnoreCase))
            .Any(disp =>
            {
                var inicioDisp = TimeOnly.Parse(disp.Inicio);
                var fimDisp = TimeOnly.Parse(disp.Fim);
                return inicio >= inicioDisp && fim <= fimDisp;
            });

        if (!disponibilidade)
            return false;

        var temConflito = professor.AulasMarcadas
            .Where(aula => string.Equals(aula.Dia, dia, StringComparison.OrdinalIgnoreCase))
            .Any(aula =>
            {
                var inicioDaAulaMarcada = TimeOnly.Parse(aula.Inicio);
                var fimDaAulaMarcada = inicioDaAulaMarcada.AddMinutes(aula.DuracaoMinutos);
                return inicio < fimDaAulaMarcada && fim > inicioDaAulaMarcada;
            });

        return !temConflito;
    }
}