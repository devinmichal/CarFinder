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

namespace carfinder.Controllers
{
    public class CarSearchController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CarSearch
        public async Task<ActionResult> Index(string year, string make , string trim, string model, string filter) 
        {
          

            if (string.IsNullOrWhiteSpace(year))
            {
                year = "2015"; 
                ViewBag.make = null;
                ViewBag.trim = null;
                ViewBag.model = null;
               
            }
            else
            {
                ViewBag.make = new SelectList(await db.MakesByYear(year), year);
            }
            if (!string.IsNullOrWhiteSpace(make))
            {
                ViewBag.model = new SelectList(await db.ModelsByYearMake(year, make), make);
            }
            if(!string.IsNullOrWhiteSpace(model))
            {
                ViewBag.trim = new SelectList(await db.TrimsByYearMakeModel(year, make, model), model);
            }
            ViewBag.year = new SelectList(await db.AllYears(), year);

            var carlist = await db.GetCars(year, make, model, trim, filter, 1);
            
            return View(carlist);
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




            return View(car);
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
