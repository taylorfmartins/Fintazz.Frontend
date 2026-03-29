- É o cadastro de uma conta bancária vinculada a um [[Grupos - Residência - Família]] (ex: Nubank, Itaú, Caixa)
- Uma conta bancária deverá ter os seguintes dados:
	- ID do [[Grupos - Residência - Família]]
	- Nome da conta (ex: Nubank, Itaú Corrente)
	- Saldo Inicial — valor que a conta já possuía no momento do cadastro

## Regras de Negócio

- Uma conta bancária sempre pertence a um [[Grupos - Residência - Família]] — todos os membros do grupo podem visualizá-la
- O **Saldo Atual** parte do Saldo Inicial e é atualizado a cada [[Transação]] efetivada (`Pago = true`) registrada na conta
	- Receitas somam ao saldo
	- Despesas subtraem do saldo
- Transações previstas (`Pago = false`) **não** afetam o Saldo Atual
- O saldo pode ficar negativo — o sistema não bloqueia lançamentos por limite de saldo
- Faturas de [[Cartão de Crédito]] **não** afetam o saldo da conta automaticamente — o pagamento é iniciado pelo endpoint do cartão, que debita a conta e marca as parcelas como pagas

## Saldo Inicial

- Representa o saldo que a conta já tinha antes de começar a ser gerenciada pelo Fintazz
- É atribuído diretamente ao Saldo Atual no momento do cadastro — a conta já inicia com o valor real, sem precisar de nenhuma transação
- Pode ser alterado posteriormente via edição da conta, o que recalcula o Saldo Atual proporcionalmente

## Endpoints

- `POST /api/bank-accounts` — Cadastra uma nova conta bancária
- `PUT /api/bank-accounts/{id}` — Edita o nome, Saldo Inicial e/ou faz ajuste manual do Saldo Atual
- `DELETE /api/bank-accounts/{id}` — Remove a conta e todas as [[Transação]] vinculadas a ela em cascata
- `GET /api/bank-accounts/house-hold/{houseHoldId}` — Lista todas as contas e seus saldos atuais de um grupo familiar


## Edição de Conta Bancária

Os seguintes campos podem ser alterados:

- **Nome** — apenas renomeia a conta, sem impacto no saldo
- **Saldo Inicial** — atualiza o valor de origem da conta e recalcula o Saldo Atual
- **Saldo Atual (ajuste manual)** — permite corrigir o saldo diretamente caso haja alguma divergência com o extrato real do banco

## Exclusão de Conta Bancária

- A exclusão é **permanente e irreversível**
- Todas as [[Transação]] vinculadas à conta são excluídas em cascata
- [[Transações Recorrentes]] que apontam para esta conta são desativadas automaticamente

## Relação com outros módulos

- [[Transação]] — receitas e despesas são sempre vinculadas a uma conta e impactam seu saldo
- [[Transações Recorrentes]] — débitos automáticos recorrentes são descontados da conta vinculada pelo Worker
- [[Cartão de Crédito]] — o pagamento de fatura é iniciado pelo cartão e debita automaticamente a conta bancária escolhida
- **Dashboard** — o balanço mensal consolida o saldo de todas as contas do grupo em `BankAccountsTotalBalance`

## Situação Atual no Código

- Apenas criação e listagem estão implementadas — edição e exclusão são pendentes
- Não existe proteção contra saldo negativo — pode ser uma regra a avaliar no futuro
