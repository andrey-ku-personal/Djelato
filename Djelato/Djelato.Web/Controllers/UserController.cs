using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Djelato.Web.Controllers
{
    [ApiController]
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add()
        {
            return Ok();
        }
    }
}