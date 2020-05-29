using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        /// <summary>
        /// Get Reward details by date, height, node address or voter address
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="height"></param>
        /// <param name="dpos_addr"></param>
        /// <param name="client_addr"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string dateTime = "", int height = 0, string dpos_addr = "", string client_addr = "")
        {
            Result<List<DposRewardDetails>> res = new Result<List<DposRewardDetails>>();
            Bll.BDpos bll = new Bll.BDpos(connStr);
            try
            {
                var query = bll.GetDposRewardDetails(dateTime, height, dpos_addr, client_addr);
                var list = (await query).ToList();
                res = new Result<List<DposRewardDetails>>(ResultCode.Ok, null, list);
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<DposRewardDetails>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }
    }
}
