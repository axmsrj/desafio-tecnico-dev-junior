## Solução implementada

A solução é composta por uma API em ASP.NET Core com .NET 8 e uma interface em React com TypeScript.

A API carrega os professores do arquivo `professores.json`, executa as regras de disponibilidade e retorna os professores elegíveis, seus possíveis horários e os motivos de exclusão dos demais professores.

## Tecnologias utilizadas

### Backend

- C#
- .NET 8
- ASP.NET Core Minimal APIs
- System.Text.Json
- xUnit

### Frontend

- React
- TypeScript
- Vite
- Tailwind CSS
- React Hook Form
- Zod

## Estrutura do projeto

```text
src/
├── EncontreUmProfessor.Api/
│   ├── Endpoints/
│   └── Validation/
├── EncontreUmProfessor.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── EncontreUmProfessor.Domain/
│   └── Entities/
└── EncontreUmProfessor.Infrastructure/
    └── Data/

tests/
└── EncontreUmProfessor.Tests/

frontend/
├── src/
│   ├── schemas/
│   ├── services/
│   └── App.tsx
└── package.json
```

Responsabilidades das camadas:

- `Domain`: entidades centrais do sistema.
- `Application`: contratos, interfaces e regras da busca.
- `Infrastructure`: leitura dos professores a partir do arquivo JSON.
- `Api`: configuração HTTP, endpoint e validação da requisição.
- `Tests`: testes automatizados das regras de negócio.
- `frontend`: formulário e apresentação dos resultados.

## Pré-requisitos

Para executar o projeto, é necessário ter instalado:

- SDK do .NET 8.
- Node.js.
- npm.

## Como executar a API

Na raiz do repositório:

```bash
dotnet restore
dotnet run --project src/EncontreUmProfessor.Api --launch-profile http
```

A API será iniciada em:

```text
http://localhost:5076
```

Para verificar se ela está funcionando:

```text
GET http://localhost:5076/health
```

## Como executar o frontend

Em outro terminal:

```bash
cd frontend
npm install
npm run dev
```

O frontend será iniciado em:

```text
http://localhost:5173
```

Durante o desenvolvimento, o Vite encaminha as chamadas de `/api` para a API em `http://localhost:5076`.

Para utilizar outro endereço de API, copie o arquivo de exemplo:

```bash
cp .env.example .env
```

Depois, altere a variável:

```env
VITE_API_URL=http://localhost:5076
```

## Como executar os testes

Na raiz do repositório:

```bash
dotnet test
```

Os testes exercitam diretamente o serviço de busca por meio de um repositório em memória. Portanto, não dependem da API em execução nem da leitura do arquivo JSON.

Os casos testados incluem:

- Professor que não ensina o idioma.
- Professor com agenda bloqueada.
- Horário fixo disponível em todos os dias solicitados.
- Conflito com aulas existentes.
- Aulas que apenas encostam em outras aulas.
- Aula inteira dentro da disponibilidade.
- Inícios em hora cheia ou meia hora.
- Ordenação por quantidade de aulas e nome.
- Ausência de horário em comum.
- Proteção contra horários que atravessam a meia-noite.

## Exemplo de requisição

```bash
curl -X POST http://localhost:5076/api/professores/busca \
  -H "Content-Type: application/json" \
  -d '{
    "idioma": "Espanhol",
    "dias": ["segunda"],
    "inicioFaixa": "18:00",
    "fimFaixa": "20:00",
    "duracaoMinutos": 60
  }'
```

## Decisões tomadas

- A regra de negócio foi mantida fora da camada HTTP para permitir testes sem iniciar a API.
- A camada de aplicação depende de `IProfessorRepository`, sem conhecer a origem dos dados.
- A implementação atual do repositório lê os professores do arquivo `professores.json`.
- O arquivo JSON permanece na raiz do repositório e é copiado para `Data/professores.json` na saída da API durante o build.
- Comparações de idiomas e dias não diferenciam letras maiúsculas de minúsculas.
- O mesmo horário de início precisa estar disponível em todos os dias solicitados.
- Os horários possíveis são gerados em intervalos de 30 minutos.
- Quando o início da faixa não está em `:00` ou `:30`, ele é arredondado para o próximo horário permitido.
- O arredondamento não pode atravessar a meia-noite.
- Duas aulas podem encostar; isso não é considerado sobreposição.
- A API é responsável pela validação definitiva da requisição.
- O frontend também valida os campos para apresentar feedback mais rápido ao usuário.
- Professores não elegíveis são retornados com o primeiro motivo aplicável na ordem definida pelo desafio.

## Estados tratados no frontend

A interface contempla:

- Estado inicial.
- Carregamento durante a busca.
- Professores encontrados.
- Nenhum professor encontrado.
- Erros de preenchimento do formulário.
- Erros de validação devolvidos pela API.
- Falha de comunicação com a API.
- Exibição dos professores não elegíveis e seus motivos.

## O que ficou faltando

Todos os requisitos obrigatórios do desafio foram implementados.

Como melhorias futuras, poderiam ser adicionados:

- Testes automatizados de integração do endpoint.
- Testes automatizados dos componentes do frontend.
- Documentação OpenAPI/Swagger.
- Persistência em banco de dados.