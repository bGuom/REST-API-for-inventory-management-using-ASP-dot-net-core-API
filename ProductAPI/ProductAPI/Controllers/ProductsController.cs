using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProductAPI.Models;
using ProductAPI.Models.Context;
using ProductAPI.Models.ViewModel;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JWT.AuthScheme)]
    [EnableCors("_myAllowSpecificOrigins")]

    public class ProductsController : ControllerBase
    {
        private readonly APIContext _context;

        private readonly ApplicationSettings Appsettings;
        private readonly IHostingEnvironment HostingEnvironment;
        private readonly string UploadPath;
        private readonly string URLPath;

        public ProductsController(APIContext context, IOptions<ApplicationSettings> appsettings, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            Appsettings = appsettings.Value;
            HostingEnvironment = hostingEnvironment;

            string relativepath = Appsettings.ProductImageStoragePath;
            UploadPath = Path.Combine(HostingEnvironment.WebRootPath, relativepath);
            URLPath = Path.Combine(Appsettings.baseURL, relativepath);
        }

        [HttpGet("info")]
        public string getta() {
           
            return "You can access products.";
        }

       



        // GET: api/Products
        [HttpGet]
        public IEnumerable<ProductReturn> GetProducts()
        {

            List<ProductReturn> productlist = new List<ProductReturn>();
            
            List<Product> allproducts = _context.Products.Include(c=>c.ProductTypeName).ToList();
            foreach (var item in allproducts)
            {
                string imageurl = null;
                if (item.ImageName != null)
                {
                    imageurl = Path.Combine(URLPath, item.ImageName);
                    imageurl=imageurl.Replace("\\", "/");
                }
          

                ProductReturn returnproduct = new ProductReturn
                {
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    ProductTypeID = item.ProductTypeID,
                    ProductTypeName = item.ProductTypeName.TypeName,
                    ImageUrl = imageurl
                };

                productlist.Add(returnproduct);
                
            }


            return productlist;
        }







        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _context.Products.Include(c => c.ProductTypeName).Where(p => p.ProductID == id).Single();
            //var product = await _context.Products.Include(i => i.ProductTypeName).FirstOrDefaultAsync(i => i.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                string imageurl = null;
                if (product.ImageName != null)
                {
                    imageurl = Path.Combine(URLPath, product.ImageName);
                    imageurl = imageurl.Replace("\\", "/");
                }

                ProductReturn returnproduct = new ProductReturn
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice,
                    ProductTypeID = product.ProductTypeID,
                    ProductTypeName = product.ProductTypeName.TypeName,
                    ImageUrl = imageurl
                };



                await LogChangeAsync(product, "GET");
                return Ok(returnproduct);
            }
        }








        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product)
        {
            string usertype = User.Claims.First(c => c.Type == "UserType").Value;
            if (usertype.Equals("Admin"))
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != product.ProductID)
                {
                    return BadRequest();
                }

                _context.Entry(product).State = EntityState.Modified;
                await LogChangeAsync(product, "PUT");

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
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
            else {
                //var err = new { status = "Unautharized access", message = "Only Admin can update" };
                return Unauthorized();
          
            }
        }









        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromForm] CreateProductViewModel model)
        {
            string usertype = User.Claims.First(c => c.Type == "UserType").Value;
            if (usertype.Equals("Admin"))
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string uniqueName = null;
                if (model.Image != null)
                {

                    
                    bool exists = System.IO.Directory.Exists(UploadPath);

                    if (!exists) { System.IO.Directory.CreateDirectory(UploadPath); }
                        
                    uniqueName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    string filepath = Path.Combine(UploadPath, uniqueName);

                    model.Image.CopyTo(new FileStream(filepath, FileMode.Create));

                }

                ProductType producttype = _context.ProductTypes.Single(i => i.ProductTypeID == model.ProductTypeID);

                Product newproduct = new Product
                {
                    ProductName = model.ProductName,
                    UnitPrice = model.UnitPrice,
                    ProductTypeID = model.ProductTypeID,
                    ProductTypeName = producttype,
                    ImageName = uniqueName
                };

                string imageurl = null;
                imageurl = Path.Combine(URLPath, newproduct.ImageName);
                imageurl = imageurl.Replace("\\", "/");

                




                _context.Products.Add(newproduct);
                await _context.SaveChangesAsync();
                await LogChangeAsync(newproduct, "POST");

                ProductReturn returnproduct = new ProductReturn
                {
                    ProductID = newproduct.ProductID,
                    ProductName = newproduct.ProductName,
                    UnitPrice = newproduct.UnitPrice,
                    ProductTypeID = newproduct.ProductTypeID,
                    ProductTypeName = newproduct.ProductTypeName.TypeName,
                    ImageUrl = imageurl
                };

                return CreatedAtAction("GetProduct", new { id = newproduct.ProductID }, returnproduct);
            }
            else
            {
                //var err = new { status = "Unautharized access", message = "Only Admin can create" };
                return Unauthorized();
            }

        }









        // POST: api/Products/upload








        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            string usertype = User.Claims.First(c => c.Type == "UserType").Value;
            if (usertype.Equals("Admin"))
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    {
                        return NotFound();
                    }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                await LogChangeAsync(product, "DELETE");
                return Ok(product);
            }
            else
            {
                //var err2 = new { status = "Unautharized access", message = "Only Admin can delete" };
                return Unauthorized();
            }
        }




        // GET: api/Products/type/1
        [HttpGet("type/{id}")]
        public IList<ProductReturn> GetProductType([FromRoute] int id)
        {
            ProductType producttype = _context.ProductTypes.Include(p => p.Products).Single(pt => pt.ProductTypeID == id);


            List<ProductReturn> productlist = new List<ProductReturn>();

            List<Product> allproducts = producttype.Products.ToList();
            foreach (var item in allproducts)
            {
                string imageurl = null;
                if (item.ImageName != null)
                {
                    imageurl = Path.Combine(URLPath, item.ImageName);
                    imageurl = imageurl.Replace("\\", "/");
                }


                ProductReturn returnproduct = new ProductReturn
                {
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    ProductTypeID = item.ProductTypeID,
                    ProductTypeName = item.ProductTypeName.TypeName,
                    ImageUrl = imageurl
                };

                productlist.Add(returnproduct);

            }


            return productlist;

        }








        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);

        }




        private async Task LogChangeAsync(Product obj, String action)
        {
            ProductLogObj productLogObj = new ProductLogObj
            {
                ProductID = obj.ProductID,
                ProductName = obj.ProductName,
                UnitPrice = obj.UnitPrice,
                ProductTypeID = obj.ProductTypeID,
                ProductTypeName = obj.ProductTypeName.TypeName,
                ProductTypeDescription = obj.ProductTypeName.Description,
                ImageName = obj.ImageName
            };

            LogObject logobj = new LogObject
            {
                Model = "Product",
                EntityID = obj.ProductID,
                Date = DateTime.Now,
                Action = action,
                Object = JsonConvert.SerializeObject(productLogObj)
        };

            _context.LogObjects.Add(logobj);
            await _context.SaveChangesAsync();

        }
    }
}