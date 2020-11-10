using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductAPI.Models;
using ProductAPI.Models.BindingModel;
using ProductAPI.Models.Context;
using ProductAPI.Models.DBModels;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly APIContext _context;


        public StocksController(APIContext context)
        {
            _context = context;
            //RecurringJob.AddOrUpdate(() => CheckExpiryDates(), Cron.Minutely);

        }







        // GET: api/Stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockViewModel>>> GetStocks()
        {
           

            List<StockViewModel> stocklist = new List<StockViewModel>();

            List<Stock> allstocks = await _context.Stocks.ToListAsync();
            foreach (var item in allstocks)
            {
                
                StockViewModel returnstock = new StockViewModel
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Threshold = item.Threshold,
                    ExpiryDate = item.ExpiryDate,
                    Supplier = item.Supplier,
                    Description = item.Description
                };

                stocklist.Add(returnstock);

            }
            return stocklist;
        }








        // GET: api/Stocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockViewModel>> GetStock(int id)
        {
            var stock = _context.Stocks.Single(s => s.StockID == id) ;

            if (stock == null)
            {
                return NotFound();
            }


            StockViewModel returnstock = new StockViewModel
            {
                ProductID = stock.ProductID,
                Quantity = stock.Quantity,
                Threshold = stock.Threshold,
                ExpiryDate = stock.ExpiryDate,
                Supplier = stock.Supplier,
                Description = stock.Description
            };

            return returnstock;
        }

        // PUT: api/Stocks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(int id, StockViewModel stockvm)
        {
            Product product = null;
            try {
                product = _context.Products.Single(i => i.ProductID == stockvm.ProductID);
            }
            catch (ArgumentNullException) {
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }

            if (product == null)
            {
                return NotFound();
            }


            Stock stock = new Stock
            {
                StockID = id,
                ProductID = stockvm.ProductID,
                Product = product,
                Quantity = stockvm.Quantity,
                Threshold = stockvm.Threshold,
                ExpiryDate = stockvm.ExpiryDate,
                Supplier = stockvm.Supplier,
                Description = stockvm.Description
            };

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Stocks
        [HttpPost]
        public async Task<ActionResult<Stock>> PostStock(StockViewModel stockvm)
        {

            Product product = null;
            try
            {
                product = _context.Products.Include(p => p.ProductTypeName).Single(i => i.ProductID == stockvm.ProductID);
            }
            catch (ArgumentNullException)
            {
                return BadRequest();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }

            if (product == null)
            {
                return NotFound();
            }


            Stock stock = new Stock
            {
                ProductID = stockvm.ProductID,
                Product = product,
                Quantity = stockvm.Quantity,
                Threshold = stockvm.Threshold,
                ExpiryDate = stockvm.ExpiryDate,
                Supplier = stockvm.Supplier,
                Description = stockvm.Description
            };

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();


            StockViewModel returnstock = new StockViewModel
            {
                ProductID = stock.ProductID,
                Quantity = stock.Quantity,
                Threshold = stock.Threshold,
                ExpiryDate = stock.ExpiryDate,
                Supplier = stock.Supplier,
                Description = stock.Description
            };

            return CreatedAtAction("GetStock", new { id = stock.StockID }, returnstock);
        }








        // DELETE: api/Stocks/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Stock>> DeleteStock(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();

            return stock;
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockID == id);
        }






        //POST api/Stocks/Use/
        [HttpPost("Use")]
        public async Task<IActionResult> UseStock([FromBody] StockUse stockuse)
        {
            int stockID = stockuse.StockID;
            int usedQuantity = stockuse.UsedQuantity;


            Stock stock = _context.Stocks.Include(s => s.Product).Single(s => s.StockID == stockID);

            if (stock == null)
            {
                return NotFound();
            }

            int availableQuantity = stock.Quantity;

            if (availableQuantity < usedQuantity) {
                var err = new { status = "error", message = "Used quantity can not be larger than the available quantity." };
                return BadRequest(err);
            }

            stock.Quantity = stock.Quantity - usedQuantity;

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(stock.StockID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (stock.Quantity <= stock.Threshold)
            {
                await CreateStockAlertAsync(stock, "THRESHOLD", "MIDEUM");
            }

            var msg = new { status = "Success", message = "Product quantity reduced by " + usedQuantity, AvailableQuantity = stock.Quantity };
            return Ok(msg);
        }



        private async Task CheckExpiryDates()
        {

            List<Stock> allstocks = await _context.Stocks.ToListAsync();
            foreach (var item in allstocks)
            {
                DateTime Today = DateTime.Now;
                DateTime ExpDate = item.ExpiryDate;

                double DateDiff = (ExpDate - Today).TotalDays;

                if (DateDiff < 0)
                {
                    await CreateStockAlertAsync(item, "EXPIRED", "VERYHIGH");                    
                }
                else if(DateDiff<10)
                {
                    await CreateStockAlertAsync(item, "NEAR_EXPIRE", "HIGH");
                }

            }

        }







        private async Task CreateStockAlertAsync(Stock stock,string alertType, string priority)
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

        [HttpGet("Report")]
        public async Task GenerateStockAlertReport()
        {
            List<StockAlert> StockAlerts = await _context.StockAlerts.Include(a => a.Stock.Product).Where(sa => sa.Status == "NEW").ToListAsync();

            IDictionary<int, AlertReport> Alertdict = new Dictionary<int, AlertReport>();

            foreach (var alert in StockAlerts)
            {
                int productID = alert.Stock.ProductID;
                AlertReport alertreport = null;
                bool available = Alertdict.TryGetValue(productID, out alertreport);

                if (available)
                {


                    List<int> StockIDList = alertreport.StockIDs;
                    StockIDList.Add(alert.StockID);
                    AlertReport newalertReport = new AlertReport
                    {
                        ProductID = productID,
                        ProductName = alert.Stock.Product.ProductName,
                        StockIDs = StockIDList
                    };
                    Alertdict[productID] = newalertReport;
                }
                else
                {
                    List<int> StockIDList = new List<int>();
                    StockIDList.Add(alert.StockID);
                    AlertReport newalertReport = new AlertReport
                    {
                        ProductID = productID,
                        ProductName = alert.Stock.Product.ProductName,
                        StockIDs = StockIDList
                    };
                    Alertdict.Add(productID, newalertReport);
                }

            }

            List<AlertReport> alertReportList = Alertdict.Values.ToList();
            AlertReportMessage alertReportMessage = new AlertReportMessage
            {
                ReportDate = DateTime.Now,
                AlertReportObj = string.Join(", ", JsonConvert.SerializeObject(alertReportList)),
            };

            _context.AlertReports.Add(alertReportMessage);
            await _context.SaveChangesAsync();


        }





     
    }


}
