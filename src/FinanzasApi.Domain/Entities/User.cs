using System.ComponentModel.DataAnnotations;

namespace FinanzasApi.src.FinanzasApi.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Email { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
