using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using ProductAPI.Models.Context;
using ProductAPI.Models.DBModels;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockDeliveriesController : ControllerBase
    {
        private readonly APIContext _context;

        public StockDeliveriesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/StockDeliveries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDelivery>>> GetStockDeliveries()
        {
            return await _context.StockDeliveries.ToListAsync();
        }

        // GET: api/StockDeliveries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockDelivery>> GetStockDelivery(int id)
        {
            var stockDelivery = await _context.StockDeliveries.FindAsync(id);

            if (stockDelivery == null)
            {
                return NotFound();
            }

            return stockDelivery;
        }

        // PUT: api/StockDeliveries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStockDelivery(int id, StockDelivery stockDelivery)
        {
            if (id != stockDelivery.DeliveryID)
            {
                return BadRequest();
            }

            _context.Entry(stockDelivery).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockDeliveryExists(id))
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

        // POST: api/StockDeliveries
        [HttpPost]
        public async Task<ActionResult<StockDelivery>> PostStockDelivery(StockDelivery stockDelivery)
        {
            _context.StockDeliveries.Add(stockDelivery);
            await _context.SaveChangesAsync();

            Stock stock = _context.Stocks.Single(s => s.StockID == stockDelivery.StockID );
            stock.DeliveryID = stockDelivery.DeliveryID;

            _context.Entry(stock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(stockDelivery.StockID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStockDelivery", new { id = stockDelivery.DeliveryID }, stockDelivery);
        }

        // DELETE: api/StockDeliveries/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StockDelivery>> DeleteStockDelivery(int id)
        {
            var stockDelivery = await _context.StockDeliveries.FindAsync(id);
            if (stockDelivery == null)
            {
                return NotFound();
            }

            _context.StockDeliveries.Remove(stockDelivery);
            await _context.SaveChangesAsync();

            return stockDelivery;
        }

        private bool StockDeliveryExists(int id)
        {
            return _context.StockDeliveries.Any(e => e.DeliveryID == id);
        }


        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockID == id);
        }
    }
}
