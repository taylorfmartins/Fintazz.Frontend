- Precisamos cadastrar usuários, eles precisam fornecer os seguintes dados
    - Nome Completo
    - Apelido
    - E-mail (único no sistema, utilizado para login)
    - Data de Nascimento
    - Senha (mínimo 8 caracteres, armazenada com hash BCrypt)
- O usuário pode criar seu próprio [[Grupos - Residência - Família]]
- O usuário pode convidar alguém para seu [[Grupos - Residência - Família]]
- O usuário pode ser convidado para um [[Grupos - Residência - Família]]
- Ao participar de uma residência, o usuário poderá ver:
    - [[Cartão de Crédito]] cadastrados
    - [[Contas Bancária]] cadastradas
    - [[Transação]] realizadas
    - [[Transações Recorrentes]] definidas
- Para utilizar o sistema, o usuário deverá se autenticar usando seu e-mail e senha

## Autenticação

- O sistema utilizará **JWT (JSON Web Token)** para autenticação stateless
- O login é feito via e-mail e senha
- A API retorna um `AccessToken` (JWT de curta duração) e um `RefreshToken`
- O cliente envia o `AccessToken` no header `Authorization: Bearer {token}` em todas as requisições protegidas
- Quando o `AccessToken` expirar, o cliente usa o `RefreshToken` para obter um novo par de tokens sem refazer o login

### Endpoints de Autenticação

- `POST /api/auth/register` — Cadastra um novo usuário
- `POST /api/auth/login` — Autentica e retorna os tokens JWT
- `POST /api/auth/refresh` — Renova o AccessToken usando o RefreshToken

## Convites para [[Grupos - Residência - Família]]

- Um membro existente do grupo pode convidar alguém pelo e-mail via `POST /api/house-holds/{id}/invites`
- O sistema gera um token de convite com validade de **72 horas**
- O convidado acessa `POST /api/house-holds/invites/{token}/accept` (autenticado) para ingressar no grupo
- Um convite só pode ser aceito uma vez e expira após o prazo

### Dados do Convite

- ID do [[Grupos - Residência - Família]]
- E-mail do convidado
- Token de convite (gerado aleatoriamente)
- Data de expiração
- Data de aceite (vazio se ainda não aceito)
- Usuário que realizou o convite

## Autorização nas Rotas

- Todas as rotas do sistema exigem autenticação (token JWT válido)
- O usuário só pode acessar dados de [[Grupos - Residência - Família]] dos quais é membro
- Tentativas de acessar dados de outros grupos devem ser bloqueadas pela API

## Pacotes Necessários

- `BCrypt.Net-Next` — hashing seguro de senhas
- `System.IdentityModel.Tokens.Jwt` — geração e validação de JWT
- `Microsoft.AspNetCore.Authentication.JwtBearer` — middleware de autenticação na API