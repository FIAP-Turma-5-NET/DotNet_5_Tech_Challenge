using System.ComponentModel.DataAnnotations;

namespace FIAP_Contato.Application.Model;

public class ContatoModel()
{
    [Required(ErrorMessage ="Nome é obrigatório!")]
    [Length(3, 50, ErrorMessage = "Nome tem que ser maior que 2 e menor que 50!")]
    public required string Nome { get; set; }

    [Required(ErrorMessage = "Telefone é obrigatório!")]
    [RegularExpression("^\\([0-9]{2}\\) [0-9]{4,5}-[0-9]{4}$", ErrorMessage = "Telefone inválido! O número deve estar no formato (XX) XXXXX-XXXX ou (XX) XXXX-XXXX.")]
    public required string Telefone { get; set; }

    [Required(ErrorMessage = "E-mail é obrigatório!")]
    [EmailAddress(ErrorMessage ="E-mail inválido!")]
    public required string Email { get; set; }
}   
    
    