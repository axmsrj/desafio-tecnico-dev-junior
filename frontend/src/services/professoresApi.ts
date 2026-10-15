import type { BuscaFormData } from '../schemas/buscaSchema'
import type { BuscarProfessoresResponse } from '../types'

const apiUrl = import.meta.env.VITE_API_URL ?? ''

export async function buscarProfessores(
  filtros: BuscaFormData,
): Promise<BuscarProfessoresResponse> {
  const response = await fetch(`${apiUrl}/api/professores/busca`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(filtros),
  })

  if (!response.ok) {
    const erro = (await response.json().catch(() => null)) as {
      mensagem?: string
    } | null

    throw new Error(erro?.mensagem ?? 'Não foi possível realizar a busca.')
  }

  return (await response.json()) as BuscarProfessoresResponse
}
