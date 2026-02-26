using WakeCommerce.Domain.Validation;

namespace WakeCommerce.Domain.Entities
{
    public sealed class Produto : Entity
    {
        public Produto() { }
        public Produto(string nome, int estoque, decimal preco)
        {
            Validate(nome, estoque, preco);
        }
        public string Nome { get; private set; }
        public decimal Estoque { get; private set; }
        public decimal Preco { get; private set; }
          

        public void Update(string nome, int estoque, decimal preco)
        {
            Validate(nome, estoque, preco);
        }

        private void Validate(string nome, int estoque, decimal preco)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(nome), "O nome é obrigatório");

            DomainExceptionValidation.When(preco < 0, "Valor do preço inválido");

            DomainExceptionValidation.When(estoque < 0, "Estoque inválido");

            Nome = nome;
            Preco = preco;
            Estoque = estoque;
            
        }

    }
}
