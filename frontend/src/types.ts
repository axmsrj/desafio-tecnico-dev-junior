export type ProfessorElegivel = {
  id: number
  nome: string
  aulasNaSemana: number
  horarios: string[]
}

export type ProfessorNaoElegivel = {
  id: number
  nome: string
  motivo: string
}

export type BuscarProfessoresResponse = {
  professores: ProfessorElegivel[]
  naoElegiveis: ProfessorNaoElegivel[]
}
