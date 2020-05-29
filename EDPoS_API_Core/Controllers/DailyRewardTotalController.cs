using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EDPoS_API_Core.Bll;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyRewardTotalController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;

        /// <summary>
        /// constructed function
        /// </summary>
        /// <param name="option"></param>
        public DailyRewardTotalController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        /// <summary>
        /// Get total rewards by EDPoS address or date
        /// </summary>
        /// <param name="dpos_addr"></param>
        /// <param name="date">eg. 2020-05-12</param>
        /// <param name="isAll"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string dpos_addr, string date, bool isAll = false)
        {
            Result<List<DposAddrDaily>> res = new Result<List<DposAddrDaily>>();
            try
            {                
                BReward bll = new BReward(connStr);
                var lst = (await bll.GetTotalReward(dpos_addr, date)).ToList();
                if (lst.Count > 0)
                {
                    if (isAll && string.IsNullOrEmpty(date))
                    {
                        var dpos = new DposAddrDailyAll();
                        foreach (var v in lst)
                        {
                            dpos.payment_money += v.payment_money;
                        }
                        if (!string.IsNullOrEmpty(dpos_addr))
                        {
                            dpos.dpos_addr = dpos_addr.Trim();
                        }
                        else
                        {
                            dpos.dpos_addr = "All EDPoS Address";
                        }
                        
                        var ress = new Result<DposAddrDailyAll>(ResultCode.Ok, null, dpos);
                        return JsonConvert.SerializeObject(ress);
                    }
                    else
                    {
                        res = new Result<List<DposAddrDaily>>(ResultCode.Ok, null, lst);
                    }
                }
                else
                {
                    res = new Result<List<DposAddrDaily>>(ResultCode.NoRecord, null, null);
                }
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<DposAddrDaily>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }
    }
}