using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace carfinder.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Car> Cars { get; set; }

        public async Task<List<string>> AllYears()
        {
            

            return await this.Database
                .SqlQuery<string>("AllYears").ToListAsync();
        }

        public async Task<List<Car>> AllCars()
        {


            return await this.Database
                .SqlQuery<Car>("AllCars").ToListAsync();
        }

        public async Task<List<string>> MakesByYear(string year)
        {
            var yearParam = new SqlParameter("@year", year);

            return await this.Database
                .SqlQuery<string>("MakesByYear @year", yearParam).ToListAsync();
        }


        public async Task<List<string>> ModelsByYearMake(string year, string make )
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make);

            return await this.Database
                .SqlQuery<string>(" ModelByModelYearMake @year, @make", yearParam, makeParam).ToListAsync();
        }


        public async Task<List<string>> TrimsByYearMakeModel(string year, string make, string model)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make);
            var modelParam = new SqlParameter("@model", model);

            return await this.Database
                .SqlQuery<string>("TrimByModelMakeYear @year, @make, @model", yearParam, makeParam, modelParam).ToListAsync();

        }

        public async Task<List<Car>> CarByYears(string year)
        {
            var yearParam = new SqlParameter("@year", year);

            return await this.Database
                .SqlQuery<Car>(" CarByYear @year", yearParam).ToListAsync();
        }

        public async Task<List<Car>> CarByYearMake(string year, string make )
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make ?? "");

            return await this.Database
                .SqlQuery<Car>(" CarByYearMake @year, @make", yearParam, makeParam).ToListAsync();
        }

        public async Task<List<Car>> CarsByYearMakeModel(string year, string make, string model)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make);
            var modelParam = new SqlParameter("@model",  model);

            return await this.Database
                .SqlQuery<Car>(" CarsByYearMakeModel @year, @make, @model", yearParam, makeParam, modelParam).ToListAsync();
        }




        public async Task<List<Car>> CarsByYearMakeModelTrim(string year, string make, string model, string trim)
        {
            var yearParam = new SqlParameter("@year", year);
            var makeParam = new SqlParameter("@make", make);
            var modelParam = new SqlParameter("@model", model);
            var trimParam = new SqlParameter("@trim", trim);


            return await this.Database
                .SqlQuery<Car>(" CarByYearMakeModelTrim @year, @make, @model,@trim", yearParam, makeParam, modelParam, trimParam).ToListAsync();
        }

        public async Task<List<Car>> GetCars(string year, string make, string model, string trim , string filter, int? paging)
        {
            var yearParam = new SqlParameter("@year", year ?? "");
            var makeParam = new SqlParameter("@make", make ?? "");
            var modelParam = new SqlParameter("@model", model ?? "");
            var trimParam = new SqlParameter("@trim", trim ?? "");
            var filterParam = new SqlParameter("@filter", filter ?? "");
            var pagingParam = new SqlParameter("@paging", paging ?? 1);
            //var perPageParam = new SqlParameter("@perpage", perPage ?? 50);
            //var sortColumnParam = new SqlParameter("@sortcolumn", sortColumn ?? "");
            //var sortDirectionParam = new SqlParameter("@sortdirection", sortDirection ?? "asc");


            return await this.Database
                .SqlQuery<Car>("GetCars @year, @make, @model, @trim, @filter, @paging", yearParam, makeParam, modelParam, trimParam, filterParam, pagingParam).ToListAsync();
        }

        public async Task<int> GetCarsCount(string year, string make, string model, string trim, string filter)
        {
            var yearParam = new SqlParameter("@year", year ?? "");
            var makeParam = new SqlParameter("@make", make ?? "");
            var modelParam = new SqlParameter("@model", model ?? "");
            var trimParam = new SqlParameter("@trim", trim ?? "");
            var filterParam = new SqlParameter("@filter", filter ?? "");



            return await this.Database
                .SqlQuery<int>("GetCars @year, @make, @model, @trim, @filter", yearParam, makeParam, modelParam, trimParam, filterParam).FirstAsync();
        }



        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}