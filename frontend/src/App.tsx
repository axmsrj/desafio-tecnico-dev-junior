import { zodResolver } from '@hookform/resolvers/zod'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { buscaSchema, type BuscaFormData } from './schemas/buscaSchema'
import { buscarProfessores } from './services/professoresApi'
import type { BuscarProfessoresResponse } from './types'

const diasSemana = [
  { valor: 'segunda', rotulo: 'Seg' },
  { valor: 'terca', rotulo: 'Ter' },
  { valor: 'quarta', rotulo: 'Qua' },
  { valor: 'quinta', rotulo: 'Qui' },
  { valor: 'sexta', rotulo: 'Sex' },
  { valor: 'sabado', rotulo: 'Sáb' },
  { valor: 'domingo', rotulo: 'Dom' },
] as const

function App() {
  const [resultado, setResultado] = useState<BuscarProfessoresResponse | null>(null)
  const [erro, setErro] = useState<string | null>(null)

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<BuscaFormData>({
    resolver: zodResolver(buscaSchema),
    defaultValues: {
      idioma: 'Inglês',
      dias: ['segunda'],
      inicioFaixa: '18:00',
      fimFaixa: '20:00',
      duracaoMinutos: 60,
    },
  })

  async function onSubmit(filtros: BuscaFormData) {
    setErro(null)
    setResultado(null)

    try {
      setResultado(await buscarProfessores(filtros))
    } catch (erro) {
      setErro(
        erro instanceof TypeError
          ? 'Não foi possível se comunicar com a API.'
          : erro instanceof Error
          ? erro.message
          : 'Não foi possível se comunicar com a API.',
      )
    }
  }

  return (
    <main className="min-h-screen bg-slate-50 px-4 py-10 text-slate-900 sm:px-6">
      <div className="mx-auto max-w-6xl">
        <header className="mb-8">
          <p className="mb-2 text-sm font-semibold uppercase tracking-widest text-indigo-600">
            Escola de idiomas
          </p>
          <h1 className="text-3xl font-bold tracking-tight sm:text-4xl">
            Encontre um professor
          </h1>
          <p className="mt-3 max-w-2xl text-slate-600">
            Informe sua disponibilidade para encontrar professores e horários
            compatíveis.
          </p>
        </header>

        <div className="grid items-start gap-8 lg:grid-cols-[22rem_1fr]">
          <form
            className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm"
            onSubmit={handleSubmit(onSubmit)}
          >
            <h2 className="mb-6 text-lg font-semibold">Filtros da busca</h2>

            <label className="block text-sm font-medium" htmlFor="idioma">
              Idioma
            </label>
            <select
              id="idioma"
              className="mt-2 w-full rounded-lg border border-slate-300 bg-white px-3 py-2 outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
              {...register('idioma')}
            >
              <option>Inglês</option>
              <option>Espanhol</option>
              <option>Francês</option>
            </select>

            <fieldset className="mt-6">
              <legend className="text-sm font-medium">Dias da semana</legend>
              <div className="mt-2 grid grid-cols-4 gap-2">
                {diasSemana.map((dia) => (
                  <label key={dia.valor} className="cursor-pointer">
                    <input
                      type="checkbox"
                      value={dia.valor}
                      className="peer sr-only"
                      {...register('dias')}
                    />
                    <span className="flex justify-center rounded-lg border border-slate-300 px-2 py-2 text-sm transition peer-checked:border-indigo-600 peer-checked:bg-indigo-600 peer-checked:text-white peer-focus-visible:ring-2 peer-focus-visible:ring-indigo-300">
                      {dia.rotulo}
                    </span>
                  </label>
                ))}
              </div>
              {errors.dias && (
                <p className="mt-2 text-sm text-red-600">{errors.dias.message}</p>
              )}
            </fieldset>

            <div className="mt-6 grid grid-cols-2 gap-3">
              <label className="text-sm font-medium">
                Início
                <input
                  type="time"
                  className="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
                  {...register('inicioFaixa')}
                />
                {errors.inicioFaixa && (
                  <span className="mt-1 block text-sm text-red-600">
                    {errors.inicioFaixa.message}
                  </span>
                )}
              </label>

              <label className="text-sm font-medium">
                Fim
                <input
                  type="time"
                  className="mt-2 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
                  {...register('fimFaixa')}
                />
                {errors.fimFaixa && (
                  <span className="mt-1 block text-sm text-red-600">
                    {errors.fimFaixa.message}
                  </span>
                )}
              </label>
            </div>

            <fieldset className="mt-6">
              <legend className="text-sm font-medium">Duração da aula</legend>
              <div className="mt-2 grid grid-cols-3 gap-2">
                {[30, 60, 90].map((duracao) => (
                  <label key={duracao} className="cursor-pointer">
                    <input
                      type="radio"
                      value={duracao}
                      defaultChecked={duracao === 60}
                      className="peer sr-only"
                      {...register('duracaoMinutos')}
                    />
                    <span className="flex justify-center rounded-lg border border-slate-300 px-2 py-2 text-sm transition peer-checked:border-indigo-600 peer-checked:bg-indigo-50 peer-checked:text-indigo-700 peer-focus-visible:ring-2 peer-focus-visible:ring-indigo-300">
                      {duracao} min
                    </span>
                  </label>
                ))}
              </div>
              {errors.duracaoMinutos && (
                <p className="mt-2 text-sm text-red-600">
                  {errors.duracaoMinutos.message}
                </p>
              )}
            </fieldset>

            <button
              type="submit"
              disabled={isSubmitting}
              className="cursor-pointer mt-7 w-full rounded-lg bg-indigo-600 px-4 py-3 font-semibold text-white transition hover:bg-indigo-700 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {isSubmitting ? 'Buscando...' : 'Buscar professores'}
            </button>
          </form>

          <section aria-live="polite">
            {erro && (
              <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-red-700">
                {erro}
              </div>
            )}

            {!resultado && !erro && (
              <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-500">
                Preencha os filtros e faça uma busca para ver os resultados.
              </div>
            )}

            {resultado && (
              <div className="space-y-8">
                <div>
                  <div className="mb-4 flex items-center justify-between gap-4">
                    <h2 className="text-xl font-semibold">Professores encontrados</h2>
                    <span className="rounded-full bg-indigo-100 px-3 py-1 text-sm font-medium text-indigo-700">
                      {resultado.professores.length}
                    </span>
                  </div>

                  {resultado.professores.length === 0 ? (
                    <div className="rounded-xl border border-amber-200 bg-amber-50 p-5 text-amber-800">
                      Nenhum professor disponível para essa combinação.
                    </div>
                  ) : (
                    <div className="grid gap-4 sm:grid-cols-2">
                      {resultado.professores.map((professor) => (
                        <article
                          key={professor.id}
                          className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm"
                        >
                          <h3 className="font-semibold">{professor.nome}</h3>
                          <p className="mt-1 text-sm text-slate-500">
                            {professor.aulasNaSemana}{' '}
                            {professor.aulasNaSemana === 1 ? 'aula' : 'aulas'} na semana
                          </p>
                          <div className="mt-4 flex flex-wrap gap-2">
                            {professor.horarios.map((horario) => (
                              <span
                                key={horario}
                                className="rounded-full bg-emerald-50 px-3 py-1 text-sm font-medium text-emerald-700"
                              >
                                {horario}
                              </span>
                            ))}
                          </div>
                        </article>
                      ))}
                    </div>
                  )}
                </div>

                {resultado.naoElegiveis.length > 0 && (
                  <div>
                    <h2 className="mb-4 text-lg font-semibold text-slate-700">
                      Professores não elegíveis
                    </h2>
                    <div className="divide-y divide-slate-200 rounded-xl border border-slate-200 bg-white">
                      {resultado.naoElegiveis.map((professor) => (
                        <div
                          key={professor.id}
                          className="flex flex-col justify-between gap-1 px-5 py-4 sm:flex-row sm:items-center"
                        >
                          <span className="font-medium">{professor.nome}</span>
                          <span className="text-sm text-slate-500">
                            {professor.motivo}
                          </span>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            )}
          </section>
        </div>
      </div>
    </main>
  )
}

export default App
