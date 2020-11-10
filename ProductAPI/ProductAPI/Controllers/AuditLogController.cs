using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductAPI.Models;
using ProductAPI.Models.Context;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogController : ControllerBase
    {
        private readonly APIContext _context;

        public AuditLogController(APIContext context)
        {
            _context = context;
        }

        // GET: api/AuditLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogObject>>> GetLogObjects()
        {
            return await _context.LogObjects.ToListAsync();
        }

        // GET: api/AuditLog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogObject>> GetLogObject(int id)
        {
            var logObject = await _context.LogObjects.FindAsync(id);

            if (logObject == null)
            {
                return NotFound();
            }

            return logObject;
        }


        [HttpGet("compare/{id}")]
        public async Task<ActionResult<List<ChangeLog>>> CompareLog(int id)
        {
            LogObject currentLog = await _context.LogObjects.FindAsync(id);

            if (currentLog == null)
            {
                return NotFound();
            }
            else
            {
                string model = currentLog.Model;
                int entityid = currentLog.EntityID;
                string currentobj = currentLog.Object;
                string action = currentLog.Action;

               
                switch (model)
                {
                    case "Account":
                        
                        AppUser currentappuser = JsonConvert.DeserializeObject<AppUser>(currentobj);
                        var currentuser = new
                        {
                            Username = currentappuser.UserName,
                            Email = currentappuser.Email,
                            Fullname = currentappuser.FullName,
                            Address = currentappuser.Address,
                            Type = currentappuser.Type,
                            EmailConfirmed = currentappuser.EmailConfirmed,
                            PhoneNumber = currentappuser.PhoneNumber,
                            TwoFactorEnabled = currentappuser.TwoFactorEnabled
                        };
                        
                        var emptyuser = new
                        {
                            Username = "-",
                            Email = "-",
                            Fullname = "-",
                            Address = "-",
                            Type = "-",
                            EmailConfirmed = false,
                            PhoneNumber = "-",
                            TwoFactorEnabled = false
                        };
                        switch (action)
                        {
                            case "REGISTER":
                                return GetChanges(emptyuser, currentuser); 
                            case "LOGIN":
                                return GetChanges(currentuser, currentuser);
                            case "UPDATE":
                                LogObject prevLog = _context.LogObjects.OrderByDescending(log => log.Date).Where(log => log.Date < currentLog.Date).Where(log => log.Model == currentLog.Model).Where(log => log.EntityID == currentLog.EntityID).FirstOrDefault();
                                AppUser prevappuser = JsonConvert.DeserializeObject<AppUser>(prevLog.Object);
                                var prevuser = new
                                {
                                    Username = prevappuser.UserName,
                                    Email = prevappuser.Email,
                                    Fullname = prevappuser.FullName,
                                    Address = prevappuser.Address,
                                    Type = prevappuser.Type,
                                    EmailConfirmed = prevappuser.EmailConfirmed,
                                    PhoneNumber = prevappuser.PhoneNumber,
                                    TwoFactorEnabled = prevappuser.TwoFactorEnabled
                                };
                                
                                return GetChanges(prevuser, currentuser);
                        }

                        return BadRequest();

                       


                    case "Product":
                    
                        ProductLogObj currentproduct = JsonConvert.DeserializeObject<ProductLogObj>(currentobj);


                        ProductLogObj emptyproduct = new ProductLogObj
                        {
                            ProductID = 0,
                            ProductName = "-",
                            UnitPrice = 0,
                            ProductTypeID = 0,
                            ProductTypeName = null,
                            ProductTypeDescription = "-",
                            ImageName = "-"
                        };

                        switch (action)
                        {
                            case "GET":
                                return GetChanges(currentproduct, currentproduct); 
                            case "POST":
                                return GetChanges(emptyproduct, currentproduct);
                            case "PUT":
                                LogObject prevLog2 = _context.LogObjects.OrderByDescending(log => log.Date).Where(log => log.Date < currentLog.Date).Where(log => log.Model == currentLog.Model).Where(log => log.EntityID == currentLog.EntityID).FirstOrDefault();
                                ProductLogObj prevproduct = JsonConvert.DeserializeObject<ProductLogObj>(prevLog2.Object);
                                return GetChanges(prevproduct, currentproduct);
                            case "DELETE":
                                return GetChanges(currentproduct, emptyproduct);
                        }

                        return BadRequest();
                    default:
                        return Ok();
                }
            }
        }



        private List<ChangeLog> GetChanges(object oldEntry, object newEntry)
        {
            List<ChangeLog> logs = new List<ChangeLog>();

            var oldType = oldEntry.GetType();
            var newType = newEntry.GetType();
            if (oldType != newType)
            {
                return logs; //Types don't match, cannot log changes
            }

            var oldProperties = oldType.GetProperties();
            var newProperties = newType.GetProperties();

            //var dateChanged = DateTime.Now;
            //var primaryKey = (int)oldProperties.Where(x => Attribute.IsDefined(x, typeof(Key))).First().GetValue(oldEntry);
            var className = oldEntry.GetType().Name;

            foreach (var oldProperty in oldProperties)
            {

                var matchingProperty = newProperties.Where(x => x.Name == oldProperty.Name
                                                                && x.PropertyType == oldProperty.PropertyType)
                                                    .FirstOrDefault();
                //!Attribute.IsDefined(x, typeof(IgnoreLoggingAttribute))&&
                if (matchingProperty == null)
                {
                    continue;
                }
                var oldValue = oldProperty.GetValue(oldEntry).ToString();
                var newValue = matchingProperty.GetValue(newEntry).ToString();
                if (matchingProperty != null && oldValue != newValue)
                {
                    logs.Add(new ChangeLog()
                    {
                        //PrimaryKey = primaryKey,
                        //DateChanged = dateChanged,
                        ModelName = className,
                        PropertyName = matchingProperty.Name,
                        OldValue = oldProperty.GetValue(oldEntry).ToString(),
                        NewValue = matchingProperty.GetValue(newEntry).ToString()
                    });
                }
            }

            return logs;
        }





    }
}
