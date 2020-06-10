using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppIdController : ControllerBase
    {
        /// <summary>
        /// Get a appID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {
            Result<string> res = new Result<string>();
            res = new Result<string>(ResultCode.Ok, null, Guid.NewGuid().ToString());
            return JsonConvert.SerializeObject(res);
        }
    }
}