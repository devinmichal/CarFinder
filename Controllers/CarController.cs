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

namespace carfinder.Controllers
{
    [RoutePrefix("api/car")]
    public class CarController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

   

        [Route("GetCar")]
        public async Task<IHttpActionResult> GetCar(int Id)
        {
            var car = db.Cars.Find(Id);

            if (car == null)
                return await Task.FromResult(NotFound());

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
            var imageUrl = marketData?.FirstOrDefault()?.Image?.FirstOrDefault()?.MediaUrl;

            dynamic recalls;

            using (var httpclient = new HttpClient())
            {
                httpclient.BaseAddress = new Uri("Http://www.nhtsa.gov/");

                try
                {
                    response =
                        await httpclient.GetAsync($"webapi/api/Recalls/vehicle/modelyear/{car.model_year}/make/{car.make}/model/{car.model_name}?format=json");
                    recalls = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                }
                catch(Exception e )
                {
                    return InternalServerError(e);
                }
            }
            return Ok(new { car, imageUrl, recalls});
        }

        [Route("AllYears")]
        public async Task<IHttpActionResult> GetAllYears()
        {
            var years = await db.AllYears();
            return Ok(years);
        }

        [Route("MakesByYear")]
        public async Task<IHttpActionResult> GetMakesByYear(string year)
        {
            var makes = await db.MakesByYear(year);
            return Ok(makes);
        }

        [Route("ModelsByYearMake")]
        public async Task<IHttpActionResult> GetModelsByYearMake(string year, string make)
        {
            var models = await db.ModelsByYearMake(year,make);
            return Ok(models);
        }

        [Route("TrimsByYearMakeModel")]
        public async Task<IHttpActionResult> GetTrimsByYearMakeModel(string year, string make, string model)
        {
            var trims = await db.TrimsByYearMakeModel(year, make, model);
            return Ok(trims);
        }

        [Route("CarsByYear")]
        public async Task<IHttpActionResult> GetCarByYears(string year)
        {
            var cars = await db.CarByYears(year);
            return Ok(cars);
        }

        [Route("CarsByYearMake")]
        public async Task<IHttpActionResult> GetCarByYearMake(string year , string make)
        {
            var cars = await db.CarByYearMake(year,make);
            return Ok(cars);
        }

        [Route("CarsByYearMakeModel")]
        public async Task<IHttpActionResult> GetCarsByYearMakeModel(string year, string make , string model)
        {
            var cars = await db.CarsByYearMakeModel(year,make,model);
            return Ok(cars);
        }

        [Route("CarsByYearMakeModelTrim")]
        public async Task<IHttpActionResult> GetCarsByYearMakeModelTrim(string year, string make , string model, string trim)
        {
            var cars = await db.CarsByYearMakeModelTrim(year,make,model,trim);
            return Ok(cars);
        }

        //[Route("GetCars")]
        //public async Task<IHttpActionResult> GetCars(string year, string make, string model, string trim, string filter , int paging)
        //{
        //    var cars = await db.GetCars(year, make, model, trim, filter , paging);
        //    return Ok(cars);
        //}
    }
}
