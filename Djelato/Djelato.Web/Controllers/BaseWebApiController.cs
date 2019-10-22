using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Djelato.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Djelato.Web.Controllers
{
    public class BaseWebApiController : Controller
    {
        protected BaseWebApiController()
        {
        }

        public IActionResult Success(object value, string message = null, bool isSucceeded = true)
        {
            ResponseContent content = new ResponseContent() { 
                ErrorMessage = "", 
                Message = message, 
                Result = value, 
                IsSucceeded = isSucceeded 
            };
            var objectResult = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return new ContentResult
            {
                Content = objectResult,
                ContentType = "application/json",
                StatusCode = 200
            };
        }

        public IActionResult ClientError(string message = null, object value = null)
        {
            ResponseContent content = new ResponseContent { 
                ErrorMessage = message, 
                Message = "", 
                Result = value, 
                IsSucceeded = false 
            };
            var result = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return new ContentResult
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = 400
            };
        }

        public IActionResult Nonexistent(string message = null, object value = null)
        {
            ResponseContent content = new ResponseContent { 
                ErrorMessage = message, 
                Message = "", 
                Result = value, 
                IsSucceeded = false 
            };
            var result = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return new ContentResult
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = 404
            };
        }

        public IActionResult ServerError()
        {
            ResponseContent content = new ResponseContent
            {
                ErrorMessage = "Something went wrong. Try again later.",
                Message = "",
                Result = null,
                IsSucceeded = false
            };
            var result = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return new ContentResult
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = 500
            };
        }
    }
}