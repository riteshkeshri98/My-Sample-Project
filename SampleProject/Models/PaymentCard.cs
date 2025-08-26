using System.ComponentModel.DataAnnotations;

namespace SampleProject.Models
{
    public class PaymentCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string CVV { get; set; } = string.Empty;

        [Required]
        public string ExpiryDate { get; set; } = string.Empty;
    }
}
