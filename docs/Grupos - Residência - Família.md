- É o núcleo central do sistema — todos os dados financeiros (contas, cartões, transações) pertencem a um grupo familiar
- Representa uma residência, família ou qualquer agrupamento de pessoas que compartilham finanças
- Um grupo deverá ter os seguintes dados:
    - Nome do grupo (ex: Família Silva, Apartamento Centro)
    - Administrador — o [[Cadastro de Usuário]] que criou o grupo

## Membros

- Um [[Cadastro de Usuário]] pode participar de quantos grupos quiser, sem limite
- Todo usuário que cria um grupo torna-se automaticamente seu **Administrador**
- Membros comuns podem visualizar e lançar dados no grupo, mas não podem gerenciar membros ou excluir o grupo
- Apenas o Administrador pode:
    - Convidar novos membros
    - Remover membros do grupo
    - Editar o nome do grupo
    - Excluir o grupo

## Convites

- O Administrador convida um novo membro pelo e-mail via `POST /api/house-holds/{id}/invites`
- O sistema gera um token de convite com validade de **72 horas**
- O convidado precisa ter uma conta cadastrada no sistema para aceitar o convite
- O convite é aceito via `POST /api/house-holds/invites/{token}/accept` pelo usuário autenticado
- Um convite só pode ser aceito uma vez e expira após o prazo
- Não é possível convidar um usuário que já é membro do grupo

## Endpoints

- `POST /api/house-holds` — Cria um novo grupo familiar
- `PUT /api/house-holds/{id}` — Edita o nome do grupo (apenas Administrador)
- `DELETE /api/house-holds/{id}` — Remove o grupo e todos os dados vinculados em cascata (apenas Administrador)
- `GET /api/house-holds` — Lista todos os grupos do sistema
- `GET /api/house-holds/{id}/members` — Lista os membros de um grupo
- `DELETE /api/house-holds/{id}/members/{userId}` — Remove um membro do grupo (apenas Administrador)
- `POST /api/house-holds/{id}/invites` — Envia um convite para um e-mail (apenas Administrador)
- `POST /api/house-holds/invites/{token}/accept` — Aceita um convite e ingressa no grupo

## Exclusão do Grupo

- A exclusão é **permanente e irreversível** e só pode ser feita pelo Administrador
- Todos os dados vinculados ao grupo são excluídos em cascata:
    - [[Contas Bancária]] e suas [[Transação]]
    - [[Cartão de Crédito]] e suas compras e parcelas
    - [[Transações Recorrentes]]
- Os [[Cadastro de Usuário]] membros **não** são excluídos — apenas desvinculados do grupo

## Relação com outros módulos

- [[Cadastro de Usuário]] — um usuário pode ser membro de vários grupos e só acessa dados dos grupos aos quais pertence
- [[Contas Bancária]] — sempre vinculadas a um grupo
- [[Cartão de Crédito]] — sempre vinculados a um grupo
- [[Transação]] — sempre vinculadas a um grupo
- [[Transações Recorrentes]] — sempre vinculadas a um grupo

## Situação Atual no Código

- Apenas criação e listagem geral estão implementadas
- Não existe ainda o conceito de Administrador nem controle de permissões — todos os endpoints são abertos
- Convites, listagem de membros, edição e exclusão do grupo são pendentes
- A implementação de autenticação via [[Cadastro de Usuário]] é pré-requisito para que as regras de Administrador e controle de acesso por grupo funcionem