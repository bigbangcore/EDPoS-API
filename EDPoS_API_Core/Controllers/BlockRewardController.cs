using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDPoS_API_Core.Bll;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    /// <summary>
    /// About Block Reward
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BlockRewardController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        public BlockRewardController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        /// <summary>
        /// GET: api/BlockReward
        /// </summary>
        /// <param name="template_addr">template addr</param>
        /// <param name="date">eg. 2020-09-09</param>
        /// <param name="ConsensusType">Consensus Type：primary-pow / primary-dpos, Default:primary-pow</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string template_addr, string date, string ConsensusType = "primary-pow")
        {
            Result<List<MBlockPa>> res = new Result<List<MBlockPa>>();
            var tdate = DateTime.Now;
            if (!DateTime.TryParse(date, out tdate) || string.IsNullOrEmpty(date))
            {
                res = new Result<List<MBlockPa>>(ResultCode.Fail, "Parameter date is not a DateTime type.", null);
                return JsonConvert.SerializeObject(res);
            }

            BCommon bll = new BCommon(connStr);
            try
            {
                var query = bll.GetBlockDailyReward(template_addr, date, ConsensusType);
                var lst = (await query).ToList();

                if (lst.Count > 0)
                {
                    res = new Result<List<MBlockPa>>(ResultCode.Ok, null, lst);
                }
                else
                {
                    res = new Result<List<MBlockPa>>(ResultCode.NoRecord, null, null);
                }
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<MBlockPa>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }
    }
}
