using System.ComponentModel.DataAnnotations;

namespace FinanzasApi.Domain.Entities
{
    public class Currency
    {
        [Key]
        [MaxLength(3)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public short MinorUnits { get; set; } = 2;
    }
}
