- É um cadastro de cobrança que se repete todo mês automaticamente, podendo ser um débito em [[Contas Bancária]] ou uma assinatura em [[Cartão de Crédito]]
- Uma transação recorrente deverá ter os seguintes dados:
    - ID do [[Grupos - Residência - Família]]
    - Descrição (ex: Netflix, Conta de Luz, Aluguel)
    - Valor
    - Dia de cobrança (1 a 31) — dia do mês em que a cobrança será gerada
    - [[Categoria]]
    - [[Contas Bancária]] (obrigatório se for um débito automático)
    - [[Cartão de Crédito]] (obrigatório se for uma assinatura no cartão)
    - Valor Variável (sim/não) — indica se o valor pode mudar mês a mês (ex: conta de água, energia)
    - Lançar Automaticamente (sim/não) — indica se o Worker deve gerar a transação sem precisar de aprovação manual
    - Usuário que realizou o cadastro

## Regras de Negócio

- É obrigatório informar **ou** uma [[Contas Bancária]] **ou** um [[Cartão de Crédito]] — nunca os dois ao mesmo tempo
- A [[Contas Bancária]] ou o [[Cartão de Crédito]] informado deve pertencer ao mesmo [[Grupos - Residência - Família]]
- Ao ser cadastrada, a recorrente fica com status **ativa** automaticamente
- Uma recorrente desativada não é processada pelo Worker — pode ser reativada futuramente
- O cancelamento é um **soft delete** — a recorrente é desativada mas seu histórico de transações geradas é preservado
- Transações já geradas por uma recorrente **não** são afetadas pelo cancelamento ou edição dela

## Valor Variável

- Quando `Valor Variável = true`, o valor cadastrado serve apenas como referência
- O Worker **não** lança automaticamente cobranças com valor variável, independente do campo `Lançar Automaticamente` — aguarda aprovação manual com o valor real do mês
- Exemplos de uso: conta de água, energia elétrica, internet com franquia variável

## Lançamento Automático

- Quando `Lançar Automaticamente = true`, o Worker processa a cobrança todo mês no dia definido sem intervenção humana
- Quando `Lançar Automaticamente = false`, o Worker identifica a cobrança mas aguarda aprovação manual de qualquer membro do grupo antes de gerar a transação
- Cobranças com `Valor Variável = true` sempre aguardam aprovação manual, independente deste campo

## Processamento pelo Worker

O Worker (`ProcessRecurringChargesJob`) executa todo dia à meia-noite e 1 minuto (UTC) e processa as recorrentes cujo Dia de cobrança é igual ao dia atual:

- Se `Lançar Automaticamente = false` ou `Valor Variável = true` → registra a pendência e aguarda aprovação manual
- Se vinculada a [[Contas Bancária]] → cria uma [[Transação]] do tipo Despesa com `Pago = true`, debitando o saldo imediatamente
- Se vinculada a [[Cartão de Crédito]] → cria uma compra de 1 parcela no cartão, seguindo as regras normais de faturamento do `BillingEngine`

## Aprovação Manual

- Qualquer membro do [[Grupos - Residência - Família]] pode aprovar uma cobrança pendente
- Na aprovação, o membro pode ajustar o valor antes de confirmar — especialmente útil para cobranças com `Valor Variável = true`
- Após a aprovação, a transação é gerada normalmente seguindo o mesmo fluxo do Worker

## Endpoints

- `POST /api/recurring-charges` — Cadastra uma nova cobrança recorrente
- `PUT /api/recurring-charges/{id}` — Edita os campos descritivos da recorrente (descrição, valor e categoria)
- `DELETE /api/recurring-charges/{id}` — Desativa (soft delete) uma cobrança recorrente
- `PATCH /api/recurring-charges/{id}/reactivate` — Reativa uma cobrança recorrente desativada
- `GET /api/recurring-charges/house-hold/{houseHoldId}` — Lista todas as cobranças ativas de um grupo familiar
- `POST /api/recurring-charges/{id}/approve` — Aprova e lança manualmente uma cobrança pendente, com possibilidade de ajuste de valor

## Edição da Recorrente

Os seguintes campos podem ser alterados:

- **Descrição** — renomeia a cobrança sem impacto no histórico
- **Valor** — altera o valor de referência para os próximos lançamentos; não afeta transações já geradas
- **Categoria** — reagrupa a cobrança; não afeta transações já geradas

O meio de pagamento ([[Contas Bancária]] ou [[Cartão de Crédito]]) e o Dia de cobrança **não podem ser alterados** — para mudá-los é necessário cancelar a recorrente atual e cadastrar uma nova.

## Relação com outros módulos

- [[Grupos - Residência - Família]] — toda recorrente pertence a um grupo
- [[Contas Bancária]] — débitos automáticos geram uma [[Transação]] do tipo Despesa na conta vinculada
- [[Cartão de Crédito]] — assinaturas geram uma compra de 1 parcela no cartão vinculado
- [[Categoria]] — agrupa a recorrente junto com as demais transações do grupo
- [[Transação]] — cada execução mensal da recorrente gera uma transação independente

## Situação Atual no Código

- Cadastro, listagem de ativas e cancelamento (soft delete) estão implementados
- O Worker já processa automaticamente as recorrentes com `Lançar Automaticamente = true`
- Edição, reativação e aprovação manual são pendentes
- Cobranças com `Valor Variável = true` ainda não possuem tratamento diferenciado no Worker — são processadas da mesma forma que as fixas