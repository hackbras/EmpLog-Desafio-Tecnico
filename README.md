# EmpLog

API de autocadastro de motoristas. O único endpoint é `POST /api/v1/motoristas`.

## Como executar

Na raiz desta pasta:

```bash
dotnet test
dotnet run --project src/EmpLog.Api --launch-profile http
```

O perfil `http` sobe em `http://localhost:5067`. No ambiente Development, o Swagger fica em `/swagger`.

O banco SQLite é o arquivo `src/EmpLog.Api/emplog.db`. Na subida, a API aplica as migrations pendentes.

## Por que o projeto está dividido assim

São quatro camadas, cada uma com uma responsabilidade:

- **Domain** guarda o motorista, os value objects (CPF, e-mail, telefone e CNH) e as regras que não dependem de banco nem de HTTP. Não referencia nenhum outro projeto.
- **Application** contém o caso de uso `CadastrarMotoristaHandler`. Ela valida a entrada com FluentValidation, aplica as regras de negócio e devolve um `Result`.
- **Infrastructure** implementa o repositório e o Unit of Work com EF Core e SQLite. A tabela `Motoristas` tem índice único em CPF e no número da CNH.
- **Api** expõe o controller, traduz o `Result` para HTTP e devolve `ProblemDetails` (RFC 7807) quando ocorre um erro não previsto.

O cadastro é um caso de uso chamado direto pelo controller. A pasta `Queries` fica reservada para uma consulta futura.

O `Result` carrega sucesso ou a lista de erros. Validação de entrada e duplicidade de CPF ou CNH voltam nesse objeto, não como exceção. `DomainException` existe só para estado inválido dentro do agregado. Se duas requisições passarem juntas pela consulta de duplicidade, a violação do índice único também volta como conflito no `Result`.

A CNH precisa estar válida depois da data UTC de hoje. A categoria aceita é A, B, C, D, E, AB, AC, AD ou AE. O motorista nasce com status `Ativo`.

## Exemplos

Cadastro aceito, `201 Created`, com `Location: /api/v1/motoristas/{id}`:

```json
{
  "id": "3f1c2b7e-8a4d-4e1a-9c2b-1d5e6f7a8b9c",
  "nomeCompleto": "Ana Silva",
  "cpf": "52998224725",
  "email": "ana.silva@email.com",
  "telefone": "11988887777",
  "cnh": {
    "numero": "12345678900",
    "categoria": "B",
    "validade": "2030-06-15"
  },
  "status": "Ativo",
  "criadoEmUtc": "2026-10-12T15:00:00Z"
}
```

Entrada inválida, `400`:

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Requisição inválida",
  "status": 400,
  "errors": {
    "cpf": ["CPF inválido."],
    "cnh.validade": ["A validade da CNH deve ser posterior à data de hoje."]
  }
}
```

CPF ou CNH já cadastrados, `409`:

```json
{
  "type": "https://httpstatuses.com/409",
  "title": "Conflito",
  "status": 409,
  "detail": "Já existe um motorista com este CPF."
}
```
# EmpLog-Desafio-T-cnico
