- É um cadastro para agrupar [[Transação]] e [[Transações Recorrentes]] em receitas e despesas
- As categorias pertencem a um [[Grupos - Residência - Família]], ou seja, cada grupo gerencia suas próprias categorias
- Uma categoria deverá ter os seguintes dados:
    - ID do [[Grupos - Residência - Família]]
    - Nome (ex: Alimentação, Salário, Moradia)
    - Tipo (Receita / Despesa)
    - Usuário que criou a categoria

## Regras de Negócio

- O nome da categoria deve ser único dentro do mesmo [[Grupos - Residência - Família]] e Tipo
    - Exemplo: pode existir "Alimentação" do tipo Despesa e "Alimentação" do tipo Receita, mas não dois "Alimentação" do tipo Despesa no mesmo grupo
- Uma categoria do tipo **Receita** só pode ser vinculada a transações do tipo Receita
- Uma categoria do tipo **Despesa** só pode ser vinculada a transações do tipo Despesa e a [[Transações Recorrentes]]
- Não deve ser possível excluir uma categoria que já esteja vinculada a alguma [[Transação]] ou [[Transações Recorrentes]]

## Endpoints

- `POST /api/categories` — Cadastra uma nova categoria
- `GET /api/categories/house-hold/{houseHoldId}` — Lista todas as categorias de um grupo familiar
- `GET /api/categories/{id}` — Retorna os dados de uma categoria
- `PATCH /api/categorias/{id}` — Atualiza os dados de uma categoria
- `DELETE /api/categories/{id}` — Remove uma categoria (apenas se não estiver em uso)

## Situação Atual no Código

- Hoje a categoria é armazenada como um campo `string?` livre nas entidades `Transaction` e `RecurringCharge`
- A implementação deste módulo deverá substituir esse campo livre por uma referência ao `Id` da entidade `Category`
- Será necessário criar a entidade `Category`, o repositório `ICategoryRepository`, os commands de criação e exclusão, a query de listagem e o `CategoriesController`