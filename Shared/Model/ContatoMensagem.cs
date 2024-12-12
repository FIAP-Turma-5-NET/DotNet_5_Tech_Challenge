namespace Shared.Model
{
    public class ContatoMensagem
    {
        public ContatoMensagem()
        {
            DataCriacao = DateTime.Now;
        }

        public string Nome { get; set; }
        public string DDD { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public DateTime DataCriacao { get; set; }
        public string TipoDeEvento { get; set; }
    }
}

