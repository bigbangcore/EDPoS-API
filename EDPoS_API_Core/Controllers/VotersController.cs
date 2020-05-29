using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDPoS_API_Core.Bll;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    /// <summary>
    /// About Voters information
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VotersController : Controller
    {
        private SiteConfig Config;
        static string connStr = string.Empty;
        public VotersController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        /// <summary>
        /// Get the Voting of voters by dposaddr,if has the voterAddr parameter,return the voter's information
        /// </summary>
        /// <param name="dposAddr"></param>
        /// <param name="voterAddr"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string dposAddr, string voterAddr = "")
        {
            Result<List<MVoters>> res = new Result<List<MVoters>>();
            try
            {
                BVoter bll = new BVoter(connStr);
                var lst = await bll.GetVoting(dposAddr, voterAddr);

                if (lst.Count > 0)
                {
                    res = new Result<List<MVoters>>(ResultCode.Ok, null, lst);
                }
                else
                {
                    res = new Result<List<MVoters>>(ResultCode.NoRecord, null, null);
                }
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<MVoters>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }
    }
}