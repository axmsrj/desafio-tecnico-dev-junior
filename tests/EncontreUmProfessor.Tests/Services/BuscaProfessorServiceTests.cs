using EncontreUmProfessor.Application.DTOs;
using EncontreUmProfessor.Application.Interfaces;
using EncontreUmProfessor.Application.Services;
using EncontreUmProfessor.Domain.Entities;

namespace EncontreUmProfessor.Tests.Services;

public class BuscaProfessorServiceTests
{
    [Fact]
    public async Task BuscarProfessoresAsync_QuandoNaoEnsinaIdioma_DeveInformarMotivo()
    {
        var professor = CriarProfessor(id: 1, nome: "Ana", idiomas: ["Inglês"]);
        var service = CriarService(professor);

        var resposta = await service.BuscarProfessoresAsync(CriarRequest());

        Assert.Empty(resposta.Professores);
        var naoElegivel = Assert.Single(resposta.NaoElegiveis);
        Assert.Equal(1, naoElegivel.Id);
        Assert.Equal("Não ensina o idioma", naoElegivel.Motivo);
    }

    [Fact]
    public async Task BuscarProfessoresAsync_QuandoAgendaEstaBloqueada_DeveInformarMotivo()
    {
        var professor = CriarProfessor(id: 1, nome: "Ana", agendaBloqueada: true);
        var service = CriarService(professor);

        var resposta = await service.BuscarProfessoresAsync(CriarRequest());

        Assert.Empty(resposta.Professores);
        var naoElegivel = Assert.Single(resposta.NaoElegiveis);
        Assert.Equal("Agenda bloqueada", naoElegivel.Motivo);
    }

    [Fact]
    public async Task BuscarProfessoresAsync_DeveRetornarSomenteHorariosComunsATodosOsDias()
    {
        var professor = CriarProfessor(
            id: 1,
            nome: "Ana",
            disponibilidades:
            [
                NovaDisponibilidade("segunda", "18:00", "20:00"),
                NovaDisponibilidade("quarta", "18:30", "20:30")
            ]);
        var service = CriarService(professor);
        var request = CriarRequest(dias: ["segunda", "quarta"]);

        var resposta = await service.BuscarProfessoresAsync(request);

        var elegivel = Assert.Single(resposta.Professores);
        Assert.Equal(["18:30", "19:00"], elegivel.Horarios);
    }

    [Fact]
    public async Task BuscarProfessoresAsync_DevePermitirAulasQueEncostamEmAulaMarcada()
    {
        var professor = CriarProfessor(
            id: 1,
            nome: "Ana",
            disponibilidades: [NovaDisponibilidade("segunda", "18:00", "21:00")],
            aulas: [NovaAula("segunda", "19:00", 60)]);
        var service = CriarService(professor);
        var request = CriarRequest(fim: "21:00");

        var resposta = await service.BuscarProfessoresAsync(request);

        var elegivel = Assert.Single(resposta.Professores);
        Assert.Equal(["18:00", "20:00"], elegivel.Horarios);
    }

    [Fact]
    public async Task BuscarProfessoresAsync_DeveExigirQueAulaCaibaInteiraNaDisponibilidade()
    {
        var professor = CriarProfessor(
            id: 1,
            nome: "Ana",
            disponibilidades: [NovaDisponibilidade("segunda", "18:30", "19:30")]);
        var service = CriarService(professor);

        var resposta = await service.BuscarProfessoresAsync(CriarRequest());

        var elegivel = Assert.Single(resposta.Professores);
        Assert.Equal(["18:30"], elegivel.Horarios);
    }

    [Fact]
    public async Task BuscarProfessoresAsync_DeveUsarSomenteHoraCheiaOuMeiaHora()
    {
        var professor = CriarProfessor(
            id: 1,
            nome: "Ana",
            disponibilidades: [NovaDisponibilidade("segunda", "18:00", "20:00")]);
        var service = CriarService(professor);
        var request = CriarRequest(inicio: "18:10", fim: "19:45", duracao: 30);

        var resposta = await service.BuscarProfessoresAsync(request);

        var elegivel = Assert.Single(resposta.Professores);
        Assert.Equal(["18:30", "19:00"], elegivel.Horarios);
    }

    [Fact]
    public async Task BuscarProfessoresAsync_DeveOrdenarPorQuantidadeDeAulasEDepoisPorNome()
    {
        var duasAulas = new[]
        {
            NovaAula("terca", "10:00", 60),
            NovaAula("quarta", "10:00", 60)
        };

        var service = CriarService(
            CriarProfessor(id: 1, nome: "Carlos", aulas: duasAulas),
            CriarProfessor(id: 2, nome: "Bruno"),
            CriarProfessor(id: 3, nome: "Ana"));

        var resposta = await service.BuscarProfessoresAsync(CriarRequest());

        Assert.Equal(["Ana", "Bruno", "Carlos"], resposta.Professores.Select(p => p.Nome));
        Assert.Equal([0, 0, 2], resposta.Professores.Select(p => p.AulasNaSemana));
    }

    [Fact]
    public async Task BuscarProfessoresAsync_QuandoNaoHaHorarioComum_DeveInformarMotivo()
    {
        var professor = CriarProfessor(
            id: 1,
            nome: "Ana",
            disponibilidades:
            [
                NovaDisponibilidade("segunda", "18:00", "19:00"),
                NovaDisponibilidade("quarta", "19:00", "20:00")
            ]);
        var service = CriarService(professor);
        var request = CriarRequest(dias: ["segunda", "quarta"]);

        var resposta = await service.BuscarProfessoresAsync(request);

        Assert.Empty(resposta.Professores);
        var naoElegivel = Assert.Single(resposta.NaoElegiveis);
        Assert.Equal("Sem horário em comum nos dias pedidos", naoElegivel.Motivo);
    }

    [Fact]
    public async Task BuscarProfessoresAsync_NaoDeveAtravessarMeiaNoite()
    {
        var professor = CriarProfessor(
            id: 1,
            nome: "Ana",
            disponibilidades:
            [
                NovaDisponibilidade("segunda", "00:00", "01:00")
            ]);
        var service = CriarService(professor);
        var request = CriarRequest(
            inicio: "23:45",
            fim: "23:59",
            duracao: 30);

        var resposta = await service.BuscarProfessoresAsync(request);

        Assert.Empty(resposta.Professores);
        var naoElegivel = Assert.Single(resposta.NaoElegiveis);
        Assert.Equal("Sem horário em comum nos dias pedidos", naoElegivel.Motivo);
    }

    private static BuscaProfessorService CriarService(params Professor[] professores)
    {
        return new BuscaProfessorService(new ProfessorRepositoryFake(professores));
    }

    private static BuscarProfessoresRequest CriarRequest(
        string[]? dias = null,
        string inicio = "18:00",
        string fim = "20:00",
        int duracao = 60)
    {
        return new BuscarProfessoresRequest
        {
            Idioma = "Espanhol",
            Dias = dias ?? ["segunda"],
            InicioFaixa = inicio,
            FimFaixa = fim,
            DuracaoMinutos = duracao
        };
    }

    private static Professor CriarProfessor(
        int id,
        string nome,
        string[]? idiomas = null,
        bool agendaBloqueada = false,
        Disponibilidade[]? disponibilidades = null,
        AulaMarcada[]? aulas = null)
    {
        return new Professor
        {
            Id = id,
            Nome = nome,
            Idiomas = idiomas ?? ["Espanhol"],
            AgendaBloqueada = agendaBloqueada,
            Disponibilidade = disponibilidades ??
            [
                NovaDisponibilidade("segunda", "18:00", "20:00")
            ],
            AulasMarcadas = aulas ?? []
        };
    }

    private static Disponibilidade NovaDisponibilidade(
        string dia,
        string inicio,
        string fim)
    {
        return new Disponibilidade
        {
            Dia = dia,
            Inicio = inicio,
            Fim = fim
        };
    }

    private static AulaMarcada NovaAula(
        string dia,
        string inicio,
        int duracaoMinutos)
    {
        return new AulaMarcada
        {
            Dia = dia,
            Inicio = inicio,
            DuracaoMinutos = duracaoMinutos
        };
    }

    private sealed class ProfessorRepositoryFake : IProfessorRepository
    {
        private readonly IReadOnlyList<Professor> _professores;

        public ProfessorRepositoryFake(IReadOnlyList<Professor> professores)
        {
            _professores = professores;
        }

        public Task<IReadOnlyList<Professor>> BuscarProfessoresAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_professores);
        }
    }
}
