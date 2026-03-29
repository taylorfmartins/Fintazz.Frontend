- É o cadastro de um cartão de crédito vinculado a um [[Grupos - Residência - Família]] (ex: Inter Black, Nubank Ultravioleta)
- Um cartão de crédito deverá ter os seguintes dados:
    - ID do [[Grupos - Residência - Família]]
    - Nome do cartão (ex: Inter Black, XP Visa)
    - Limite Total
    - Dia de Fechamento — dia do mês em que a fatura fecha
    - Dia de Vencimento — dia do mês em que a fatura deve ser paga

## Regras de Negócio

- Um cartão sempre pertence a um [[Grupos - Residência - Família]] — todos os membros do grupo podem visualizá-lo
- O **Limite Disponível** é calculado on-the-fly com base nas parcelas ainda não pagas de todas as compras do cartão
    - `Limite Disponível = Limite Total - Soma das parcelas pendentes`
- Não é possível registrar uma compra que ultrapasse o Limite Disponível atual
- O cartão não movimenta saldo de [[Contas Bancária]] diretamente — o impacto no saldo ocorre apenas quando a fatura é paga via `POST /api/credit-cards/{id}/invoice/pay`

## Dia de Fechamento e Faturamento

O Dia de Fechamento determina em qual fatura cada compra será cobrada:

- Se a compra foi feita **até** o Dia de Fechamento → cai na fatura do **mês atual**
- Se a compra foi feita **após** o Dia de Fechamento → cai na fatura do **mês seguinte**

Exemplo com fechamento no dia 10:

- Compra em 08/03 → fatura de Março
- Compra em 15/03 → fatura de Abril

Compras parceladas seguem a mesma regra para a primeira parcela — as demais são distribuídas nos meses subsequentes automaticamente pelo `BillingEngine`.

## Parcelamento

- Ao registrar uma compra, o número de parcelas deve ser informado
- O `BillingEngine` divide o valor total pelo número de parcelas e distribui nos meses corretos
- O ajuste de centavos (arredondamento) é aplicado na **primeira parcela** para garantir que a soma das parcelas seja exatamente igual ao valor total da compra
- Compras à vista devem ser informadas com `TotalInstallments = 1`

## Endpoints

- `POST /api/credit-cards` — Cadastra um novo cartão de crédito
- `PUT /api/credit-cards/{id}` — Edita os dados do cartão
- `DELETE /api/credit-cards/{id}` — Remove o cartão e todas as compras em cascata
- `GET /api/credit-cards/house-hold/{houseHoldId}` — Lista todos os cartões de um grupo familiar
- `POST /api/credit-cards/purchases` — Registra uma nova compra no cartão
- `DELETE /api/credit-cards/purchases/{purchaseId}` — Estorna (cancela) uma compra e remove todas as suas parcelas
- `GET /api/credit-cards/{creditCardId}/purchases` — Lista todas as compras de um cartão independente da fatura
- `GET /api/dashboards/credit-card/{creditCardId}/invoice` — Retorna o detalhamento da fatura de um mês/ano específico
- `POST /api/credit-cards/{id}/invoice/pay` — Paga a fatura de um mês/ano debitando automaticamente uma conta bancária

## Edição do Cartão

Os seguintes campos podem ser alterados:

- **Nome** — apenas renomeia o cartão, sem impacto nas faturas
- **Limite Total** — atualiza o limite e recalcula o Limite Disponível imediatamente
- **Dia de Fechamento** — altera a regra de faturamento para compras **futuras**; compras já registradas não são recalculadas
- **Dia de Vencimento** — altera apenas a referência de vencimento; não impacta o cálculo de parcelas

## Exclusão do Cartão

- A exclusão é **permanente e irreversível**
- Todas as compras vinculadas ao cartão são excluídas em cascata, incluindo todas as suas parcelas (pagas ou pendentes)
- [[Transações Recorrentes]] que apontam para este cartão são desativadas automaticamente

## Exclusão de Compra (Estorno)

- Remove a compra e **todas as suas parcelas**, pagas ou pendentes
- O Limite Disponível é recalculado automaticamente após o estorno
- Não gera nenhum estorno automático no saldo de [[Contas Bancária]] — caso a fatura já tenha sido paga, o ajuste deve ser feito manualmente

## Pagamento de Fatura

- O endpoint `POST /api/credit-cards/{id}/invoice/pay` é o ponto de entrada para pagamento de fatura
- O endpoint recebe:
    - ID da [[Contas Bancária]] que será debitada
    - Mês e ano da fatura a ser paga
- Internamente o endpoint:
    1. Calcula o valor total da fatura do mês informado
    2. Lança automaticamente uma [[Transação]] do tipo Despesa com `Pago = true` na conta bancária informada
    3. Marca todas as parcelas daquela fatura como pagas (`IsPaid = true`)
- O saldo da [[Contas Bancária]] é debitado imediatamente pelo valor total da fatura
- Não é possível pagar uma fatura com valor zero — o endpoint deve retornar erro caso não haja parcelas pendentes no mês informado

## Fatura do Mês

- A fatura é calculada dinamicamente — não é armazenada como um documento separado no banco
- O endpoint `GET /api/dashboards/credit-card/{creditCardId}/invoice?month={m}&year={y}` retorna:
    - Valor total da fatura no mês
    - Lista detalhada de cada compra/parcela com descrição, data da compra, número da parcela e valor
    - Status de pagamento de cada parcela (`IsPaid`)
- O balanço mensal do Dashboard (`GET /api/dashboards/monthly-balance/{houseHoldId}`) consolida as faturas de todos os cartões do grupo em `TotalCreditCardInvoices`

## Relação com outros módulos

- [[Contas Bancária]] — o pagamento da fatura debita automaticamente a conta bancária escolhida
- [[Transações Recorrentes]] — assinaturas no cartão são lançadas automaticamente pelo Worker como compras de 1 parcela
- [[Grupos - Residência - Família]] — todos os membros do grupo visualizam os cartões e faturas

## Situação Atual no Código

- Criação, listagem, compras e estorno de compras estão implementados
- Edição e exclusão do cartão são pendentes
- Alteração do Dia de Fechamento não recalcula parcelas já existentes — comportamento intencional