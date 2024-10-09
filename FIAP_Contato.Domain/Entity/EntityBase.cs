using System.ComponentModel.DataAnnotations;

namespace FIAP_Contato.Domain.Entity;

public class EntityBase
{
    [Key]
    public int Id { get; set; }
}