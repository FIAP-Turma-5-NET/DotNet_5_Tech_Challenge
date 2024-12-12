using AutoMapper;

using FIAP_Contato.Consumer.Interface;
using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface.Repository;

using Shared.Model;

namespace FIAP_Contato.Consumer.Service
{
     public class ConsumerService : IConsumerService
    {
        private readonly IContatoRepository _contatoRepository;
        private IMapper _mapper;

        public ConsumerService(IContatoRepository contatoRepository, IMapper mapper)
        {
            _contatoRepository = contatoRepository;
            _mapper = mapper;
        }


        public async Task<string> CadastrarContato(ContatoMensagem request)
        {
            var response = string.Empty;

            var entity = (await _contatoRepository.ObterTodosAsync())
                .FirstOrDefault(c => c.Nome.Equals(request.Nome, StringComparison.CurrentCultureIgnoreCase)
                           && c.Email.Equals(request.Email));

            if (entity == null)
            {

                var cadastro = _mapper.Map<Contato>(request);

                var res = await _contatoRepository.CadastrarScalarAsync(cadastro);

                if (res > 0)
                    response = "Cadastrado com sucesso!";
                else
                    throw new InvalidOperationException("Erro ao Cadastrar!");
            }
            else
            {
                throw new InvalidOperationException("Contato já existe!");
            }

            return response;
        }
    }
}
