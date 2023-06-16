using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalStore.Models
{
    public class Billing
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public long Phone { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
       

    }
}
