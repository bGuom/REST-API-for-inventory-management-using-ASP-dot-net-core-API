using Microsoft.EntityFrameworkCore;
using ProductAPI.Models.DBModels;
using ProductAPI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductAPI.Models.Context
{
    public class APIContext : DbContext
    {

        public APIContext(DbContextOptions<APIContext> options) : base(options)
        {

        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<LogObject> LogObjects { get; set; }
        public DbSet<StockAlert> StockAlerts { get; set; }
        public DbSet<AlertReportMessage> AlertReports { get; set; }
        public DbSet<SalesRep> SalesReps { get; set; }
        public DbSet<StockDelivery> StockDeliveries { get; set; }
        public DbSet<DeliveryArea> DeliveryAreas { get; set; }
        public DbSet<Route> Routes { get; set; }



    }
}
