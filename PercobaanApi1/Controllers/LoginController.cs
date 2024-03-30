using Microsoft.AspNetCore.Mvc;
using PercobaanApi1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PercobaanApi1.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("API/LOGIN")]
        public IActionResult LoginUser(string namaUser, string password)
        {
            var context = new LoginContext(_config.GetConnectionString("WebApiDatabase"));

            if (context.IsValidUser(namaUser, password))
            {
                return Ok(new { token = context.GenerateJwtToken(namaUser, _config) }); // Mengembalikan token
            }

            return Unauthorized();  // Respon Unauthorized jika login gagal
        }
    }
}
