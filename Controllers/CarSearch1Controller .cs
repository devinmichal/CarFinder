using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using carfinder.Models;
using System.Threading.Tasks;
using Bing;
using Newtonsoft.Json;
using System.Web.Mvc;
using DataTables.Mvc;

namespace carfinder.Controllers
{
    public class CarSearch1Controller : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CarSearch
        //public async Task<ActionResult> Index( string year, string make , string trim, string model, string filter) 
        //{
         
        //    if (string.IsNullOrWhiteSpace(year))
        //    {
              
        //        ViewBag.make = new SelectList(new List<string>());
        //        ViewBag.trim = new SelectList(new List<string>());
        //        ViewBag.model = new SelectList(new List<string>());
             
             

               
        //    }
        //    else
        //    {
        //        ViewBag.make = new SelectList(await db.MakesByYear(year), year);
        //    }
        //    if (!string.IsNullOrWhiteSpace(make))
        //    {
        //        ViewBag.model = new SelectList(await db.ModelsByYearMake(year, make), make);
        //    }
        //    if(!string.IsNullOrWhiteSpace(model))
        //    {
        //        ViewBag.trim = new SelectList(await db.TrimsByYearMakeModel(year, make, model), model);
        //    }
        //    ViewBag.year = new SelectList(await db.AllYears(), year);

        //    //var carlist = await db.GetCars(year, make, model, trim, filter, null, null ,null, null, null);
            
        //    return View(carlist);
        //}

        public async Task<ActionResult> MakesByYear (string year)
        {
            var model = new SelectList(await db.MakesByYear(year));
            return Content(JsonConvert.SerializeObject(model), "application/json");
        }


        public async Task<ActionResult> ModelsByYearMake(string year, string make)
        {
            var model = new SelectList(await db.ModelsByYearMake(year, make));
            return Content(JsonConvert.SerializeObject(model), "application/json");
        }


        public async Task<ActionResult> TrimsByYearMakeModel(string year, string make , string model)
        {
            var Model = new SelectList(await db.TrimsByYearMakeModel(year,make,model));
            return Content(JsonConvert.SerializeObject(Model), "application/json");
        }


        public async Task<JsonResult> GetCars([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest request, string year, string make, string trim, string model)
        {
            var filter = request.Search.Value;

            var totalCount = await db.GetCarsCount(year, make, model, trim, filter);

            var column = request.Columns.FirstOrDefault(r => r.IsOrdered == true);
            var sortColumn = "";
            var sortOrder = "asc";

            if (column != null)
            {
                sortColumn = column.Data;
                if (column.SortDirection == Column.OrderDirection.Descendant)
                {
                    sortOrder = "Desc";
                }
            }
            List<Car> cars = new List<Car>();
            //cars = await db.GetCars(year, make, model, trim, filter, 1, (request.Start / request.Length + 1), request.Length, sortColumn, sortOrder);

            var paged = cars.Select(c =>
            new CarViewModel
            {
                Id = c.id,
                Make = c.make,
                Model = c.model_name,
                Trim = c.model_trim,
                Year = c.model_year,

                Link = "<a href=\"/Cars1/Details/" + c.id + "\">Select</a>",

            });


            return Json(new DataTablesResponse(request.Draw, paged, totalCount, db.Cars.Count()),
                           JsonRequestBehavior.AllowGet);
        }





        // GET: CarSearch/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = await db.Cars.FindAsync(id);
            if (car == null)
            {
                return HttpNotFound();
            }

            HttpResponseMessage response;

            var client = new BingSearchContainer(new Uri("https://api.datamarket.azure.com/Bing/search/"));
            client.Credentials = new NetworkCredential("accountKey", "bYJ3eVHV+r8dLybppgrXgIetjoLSVumwlpyLDwmbw8w");
            var marketData = client.Composite(
                "image",
                $"{car.model_year} {car.make} {car.model_name} {car.model_trim}",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
                ).Execute();
            ViewBag.imageUrl = marketData?.FirstOrDefault()?.Image?.FirstOrDefault()?.MediaUrl;

            

            using (var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri("Http://www.nhtsa.gov/");

                try
                {
                    response =
                        await httpclient.GetAsync($"webapi/api/Recalls/vehicle/modelyear/{car.model_year}/make/{car.make}/model/{car.model_name}?format=json");
                    ViewBag.recalls = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }




            return PartialView("_Details",car);
        }

        private ActionResult InternalServerError(Exception e)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
