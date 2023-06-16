using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalStore.Models
{
    public class StoreManager
    {
       
        [Key]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDateTiem { get; set; } = DateTime.Now;
        public string role { get; set; }

    }
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set;}
    }
    public class Order
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public int MEDICINEID { get; set; }
        public int COUNTING { get; set; }
        public string STATUS { get; set; }
        public DateTime ORDERDATETIME { get; set; }
    }
   public class OrderShown
    {
        public int Id { get; set; }
        public string MedicineName { get; set; }
        public int Counting { get; set; }
        public int cost_per_item { get; set; }
        public string? STATUS { get; set; }
    }
    public class cart
    {
        [Key]
        public int OrderId { get; set; }
        public int UserID { get; set; }
        public int MedicineID { get; set; }
        public int UnitPrice { get; set; }
        public int counting { get; set; }
    }
    public class cartWithMedicineName
    {
        [Key]
        public int OrderId { get; set; }
        public int UserID { get; set; }
        public int MedicineID { get; set; }
        public int UnitPrice { get; set; }
        public int counting { get; set; }
        public string MedicineName { get; set; }
    }
}
