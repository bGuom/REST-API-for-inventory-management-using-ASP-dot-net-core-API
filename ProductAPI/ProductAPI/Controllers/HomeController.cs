using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using ProductAPI.Models.Context;
using ProductAPI.Models.DBModels;

namespace ProductAPI.Controllers
{
    public class HomeController : Controller
    {

        private readonly APIContext _context;

        public HomeController(APIContext context)
        {
            _context = context;
            RecurringJob.AddOrUpdate(() => CheckExpiryDates(), Cron.Daily);

        }

        public IActionResult Index()
        {
       
            return View();
        }


        public async Task CheckExpiryDates()
        {
            int offset = 0;
            int loadsize = 2;

            bool endofdata = false;

            do
            {

                List<Stock> selectedstocks = await _context.Stocks.Skip(offset).Take(loadsize).ToListAsync();
                foreach (var item in selectedstocks)
                {
                    DateTime Today = DateTime.Now;
                    DateTime ExpDate = item.ExpiryDate;

                    double DateDiff = (ExpDate - Today).TotalDays;

                    if (DateDiff < 0)
                    {
                        await CreateStockAlertAsync(item, "EXPIRED", "VERYHIGH");
                    }
                    else if (DateDiff < 10)
                    {
                        await CreateStockAlertAsync(item, "NEAR_EXPIRE", "HIGH");
                    }

                }
                offset += loadsize;
                if (selectedstocks.Count == 0) { endofdata = true; }

            } while (!endofdata);
        }







        







        private async Task CreateStockAlertAsync(Stock stock, string alertType, string priority)
        {
            StockAlert prevStockAlert = _context.StockAlerts.Where(sa => sa.StockID == stock.StockID).Where(sa => sa.AlertType == alertType).FirstOrDefault();

            if (prevStockAlert == null)
            {
                StockAlert stockAlert = new StockAlert
                {
                    StockID = stock.StockID,
                    Stock = stock,
                    AlertType = alertType,
                    Priority = priority,
                    CreatedDate = DateTime.Now,
                    Status = "NEW"
                };

                _context.StockAlerts.Add(stockAlert);
                await _context.SaveChangesAsync();

            }

        }


    }
}
