using System.ComponentModel.DataAnnotations;

namespace MedicalStore.Models
{
    public class Pending
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string MedicineName { get; set; }
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public int Expired { get; set; }
        public int Final { get; set; }
    }
}
