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
        BAppInfo bllInfo;

        /// <summary>
        /// constructed function
        /// </summary>
        /// <param name="option"></param>
        public RewardController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
            bllInfo = new BAppInfo(connStr);
        }

        /// <summary>
        ///  Get earnings list by dpos address and date
        /// </summary>
        /// <param name="appID">appID</param>
        /// <param name="dpos_addr">super node address</param>
        /// <param name="date">formate:2020-01-01</param>
        /// <returns>list</returns>
        [HttpGet]
        public async Task<string> Get(string appID, string dpos_addr, DateTime date)
        {
            Result<DposDailyRewardLst> res = new Result<DposDailyRewardLst>();
            BReward bll = new BReward(connStr);
            try
            {
                string secretKey = string.Empty;
                var moInfo = await bllInfo.GetAppInfo(appID);
                if (moInfo == null)
                {
                    string str = "The appID can't be null.";
                    if (!string.IsNullOrEmpty(appID))
                    {
                        str = "There's no such app that appID is " + appID + ".";
                    }
                    res = new Result<DposDailyRewardLst>(ResultCode.Fail, str, null);
                    return JsonConvert.SerializeObject(res);
                }
                secretKey = moInfo.secretKey.Trim();

                var query = bll.GetDposDailyReward(dpos_addr, date);
                var lst = (await query).ToList();
                var mo = lst[0];

                var hashObj = mo.id + ":" + mo.dpos_addr + ":" + mo.client_addr + ":" + mo.payment_money + ":" + mo.payment_date.ToString("yyyyMMdd");
                var hash = Encrypt.HmacSHA256(secretKey, hashObj);

                DposDailyRewardLst moRe = new DposDailyRewardLst();
                moRe.hash = hash;
                moRe.hashObj = hashObj;

                if (lst.Count > 0)
                {
                    moRe.lst = lst;
                    res = new Result<DposDailyRewardLst>(ResultCode.Ok, null, moRe);
                }
                else
                {
                    res = new Result<DposDailyRewardLst>(ResultCode.NoRecord, null, null);
                }
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<DposDailyRewardLst>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }
    }
}
