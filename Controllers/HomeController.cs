using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebApp_OpenIDConnect_DotNet.Models;

namespace WebApp_OpenIDConnect_DotNet.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(IHttpContextAccessor contextAccessor)
        {
            // Here HttpContext is not Null :)
            _httpContextAccessor = contextAccessor;
            var authenticatedUser = contextAccessor.HttpContext.User.Identity.Name;
        }
        public IActionResult Index()
        {   
            ViewData["Username"] = _httpContextAccessor.HttpContext.User.Identity.Name;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = User.Identity.Name;

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = User.Identity.Name;

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
