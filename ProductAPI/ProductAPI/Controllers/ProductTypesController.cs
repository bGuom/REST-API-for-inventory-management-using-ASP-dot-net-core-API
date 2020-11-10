using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
using ProductAPI.Models.Context;
using ProductAPI.Models.ViewModel;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypesController : ControllerBase
    {
        private readonly APIContext _context;

        public ProductTypesController(APIContext context)
        {
            _context = context;
        }

        // GET: api/ProductTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Object>>> GetProductTypes()
        {
            List<Object> producttypelist = new List<Object>();

            List<ProductType> allproducttypes = await _context.ProductTypes.ToListAsync();

            foreach (var item in allproducttypes)
            {

                var returnproducttype = new
                {
                    ProductTypeID = item.ProductTypeID,
                    TypeName = item.TypeName,
                    Description = item.Description

                };

                producttypelist.Add(returnproducttype);

            }

            return producttypelist;
        }

        // GET: api/ProductTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Object>> GetProductType(int id)
        {
            var productType = await _context.ProductTypes.FindAsync(id);

            if (productType == null)
            {
                return NotFound();
            }

            var returnproducttype = new
            {
                ProductTypeID = productType.ProductTypeID,
                TypeName = productType.TypeName,
                Description = productType.Description

            };

            return returnproducttype;
        }

        // PUT: api/ProductTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductTypes(int id, ProductType productType)
        {
            if (id != productType.ProductTypeID)
            {
                return BadRequest();
            }

            _context.Entry(productType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTypeExists(id))
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

        // POST: api/ProductTypes
        [HttpPost]
        public async Task<ActionResult<ProductType>> PostStock(ProductType productTypes)
        {
            _context.ProductTypes.Add(productTypes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductType", new { id = productTypes.ProductTypeID }, productTypes);
        }

        // DELETE: api/ProductTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductType>> DeleteStock(int id)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            _context.ProductTypes.Remove(productType);
            await _context.SaveChangesAsync();

            return productType;
        }

        private bool ProductTypeExists(int id)
        {
            return _context.ProductTypes.Any(e => e.ProductTypeID == id);
        }
    }
}
