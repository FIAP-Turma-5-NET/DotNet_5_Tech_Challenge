using Bogus;

using FIAP_Contato.Domain.Entity;

namespace FIAP_Contato.Test.Integration
{
    internal class ContatoDataBuilder : Faker<Contato>
    {       
        public ContatoDataBuilder()
        {
            
             CustomInstantiator(f => new Contato
             {
                 Nome = f.Name.FullName(),
                 DDD = "",
                 Telefone = f.Phone.PhoneNumber("(##) #####-####"),
                 Email = f.Internet.Email()
             });
        }

        public Contato Build() => Generate();

        public List<Contato> BuildList(int quantidade) => Generate(quantidade);
    }
}
