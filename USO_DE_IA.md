# Uso de IA

## Ferramentas utilizadas

- ChatGPT Codex com o modelo GPT-5.6 Sol.

## Como as ferramentas foram usadas

Utilizei a IA como apoio durante o desenvolvimento para:

- Criar o esqueleto inicial dos projetos .NET 8 e React com TS.
- Auxiliar na criação e validação dos testes automatizados do backend.
- Auxiliar na implementação do validator da API.
- Desenho do layout do frontend.

Todo o código gerado foi revisado e testado antes de ser mantido no projeto.

## Caso em que a IA errou

Durante a implementação do formulário, a IA sugeriu utilizar `valueAsNumber` do React Hook Form para converter o valor dos botões de duração.

Essa solução não funcionou corretamente com os campos do tipo `radio`. Os valores enviados pelo HTML continuavam sendo textos, como `"30"`, enquanto o schema do Zod esperava um número.

Percebi o problema ao testar a opção de 30 minutos: o botão de busca aparentemente não fazia nada. Ao inspecionar o formulário, identifiquei a mensagem de validação:

```text
Invalid input: expected number, received string
```

A correção foi alterar o schema pra realizar a conversão de maneira explícita:

```ts
duracaoMinutos: z
  .coerce
  .number<number>()
  .refine((duracao) => [30, 60, 90].includes(duracao), {
    message: "Escolha uma duração válida.",
  })
```