using MedicalStore.Data;
using Microsoft.AspNetCore.Mvc;
using MedicalStore.Migrations;
using MedicalStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MedicalStore.Controllers
{
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _db;
		public UserController(ApplicationDbContext db)
		{
			_db = db;


		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult SearchingPage()
		{
            if (HttpContext.Session.GetString("role") == "Customer")
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("role", "");
            HttpContext.Session.SetString("Email", "");
            return RedirectToAction("Index","Home");
        }
        [HttpPost]
		public JsonResult SearchMedicine(string tosearch)
		{
			var query = $"Select * from Inventories where MedicineName like '%{tosearch}%'";
			var query1 = "SELECT * FROM Inventories WHERE MedicineName LIKE {0}";
			var query2 = string.Format("SELECT * FROM Inventories WHERE MedicineName LIKE '%{0}%'", tosearch);
			var category=_db.Inventories.FromSqlRaw(query2).ToList();
			var a = category;
			return Json(a) ;
		}
        [HttpPost]
        public JsonResult ShowMeAllMedicine(string tosearch)
        {
            var query2 = string.Format("SELECT * FROM Inventories");
            var category = _db.Inventories.FromSqlRaw(query2).ToList();
            var a = category;
            return Json(a);
        }
        [HttpPost]
		public JsonResult LoginMeAsCustomer(string Email, string password) {
            var query2 = string.Format("SELECT * FROM Users WHERE Email='{0}' and password='{1}'", Email,password);
            var categoryFromDb = _db.Users.FromSqlRaw(query2).ToList();
            //var categoryFromDb=_db.Users.Where(d => d.Email == Email).FirstOrDefault();
            if (categoryFromDb != null)
            {
                HttpContext.Session.SetString("role", "Customer");
                HttpContext.Session.SetString("Email", categoryFromDb[0].Email); 

                return Json(categoryFromDb);


            }
            TempData["failed"] = "Try Again";
			return Json(null);
        }
        [HttpPost]
        public JsonResult MakeUser(string Name,string Phone,string Email,string  Password,string Address) {
            var query2 = string.Format("SELECT * FROM Users WHERE Email='{0}'", Email);
            var categoryFromDb = _db.Users.FromSqlRaw(query2).ToList();
            if(categoryFromDb.Count>0) {
                return Json("AlreadyExists");
                    }
            var query3 = string.Format("INSERT INTO Users(EMAIL,NAME,ADDRESS,PASSWORD,PHONE)VALUES('{0}','{1}','{2}','{3}','{4}')", Email,Name,Address,Password,Phone);
            var categoryFromDb1 = _db.Database.ExecuteSqlRaw(query3);

            return Json("success");
        }
        [HttpPost]
		public JsonResult AddtoCart(string MedicineId,string Count,string UnitPrice) {
            if (HttpContext.Session.GetString("role") == "Customer")
            {
                var query1 = string.Format("SELECT * FROM Users WHERE Email='{0}'", HttpContext.Session.GetString("Email"));
                var categoryFromDb1 = _db.Users.FromSqlRaw(query1).ToList();
                var query3 = string.Format("UPDATE CART SET counting={0} WHERE MedicineID={1} AND USERID={2}",Count,MedicineId, categoryFromDb1[0].Id);
                var categoryFromDb3 = _db.Database.ExecuteSqlRaw(query3);
                if (categoryFromDb3 == 0)
                {
                    var query2 = string.Format("Insert into Cart(UserID,MedicineId,UnitPrice,Counting) values({0},{1},{2},{3})", categoryFromDb1[0].Id, MedicineId, UnitPrice, Count);
                    var categoryFromDb = _db.Database.ExecuteSqlRaw(query2);
                }
                return Json("Success");
            }
			return Json("Bye");
            
				
		}
        public IActionResult ShowMeCart()
        {
            if (HttpContext.Session.GetString("role") == "Customer")
            {
                var query1 = string.Format("SELECT * FROM Users WHERE Email='{0}'", HttpContext.Session.GetString("Email"));
                var categoryFromDb1 = _db.Users.FromSqlRaw(query1).ToList();
                var query2 = string.Format("SELECT * FROM Cart inner join Inventories on Cart.MedicineID=Inventories.Id WHERE Cart.UserId='{0}'", categoryFromDb1[0].Id);
                var categoryFromDb2 = _db.cartWithMedicineNames.FromSqlRaw(query2);
                IEnumerable<cartWithMedicineName> objCategoryList = categoryFromDb2;
                return View(objCategoryList);
            }
            return RedirectToAction("Index");
            
        }
        public IActionResult MyOrders()
        {
            var query1 = string.Format("SELECT * FROM Users WHERE Email='{0}'", HttpContext.Session.GetString("Email"));
            var categoryFromDb1 = _db.Users.FromSqlRaw(query1).ToList();
            var query2 = string.Format("SELECT ORDERS.ID AS ID,INVENTORIES.MEDICINENAME AS MEDICINENAME,ORDERS.COUNTING AS COUNTING,INVENTORIES.PRICE AS cost_per_item,ORDERS.STATUS FROM ORDERS INNER JOIN INVENTORIES ON ORDERS.MEDICINEID=INVENTORIES.ID WHERE USERID={0}", categoryFromDb1[0].Id);
            var categoryFromDb2 = _db.OrdersShown.FromSqlRaw(query2);
            IEnumerable<OrderShown> objCategoryList = categoryFromDb2;
            return View(categoryFromDb2);
        }
        [HttpPost]
        public JsonResult GiveMecount(int MedicineId)
        {
            if (HttpContext.Session.GetString("role") == "Customer")
            {
                var query1 = string.Format("SELECT * FROM Cart WHERE MedicineID='{0}'", MedicineId);
                var categoryFromDb1 = _db.Cart.FromSqlRaw(query1).ToList();
                return Json(categoryFromDb1);
                
               
            }
            return Json(null);
        }
        [HttpPost]
        public JsonResult ChangeStatus(int orderId,string FUTURE)
        {

            if (HttpContext.Session.GetString("role") == "StoreManager")
            {
                var query1 = string.Format("UPDATE ORDERS SET STATUS='{0}' WHERE ID={1}", FUTURE,orderId);
                var categoryFromDb1 = _db.Database.ExecuteSqlRaw(query1);
                return Json("success");


            }
            return Json(null);
        }
        public IActionResult PlaceOrder() {
            if (HttpContext.Session.GetString("role") == "Customer")
            {
                var query1 = string.Format("SELECT * FROM Users WHERE Email='{0}'", HttpContext.Session.GetString("Email"));
                var categoryFromDb1 = _db.Users.FromSqlRaw(query1).ToList();
                var query2 = string.Format("INSERT INTO ORDERS(UserId,MedicineId,COUNTING) SELECT UserID, MedicineID, Counting FROM CART where userid={0}", categoryFromDb1[0].Id);
                var categoryFromDb2 = _db.Database.ExecuteSqlRaw(query2);
                var query3 = string.Format("DELETE FROM CART WHERE USERID={0}", categoryFromDb1[0].Id);
                var categoryFromDb3 = _db.Database.ExecuteSqlRaw(query3);
                return RedirectToAction("MyOrders");


            }
            return View("Index");
        }
    }

    
}
