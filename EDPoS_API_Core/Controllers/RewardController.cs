using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDPoS_API_Core.Bll;
using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    /// <summary>
    /// About rewards
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RewardController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;

        /// <summary>
        /// constructed function
        /// </summary>
        /// <param name="option"></param>
        public RewardController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        /// <summary>
        ///  Get earnings list by dpos address and date
        /// </summary>
        /// <param name="dpos_addr">super node address</param>
        /// <param name="date">formate:2020-01-01</param>
        /// <returns>list</returns>
        [HttpGet]
        public async Task<string> Get(string dpos_addr, DateTime date)
        {
            Result<List<DposDailyReward>> res = new Result<List<DposDailyReward>>();
            BReward bll = new BReward(connStr);
            try
            {
                var query = bll.GetDposDailyReward(dpos_addr, date);
                var lst = (await query).ToList();

                if (lst.Count > 0)
                {
                    res = new Result<List<DposDailyReward>>(ResultCode.Ok, null, lst);
                }
                else
                {
                    res = new Result<List<DposDailyReward>>(ResultCode.NoRecord, null, null);
                }
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<DposDailyReward>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }
    }
}
