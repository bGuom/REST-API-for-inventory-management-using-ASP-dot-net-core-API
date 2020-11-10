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
    public class SalesRepsController : ControllerBase
    {
        private readonly APIContext _context;

        public SalesRepsController(APIContext context)
        {
            _context = context;
        }

        // GET: api/SalesReps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesRep>>> GetSalesReps()
        {
            return await _context.SalesReps.ToListAsync();
        }

        // GET: api/SalesReps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesRep>> GetSalesRep(int id)
        {
            var salesRep = await _context.SalesReps.FindAsync(id);

            if (salesRep == null)
            {
                return NotFound();
            }

            return salesRep;
        }

        // PUT: api/SalesReps/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesRep(int id, SalesRep salesRep)
        {
            if (id != salesRep.RepID)
            {
                return BadRequest();
            }

            _context.Entry(salesRep).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesRepExists(id))
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

        // POST: api/SalesReps
        [HttpPost]
        public async Task<ActionResult<SalesRep>> PostSalesRep(SalesRep salesRep)
        {
            _context.SalesReps.Add(salesRep);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSalesRep", new { id = salesRep.RepID }, salesRep);
        }

        // DELETE: api/SalesReps/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SalesRep>> DeleteSalesRep(int id)
        {
            var salesRep = await _context.SalesReps.FindAsync(id);
            if (salesRep == null)
            {
                return NotFound();
            }

            _context.SalesReps.Remove(salesRep);
            await _context.SaveChangesAsync();

            return salesRep;
        }

        private bool SalesRepExists(int id)
        {
            return _context.SalesReps.Any(e => e.RepID == id);
        }
    }
}
