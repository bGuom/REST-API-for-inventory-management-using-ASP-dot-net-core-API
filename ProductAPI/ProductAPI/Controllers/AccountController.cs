using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductAPI.Models;
using ProductAPI.Models.ViewModel;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using ProductAPI.Models.Context;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  

    public class AccountController : ControllerBase
    {

        private UserManager<AppUser> UserMgr { get; }
        private SignInManager<AppUser> SignInMgr { get; }

        private ApplicationSettings AppSettings { get; }
        private readonly APIContext _context;

        public AccountController(UserManager<AppUser> usermanager, SignInManager<AppUser> signinmanager, APIContext logcontext, IOptions<ApplicationSettings> appsettings) {
            UserMgr = usermanager;
            SignInMgr = signinmanager;
            _context = logcontext;
            AppSettings = appsettings.Value;
        }






        [HttpGet("info")]
        [Authorize(AuthenticationSchemes = JWT.AuthScheme)]
        public async Task<IActionResult> getinfoAsync()
        {
            string username = User.Claims.First(c => c.Type == "UserName").Value;
            AppUser user = await UserMgr.FindByNameAsync(username);
            var returnuser = new 
            {
                Username = user.UserName,
                Email = user.Email,
                Fullname = user.UserName,
                Address = user.Address,
                Type = user.Type,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                TwoFactorEnabled =user.TwoFactorEnabled
                                             
            };
            return Ok(returnuser);
        }










        [HttpPost("Auth")]
        public async Task<IActionResult> GenerateToken([FromBody]JWTViewModel m)
        {
            if (ModelState.IsValid)
            {

                AppUser user = await UserMgr.FindByNameAsync(m.UserName);
                if (user != null)
                {
                    var signInResult = await SignInMgr.CheckPasswordSignInAsync(user, m.Password.ToString(), false);

                    if (signInResult.Succeeded)
                    {
                        
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JWT_Secret));
                        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var claims = new[]
                        {
                            
                            new Claim(JwtRegisteredClaimNames.Sub, m.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                            new Claim("UserType",user.Type),
                            new Claim("UserName",m.UserName)

                        };

                        var token = new JwtSecurityToken(

                            JWT.Iss,
                            JWT.Aud,
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            signingCredentials: cred

                            );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };
                        await LogChangeAsync(user, "Login");
                        return Ok(results);

                    }
                    else
                    {
                        var err2 = new { status = "error", message = "Authentication Failed ! Check UserName & Password" };
                        return BadRequest(err2);
                    }
                }

                var err = new { status = "error", message = "Could not find a user!" };
                return BadRequest(err);
            }

            return BadRequest();
        }








        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterViewModel usermodel)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    AppUser user = await UserMgr.FindByNameAsync(usermodel.Username);
                    if (user == null)
                    {
                        user = new AppUser
                        {
                            UserName = usermodel.Username,
                            Email = usermodel.Email,
                            FullName = usermodel.FullName,
                            Address = usermodel.Address,
                            Type = "Customer"
                        };

                        IdentityResult result = await UserMgr.CreateAsync(user, usermodel.Password);
                        //await UserMgr.AddToRoleAsync(user, user.Type);
                        if (result.Succeeded)
                        {
                            await LogChangeAsync(user, "REGISTER");
                            return Created("",usermodel);
                        }
                        else
                        {
                            var err = new { status = "error", message = "User registration failed!" };
                            return BadRequest(err);
                        }

                    }
                    else
                    {
                        //User Already exsist
                        var err = new { status = "error", message = "User already exsist!" };
                        return BadRequest(err);
                    }
                }
                catch (Exception ex)
                {
                    var err = new { status = "error", message = ex.Message };
                    return BadRequest(err);
                }
            }
            else
            {
                var err = new { status = "error", message="Invalid details" };
                return BadRequest(err);
            }


        }

        private async Task LogChangeAsync(AppUser obj, String action)
        {
            var user = new
            {
                Username = obj.UserName,
                Email = obj.Email,
                Fullname = obj.UserName,
                Address = obj.Address,
                Type = obj.Type,
                EmailConfirmed = obj.EmailConfirmed,
                PhoneNumber = obj.PhoneNumber,
                TwoFactorEnabled = obj.TwoFactorEnabled

            };

            LogObject logobj = new LogObject
            {
                Model = "Account",
                EntityID = obj.Id,
                Date = DateTime.Now,
                Action = action,
                Object = JsonConvert.SerializeObject(user)
            };

            _context.LogObjects.Add(logobj);
            await _context.SaveChangesAsync();

        }

    }
}