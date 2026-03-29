- Será o lançamento de um valor financeiro, podendo ser receita ou despesa, vinculado a uma [[Contas Bancária]]
- Uma transação deverá ter os seguintes dados:
    - ID do [[Grupos - Residência - Família]]
    - ID da [[Contas Bancária]] afetada
    - Descrição (ex: Salário Mês, Compra Mercado)
    - Valor (sempre positivo — o Tipo determina se soma ou subtrai do saldo)
    - Data (quando a transação ocorreu ou ocorrerá)
    - Tipo (Receita ou Despesa)
    - Pago (sim/não) — indica se o valor já foi efetivado na conta real
    - [[Categoria]]
    - Usuário que realizou o lançamento
- Ao lançar uma transação, ela pode ser cancelada/excluída

## Tipos de Transação

- **Receita (`Income`)** — entrada de valor na conta (ex: salário, freelance, transferência recebida)
- **Despesa (`Expense`)** — saída de valor da conta (ex: compra no mercado, conta de luz)
- **Assinatura (`Subscription`)** — tipo reservado para uso futuro, possui fluxo separado via [[Transações Recorrentes]]

## Regras de Negócio

- O valor deve ser sempre maior que zero — o campo Tipo é quem define se soma ou subtrai
- A [[Contas Bancária]] informada deve pertencer ao mesmo [[Grupos - Residência - Família]] da transação
- O campo **Pago** controla o impacto no saldo da conta:
    - `Pago = true` — a transação já aconteceu e o saldo da [[Contas Bancária]] é atualizado imediatamente
    - `Pago = false` — é uma previsão futura, o saldo **não** é alterado até ser efetivada
- Receitas somam ao saldo da conta; Despesas subtraem do saldo da conta
- Transações do tipo Assinatura não podem ser criadas por este fluxo — são geradas automaticamente pelo Worker via [[Transações Recorrentes]]

## Impacto no Saldo

O ajuste de saldo é processado pelo `BalanceEngine` (Domain Service) no momento do lançamento:

```
Pago = true  + Receita  → CurrentBalance + Valor
Pago = true  + Despesa  → CurrentBalance - Valor
Pago = false            → Saldo não é alterado
```

## Endpoints

- `POST /api/transactions` — Registra uma nova transação (receita ou despesa)
- `DELETE /api/transactions/{id}` — Deleta uma transação
- `PATCH /api/transactions/{id}` — Atualiza uma transação
- `GET /api/transactions/house-hold/{houseHoldId}` — Lista o extrato de transações de um grupo familiar
    - Parâmetros opcionais: `startDate` e `endDate` para filtrar por período

## Consulta de Extrato

- O extrato retorna todas as transações de todas as contas do grupo no período informado
- Os resultados são ordenados por data de forma decrescente (mais recentes primeiro)
- Sem filtro de data, retorna todas as transações do grupo sem limite de período

## Situação Atual no Código

- O campo [[Categoria]] é atualmente um `string?` livre — será substituído por uma referência ao `Id` da entidade `Category` quando o módulo de [[Categoria]] for implementado
- Ainda não existe endpoint para marcar uma transação prevista (`Pago = false`) como efetivada (`Pago = true`) — está listado como pendente no [[Backend]]
- Ainda não existe endpoint de exclusão de transação — está indicado na especificação original mas ainda não foi implementado