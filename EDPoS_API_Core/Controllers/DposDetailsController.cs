using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    /// <summary>
    /// EDPoS details
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DposDetailsController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;

        public DposDetailsController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        // GET: api/DposDetails
        [HttpGet]
        public async Task<string> Get(string dateTime = "", int height = 0, string dpos_addr = "", string client_addr = "")
        {
            Result<List<DposRewardDetails>> res = new Result<List<DposRewardDetails>>();
            Bll.BCommon bll = new Bll.BCommon(connStr);
            var query = bll.GetDposRewardDetails(dateTime, height, dpos_addr, client_addr);
            var list = (await query).ToList();
            res = new Result<List<DposRewardDetails>>(ResultCode.Ok, null, list);
            return JsonConvert.SerializeObject(res);
        }
    }
}
