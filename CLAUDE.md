## Visão Geral

Frontend da aplicação Fintazz — sistema de gestão financeira familiar. Consome a API REST do backend Fintazz e é distribuído como PWA (Progressive Web App), permitindo instalação no celular e uso offline básico.

## Documentação do Produto

A especificação de negócio de cada módulo está em `/docs`. Antes de implementar qualquer tela ou funcionalidade, leia o arquivo correspondente:

|Módulo|Arquivo|
|---|---|
|Grupos Familiares|`/docs/Grupos - Residência - Família.md`|
|Cadastro de Usuário e Autenticação|`/docs/Cadastro de Usuário.md`|
|Contas Bancárias|`/docs/Contas Bancária.md`|
|Cartão de Crédito|`/docs/Cartão de Crédito.md`|
|Transações|`/docs/Transação.md`|
|Transações Recorrentes|`/docs/Transações Recorrentes.md`|
|Categorias|`/docs/Categoria.md`|

## Stack

- .NET 10 / C# / Blazor WebAssembly + PWA
- MudBlazor — componentes de UI
- Idioma da interface: Português (pt-BR)

## Projetos da Solution

```
Fintazz.Frontend.slnx
├── Fintazz.Web             → Projeto Blazor WebAssembly (PWA), páginas, componentes e entry point
├── Fintazz.Web.Core        → Interfaces, modelos (requests/responses), enums e contratos compartilhados
├── Fintazz.Web.Services    → Implementação dos serviços HTTP que consomem a API do backend
```

Regra de dependência: `Core ← Services ← Web`

## Configuração da API

A URL base da API é lida de uma variável de ambiente. Em `wwwroot/appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": ""
  }
}
```

Em desenvolvimento, usar `wwwroot/appsettings.Development.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7000"
  }
}
```

O `HttpClient` deve ser registrado com a `BaseUrl` lida de `ApiSettings` via `IConfiguration`. Nunca hardcodar a URL da API no código.

## Autenticação JWT

O fluxo de autenticação usa JWT com refresh token automático:

1. O usuário faz login — a API retorna `AccessToken` e `RefreshToken`
2. Ambos são armazenados no `LocalStorage`
3. O `AccessToken` é injetado automaticamente em todas as requisições via `AuthorizationMessageHandler`
4. Quando uma requisição retorna `401`, o handler tenta renovar o token via endpoint de refresh automaticamente antes de redirecionar para o login
5. O estado de autenticação é gerenciado por um `AuthenticationStateProvider` customizado

### Arquivos de autenticação

```
Fintazz.Web/
└── Auth/
    ├── FintazzAuthStateProvider.cs   → AuthenticationStateProvider customizado
    ├── AuthorizationMessageHandler.cs → Intercepta requisições e renova token
    └── TokenStorageService.cs        → Lê e escreve tokens no LocalStorage
```

## Organização de Pastas

O projeto `Fintazz.Web` é organizado de forma mista — por módulo com subpastas por tipo:

```
Fintazz.Web/
├── Auth/                         → Autenticação (ver seção acima)
├── Layout/                       → MainLayout, NavMenu, componentes estruturais
├── Modules/
│   ├── HouseHolds/
│   │   ├── Pages/                → Páginas Blazor (.razor) do módulo
│   │   └── Components/           → Componentes reutilizáveis do módulo
│   ├── BankAccounts/
│   │   ├── Pages/
│   │   └── Components/
│   ├── CreditCards/
│   │   ├── Pages/
│   │   └── Components/
│   ├── Transactions/
│   │   ├── Pages/
│   │   └── Components/
│   ├── RecurringCharges/
│   │   ├── Pages/
│   │   └── Components/
│   ├── Categories/
│   │   ├── Pages/
│   │   └── Components/
│   └── Dashboard/
│       ├── Pages/
│       └── Components/
└── Shared/                       → Componentes globais reutilizáveis entre módulos

Fintazz.Web.Core/
└── Modules/
    ├── HouseHolds/
    │   ├── Requests/             → Records de request para a API
    │   └── Responses/            → Records de response da API
    ├── BankAccounts/
    │   ├── Requests/
    │   └── Responses/
    └── ...                       → Mesmo padrão para cada módulo

Fintazz.Web.Services/
└── Modules/
    ├── HouseHolds/
    │   └── HouseHoldService.cs   → Implementa IHouseHoldService
    ├── BankAccounts/
    │   └── BankAccountService.cs
    └── ...                       → Mesmo padrão para cada módulo
```

## Padrões de Código

### Serviços HTTP

Cada módulo tem uma interface em `Fintazz.Web.Core` e uma implementação em `Fintazz.Web.Services`:

```csharp
// Fintazz.Web.Core/Modules/BankAccounts/IBankAccountService.cs
public interface IBankAccountService
{
    Task<List<BankAccountResponse>> GetByHouseHoldAsync(Guid houseHoldId);
    Task<Result> CreateAsync(CreateBankAccountRequest request);
}

// Fintazz.Web.Services/Modules/BankAccounts/BankAccountService.cs
public class BankAccountService(HttpClient httpClient) : IBankAccountService
{
    public async Task<List<BankAccountResponse>> GetByHouseHoldAsync(Guid houseHoldId)
    {
        return await httpClient.GetFromJsonAsync<List<BankAccountResponse>>(
            $"api/bank-accounts/house-hold/{houseHoldId}") ?? [];
    }
}
```

### Páginas Blazor

- Sempre usar `@inject` para injetar serviços — nunca instanciar diretamente
- Estados de carregamento devem usar `MudProgressCircular` ou `MudSkeleton`
- Erros de API devem ser exibidos com `MudSnackbar`
- Formulários devem usar `MudForm` com validação antes de chamar a API
- Usar `@page` com rotas em português e kebab-case (ex: `@page "/contas-bancarias"`)

### Tratamento de Erros da API

A API retorna erros no formato `{ code: string, message: string }`. Sempre tratar e exibir a mensagem ao usuário via `ISnackbar`:

```csharp
var result = await BankAccountService.CreateAsync(request);
if (!result.IsSuccess)
{
    Snackbar.Add(result.Error.Message, Severity.Error);
    return;
}
Snackbar.Add("Conta criada com sucesso!", Severity.Success);
```

## Módulos Implementados

- **Autenticação** ✅ — login, cadastro com auto-login, refresh token automático
- **Dashboard** ✅ — balanço mensal consolidado
- **Grupos Familiares** ✅ — criação, convites, gestão de membros
- **Contas Bancárias** ✅ — CRUD completo
- **Cartões de Crédito** ✅ — CRUD completo, compras com categoria, edição de compra, marcar parcela como paga, fatura mensal e pagamento
- **Transações** ✅ — lançamento de receitas e despesas, extrato paginado, marcar como paga, excluir
- **Transações Recorrentes** ✅ — cadastro, aprovação manual, reativação, gestão
- **Categorias** ✅ — CRUD completo com subcategorias e categorias de sistema

## Melhorias Pendentes

- **Grupos Familiares**
	- Ao enviar e-mail com o convite do grupo para o usuário, ao clicar em entrar no grupo no link do e-mail, irá trazer o usuário para o sistema: precisa fazer cadastro se não tiver conta ou entrar no grupo automaticamente se já autenticado.

## Melhorias Implementadas

- **Tela de Registros** ✅
	- Ao apertar Enter no campo senha, o formulário de cadastro é enviado
	- Após registrar, o login é feito automaticamente e o usuário é redirecionado para o dashboard

- **Grupos Familiares** ✅
	- Convite não aceita o e-mail do dono do grupo — erro tratado pelo backend (`HouseHold.CannotInviteYourself`)
	- Verificação de membro já no grupo antes de enviar convite — erro tratado pelo backend (`HouseHold.AlreadyMember`)

- **Compras Cartão de Crédito** ✅
	- Seleção de categoria de despesas ao registrar compra
	- Edição de compra existente — descrição e categoria
	- Marcar parcela individual como paga diretamente na fatura

- **Categorias** ✅
	- Subcategorias: registradas dentro de uma categoria pai; botão "Nova Subcategoria" em cada linha
	- Categorias de sistema exibem badge "Sistema" e não podem ser editadas ou excluídas
	- Categorias agrupadas por tipo (Despesas / Receitas) com subcategorias indentadas