using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDPoS_API_Core.Bll;
using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EDPoS_API_Core.Controllers
{
    /// <summary>
    /// Valid data for request api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValidController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;

        /// <summary>
        /// constructed function
        /// </summary>
        /// <param name="option"></param>
        public ValidController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mop"></param>
        [HttpPost]
        public async Task<bool> Post(MValid mop)
        {
            BAppInfo bll = new BAppInfo(connStr);
            try
            {
                var mo = await bll.GetAppInfo(mop.appID);
                string secretKey = mo.secretKey.Trim();
                return DataValid.Valid(mop.requestSign, mop.appID, mop.signPlain, mop.timeSpan, secretKey);
            }
            catch (Exception)
            {
                return false;
            }            
        }
    }
}
