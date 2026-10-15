import { z } from 'zod'

const horarioRegex = /^([01]\d|2[0-3]):[0-5]\d$/

export const buscaSchema = z
  .object({
    idioma: z.enum(['Inglês', 'Espanhol', 'Francês']),
    dias: z
      .array(
        z.enum([
          'segunda',
          'terca',
          'quarta',
          'quinta',
          'sexta',
          'sabado',
          'domingo',
        ]),
      )
      .min(1, 'Selecione pelo menos um dia.'),
    inicioFaixa: z
      .string()
      .regex(horarioRegex, 'Informe um horário inicial válido.'),
    fimFaixa: z
      .string()
      .regex(horarioRegex, 'Informe um horário final válido.'),
    duracaoMinutos: z
      .coerce.number<number>()
      .refine((duracao) => [30, 60, 90].includes(duracao), {
        message: 'Escolha uma duração válida.',
      }),
  })
  .refine((dados) => dados.fimFaixa > dados.inicioFaixa, {
    message: 'O horário final deve ser posterior ao inicial.',
    path: ['fimFaixa'],
  })

export type BuscaFormData = z.infer<typeof buscaSchema>
