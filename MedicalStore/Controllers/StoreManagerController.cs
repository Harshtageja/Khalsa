using MedicalStore.Data;
using MedicalStore.Migrations;
using MedicalStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Web.Helpers;
using Microsoft.AspNetCore.Identity;

namespace MedicalStore.Controllers
{
    public class StoreManagerController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StoreManagerController(ApplicationDbContext db)
        { 
            _db=db;
        
            
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(StoreManager obj)
        {
            var categoryFromDb = _db.StoreManagers.Find(obj.Email);
            if (categoryFromDb != null)
            {
                if (categoryFromDb.role == "StoreManager" && (categoryFromDb.Password == obj.Password))
                {
                    HttpContext.Session.SetString("role", categoryFromDb.role);
                    HttpContext.Session.SetString("Email", obj.Email); return RedirectToAction("Logged_in_as_StoreManager");
                }
                else if (categoryFromDb.role == "StoreKeeper" && (BCrypt.Net.BCrypt.Verify(obj.Password, categoryFromDb.Password)))
                {
                    HttpContext.Session.SetString("role", categoryFromDb.role);
                    HttpContext.Session.SetString("Email", obj.Email);
                    return RedirectToAction("Logged_in_as_StoreManager");
                }
                

            }
            TempData["failed"] = "Try Again";
            return View();
        }
        public IActionResult Logged_in_as_StoreManager()
        {
             
            
            if (HttpContext.Session.GetString("role") == "StoreManager" || HttpContext.Session.GetString("role") == "StoreKeeper")
            {
                IEnumerable<Inventory> objCategoryList = _db.Inventories;


                return View(objCategoryList);
            }
            return RedirectToAction("Index");


        }
        public IActionResult NewStoreKeeper()
        {
            if (HttpContext.Session.GetString("role") == "StoreManager")
            {

                return View();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult NewStoreKeeper(StoreManager obj)
        {
            if (HttpContext.Session.GetString("role") == "StoreManager")
            {
                if (ModelState.IsValid)
                {


                    obj.role = "StoreKeeper";
                    obj.Password = BCrypt.Net.BCrypt.HashPassword(obj.Password);

                    _db.StoreManagers.Add(obj);
                    _db.SaveChanges();
                    TempData["success"] = "StoreKeeper Added Successfully";
                    HttpContext.Session.SetString("role", "StoreManager");
                    return RedirectToAction("Logged_in_as_StoreManager");
                }
                return View();
            }
            return RedirectToAction("Index");
        }
        public IActionResult AddItem()
        {
            if (HttpContext.Session.GetString("role") == "StoreManager" || HttpContext.Session.GetString("role") == "StoreKeeper")
            {

                return View();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(Inventory obj)
        {
            
                var role = HttpContext.Session.GetString("role");
            if (role == "StoreKeeper")
            {
                if (ModelState.IsValid)
                {
                    Pending obj2 = new Pending();
                    obj2.MedicineName = obj.MedicineName;

                    obj2.StockIn = obj.StockIn;
                    obj2.StockOut = 0;
                    obj2.Expired = obj.Expired;
                    obj2.Final = obj.StockIn - obj.Expired;
                    await _db.Pendings.AddAsync(obj2);
                    await _db.SaveChangesAsync();


                    return RedirectToAction("Logged_in_as_StoreManager");
                }
                else
                {
                    return View();
                }
            }
            else if (role == "StoreManager")
            {
                if (ModelState.IsValid)
                {


                    obj.Approved = 1;
                    obj.Final = obj.StockIn - obj.Expired;
                    //var category = _db.Inventories.Where(d => d.MedicineName == obj.MedicineName);
                    var category = _db.Inventories.FromSql($"Select * from Inventories where MedicineName= {obj.MedicineName}");
                    if (category.Any())
                    {
                        var a = category.ToList();
                        a[0].StockIn += obj.StockIn;
                        a[0].Expired += obj.Expired;
                        a[0].Final = a[0].StockIn - (a[0].Expired + a[0].StockOut);

                        _db.Inventories.Update(a[0]);
                        _db.SaveChanges(true);
                        HttpContext.Session.SetString("role", "StoreManager");
                        return RedirectToAction("Logged_in_as_StoreManager");

                    }


                    obj.StockOut = 0;


                    _db.Inventories.Add(obj);
                    _db.SaveChanges();
                    TempData["success"] = "Inventory Added Successfully";
                    HttpContext.Session.SetString("role", "StoreManager");
                    return RedirectToAction("Logged_in_as_StoreManager");
                }
                else
                {
                    return View();
                }
            }
                return RedirectToAction("Index");

            }

           

        
        public IActionResult Approve()
        {
            if (HttpContext.Session.GetString("role") == "StoreManager" || HttpContext.Session.GetString("role")=="StoreKeeper")
            {
                IEnumerable<Pending> objCategoryList = _db.Pendings;
                //HttpContext.Session.SetString("role", "StoreManager");

                return View(objCategoryList);
            }
            return RedirectToAction("Index");
           
            
        }
        public IActionResult Approved(int id)
        {
            if (HttpContext.Session.GetString("role") == "StoreManager" || HttpContext.Session.GetString("role") == "StoreKeeper")
            {
                var obj = _db.Pendings.Find(id);
                var category = _db.Inventories.Where(d => d.MedicineName == obj.MedicineName);
                List<Inventory> a = new List<Inventory>();
                if (category.Any())


                {
                    a = category.ToList();
                    a[0].StockIn += obj.StockIn;
                    a[0].Expired += obj.Expired;
                    a[0].Final = a[0].StockIn - (a[0].Expired + a[0].StockOut);
                    _db.Inventories.Update(a[0]);
                }
                else
                {
                    var b = new Inventory();

                    b.StockIn = obj.StockIn;
                    b.Expired = obj.Expired;
                    b.StockOut = 0;
                    b.Approved = 1;
                    b.MedicineName = obj.MedicineName;
                    b.Final = b.StockIn - b.Expired;
                    _db.Inventories.Add(b);
                }
                var query = _db.Pendings.Remove(obj);


                _db.SaveChanges(true);

                return RedirectToAction("Logged_in_as_StoreManager");
            }
            return RedirectToAction("Index");
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.SetString("role", "");
            HttpContext.Session.SetString("Email", "");


            return RedirectToAction("Index");

        }
        public IActionResult Billing()
            
        {
            if (HttpContext.Session.GetString("role") == "StoreManager" || HttpContext.Session.GetString("role") == "StoreKeeper")
            {
                IEnumerable<Billing> objCategoryList = _db.Billings;
                return View(objCategoryList);
            }
            return RedirectToAction("Index");
        

        }
        public IActionResult AllOrders()

        {
            if (HttpContext.Session.GetString("role") == "StoreManager" || HttpContext.Session.GetString("role") == "StoreKeeper")
            {
                var query2 = string.Format("SELECT ORDERS.ID AS ID,INVENTORIES.MEDICINENAME AS MEDICINENAME,ORDERS.COUNTING AS COUNTING,INVENTORIES.PRICE AS cost_per_item,ORDERS.STATUS FROM ORDERS INNER JOIN INVENTORIES ON ORDERS.MEDICINEID=INVENTORIES.ID ");
                var categoryFromDb2 = _db.OrdersShown.FromSqlRaw(query2);
                IEnumerable<OrderShown> objCategoryList = categoryFromDb2;
                return View(categoryFromDb2);
            }
            return RedirectToAction("Index");


        }
        public IActionResult CustomerLogin()

        {
            return View();


        }
        public IActionResult CustomerSignUp()
        {
            return View();
        }
        public IActionResult Sell(int? id)
        {
            if (HttpContext.Session.GetString("role") == "StoreManager" || HttpContext.Session.GetString("role") == "StoreKeeper")
            {
                var med = _db.Inventories.Find(id);
                if (med != null)
                {
                    ViewBag.Medicine = med.MedicineName;
                    ViewBag.Qty = med.Final;
                    ViewBag.ls = _db.Inventories;
                }
                return View();
            }
            return RedirectToAction("Index");   


            
        }

        [HttpPost]
        public IActionResult Sell(Billing obj)
          
        {
            if (HttpContext.Session.GetString("role") == "StoreManager" || HttpContext.Session.GetString("role") == "StoreKeeper")
            {
                obj.id = 0;
                if (ModelState.IsValid)
                {
                    var category = _db.Inventories.Where(d => d.MedicineName == obj.MedicineName);
                    if (category.Any())
                    {
                        var a = category.ToList();
                        a[0].StockOut += obj.Quantity;
                        a[0].Final = a[0].StockIn - (a[0].Expired + a[0].StockOut);

                        _db.Inventories.Update(a[0]);
                        _db.SaveChanges(true);

                    }

                    _db.Billings.Add(obj);
                    _db.SaveChanges();
                    TempData["success"] = "Bill Generated Successfully";
                    return RedirectToAction("Billing");
                }
                return View(obj);
                
            }
            return RedirectToAction("Index");
        }
      


    }
}
