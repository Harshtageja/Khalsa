using MedicalStore.Data;
using MedicalStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalStore.Controllers
{
    public class StoreKeeperController : Controller
    {
        private readonly ApplicationDbContext _db;
        public StoreKeeperController(ApplicationDbContext db)
        {
            _db=db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(StoreKeeper obj)
        {
            var categoryFromDb = _db.StoreKeepers.Find(obj.Email);
            if (categoryFromDb != null)
            {
                if (categoryFromDb.Password == obj.Password)
                {
                    return RedirectToAction("Logged_in_as_StoreKeeper");
                }

            }
            TempData["failed"] = "Try Again";


                
               //if(storekeeper != null)
               //{
                    
               // }
            
            return View();
        }
        public IActionResult Logged_in_as_StoreKeeper()
        {
            IEnumerable<Inventory> objCategoryList = _db.Inventories;

            return View(objCategoryList);
        }
        public IActionResult AddItem()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddItem(Pending obj)
        {
            if (ModelState.IsValid)
            {
                _db.Pendings.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Logged_in_as_StoreKeeper");
            }
            else
            {
                return View();
            }

        }
    }
}
