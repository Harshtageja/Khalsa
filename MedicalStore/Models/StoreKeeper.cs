using System.ComponentModel.DataAnnotations;
namespace MedicalStore.Models
   
{
    public class StoreKeeper
    {
        [Key]
     
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDateTiem { get; set; } = DateTime.Now;
    }
}
