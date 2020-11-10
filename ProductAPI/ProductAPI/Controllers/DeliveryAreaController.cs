using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models.Context;
using ProductAPI.Models.DBModels;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryAreaController : ControllerBase
    {
        private readonly APIContext _context;

        public DeliveryAreaController(APIContext context)
        {
            _context = context;
        }

        // GET: api/DeliveryArea
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryArea>>> GetDeliveryAreas()
        {
            return await _context.DeliveryAreas.ToListAsync();
        }

        // GET: api/DeliveryArea/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryArea>> GetDeliveryArea(int id)
        {
            var deliveryArea = await _context.DeliveryAreas.FindAsync(id);

            if (deliveryArea == null)
            {
                return NotFound();
            }

            return deliveryArea;
        }

        // PUT: api/DeliveryArea/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeliveryArea(int id, DeliveryArea deliveryArea)
        {
            if (id != deliveryArea.DeliveryID)
            {
                return BadRequest();
            }

            _context.Entry(deliveryArea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryAreaExists(id))
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

        // POST: api/DeliveryArea
        [HttpPost]
        public async Task<ActionResult<DeliveryArea>> PostDeliveryArea(DeliveryArea deliveryArea)
        {
            _context.DeliveryAreas.Add(deliveryArea);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeliveryArea", new { id = deliveryArea.DeliveryID }, deliveryArea);
        }

        // DELETE: api/DeliveryArea/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeliveryArea>> DeleteDeliveryArea(int id)
        {
            var deliveryArea = await _context.DeliveryAreas.FindAsync(id);
            if (deliveryArea == null)
            {
                return NotFound();
            }

            _context.DeliveryAreas.Remove(deliveryArea);
            await _context.SaveChangesAsync();

            return deliveryArea;
        }

        private bool DeliveryAreaExists(int id)
        {
            return _context.DeliveryAreas.Any(e => e.DeliveryID == id);
        }
    }
}
