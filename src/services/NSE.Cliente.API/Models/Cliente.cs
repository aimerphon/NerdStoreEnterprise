using NSE.Core.DomainObjects;
using System;

namespace NSE.Clientes.API.Models
{
    public class Cliente : Entity, IAggregateRoot
    {
        public string Nome { get; private set; }

        public Email Email { get; private set; }

        public Cpf Cpf { get; private set; }

        public bool Excluido { get; private set; }

        public Endereco Endereco { get; private set; }

        protected Cliente() { }

        public Cliente(Guid id, string nome, string email, string cpf)
        {
            Id = id;
            Nome = nome;
            Email = new Email(email);
            Cpf = new Cpf(cpf);
            Excluido = false;
        }

        public void TrocarEmail(string email)
        {
            Email = new Email(email);
        }

        public void AdicionarEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }
    }

    public class Endereco : Entity
    {
        public string Logradouro { get; private set; }

        public string Numero { get; private set; }

        public string Complemento { get; private set; }

        public string Bairro { get; private set; }

        public string Cep { get; private set; }

        public string Cidade { get; private set; }

        public string Estado { get; private set; }

        public Guid ClienteId { get; private set; }

        public Cliente Cliente { get; protected set; }

        public Endereco(string logradouro, string numero, string complemento, string bairro, string cep, string cidade, string estado)
        {
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cep = cep;
            Cidade = cidade;
            Estado = estado;
        }
    }
}