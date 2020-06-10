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

namespace EDPoS_API_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowPoolDailyRewardController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;

        /// <summary>
        /// constructed function
        /// </summary>
        /// <param name="option"></param>
        public PowPoolDailyRewardController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        /// <summary>
        /// Get one by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Get")]
        public async Task<string> Get(long id)
        {
            BPowPoolDailyReward bll = new BPowPoolDailyReward(connStr);
            Result<List<MPowPoolDailyReward>> res = new Result<List<MPowPoolDailyReward>>();
            try
            {
                var lst = await bll.GetOne(id);
                res = new Result<List<MPowPoolDailyReward>>(ResultCode.Ok, null, lst);
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<MPowPoolDailyReward>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }

        /// <summary>
        /// Get info by addrFrom, addrTo or settlement date
        /// </summary>
        /// <param name="appID">appID</param>
        /// <param name="addrFrom">wallet address : from</param>
        /// <param name="addrTo">wallet address : to</param>
        /// <param name="date">2020-06-04</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string appID, string addrFrom = "", string addrTo = "", string date = "")
        {
            BPowPoolDailyReward bll = new BPowPoolDailyReward(connStr);
            Result<MPowPoolDailyRewardWithHash> res = new Result<MPowPoolDailyRewardWithHash>();
            BAppInfo bllInfo = new BAppInfo(connStr);

            try
            {
                var lst = await bll.GetBySomething(addrFrom, addrTo, date);
                MPowPoolDailyRewardWithHash mo = new MPowPoolDailyRewardWithHash();
                mo.rewardLst = lst;
                var moTmp = lst[0];
                var hashObj = moTmp.id + ":" + moTmp.addrFrom + ":" + moTmp.addrTo + ":" + moTmp.reward + DateTime.Parse(moTmp.settlementDate).ToString("yyyyMMdd");

                mo.hashObj = hashObj;
                var moInfo = await bllInfo.GetAppInfo(appID);
                if (moInfo == null)
                {
                    string str = "The appID can't be null.";
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = "There's no such app that appID is " + appID + ".";
                    }
                    res = new Result<MPowPoolDailyRewardWithHash>(ResultCode.Fail, str, null);
                    return JsonConvert.SerializeObject(res);
                }
                string secretKey = moInfo.secretKey.Trim();

                mo.hash = Encrypt.HmacSHA256(secretKey, hashObj); //使用私钥对lst签名
                res = new Result<MPowPoolDailyRewardWithHash>(ResultCode.Ok, null, mo);
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<MPowPoolDailyRewardWithHash>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }

        /// <summary>
        /// Insert Reward Info into PowPoolDailyReward
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Return the count of inserted info</returns>
        [HttpPost]
        public async Task<string> Post([FromBody] MPowPoolDailyRewardWithValid value)
        {
            BAppInfo bll = new BAppInfo(connStr);
            BPowPoolDailyReward bllp = new BPowPoolDailyReward(connStr);
            Result<int> res = new Result<int>();
            try
            {
                var mo = await bll.GetAppInfo(value.appID);
                if (mo == null)
                {
                    res = new Result<int>(ResultCode.Fail, " There's no such app that appID is " + value.appID, 0);
                    return JsonConvert.SerializeObject(res);
                }
                string secretKey = mo.secretKey.Trim();
                var isOK = DataValid.Valid(value.requestSign, value.appID, value.signPlain, value.timeSpan, secretKey);
                if (isOK)
                {
                    //insert into PowPoolDailyReward
                    //var re = await bllp.InsertLst(value.rewardLst);
                    int j = 0;
                    foreach (var item in value.rewardLst)
                    {
                        var re = await bllp.InsertOne(item);
                        if (re)
                        {
                            j++;
                        }
                    }
                    res = new Result<int>(j > 0 ? ResultCode.Ok : ResultCode.Fail, null, j);
                }
                else
                {
                    res = new Result<int>(ResultCode.Fail, null, 0);                    
                }
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<int>(ResultCode.Fail, ex.Message, 0);
                return JsonConvert.SerializeObject(res);
            }            
        }
    }
}
