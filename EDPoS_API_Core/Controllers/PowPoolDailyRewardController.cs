using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
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
        /// <param name="addrFrom">wallet address : from</param>
        /// <param name="addrTo">wallet address : to</param>
        /// <param name="date">2020-06-04</param>
        /// <param name="isNewWay">If you get data from unlockedBlock, set it as true</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string addrFrom = "", string addrTo = "", string date = "", bool isNewWay = false)
        {
            Result<List<MPowPoolDailyReward>> res = new Result<List<MPowPoolDailyReward>>();
            var lst = new List<MPowPoolDailyReward>();
            try
            {
                if (isNewWay)
                {
                    BUnlockBlock b = new BUnlockBlock(connStr);                    
                    if (!string.IsNullOrEmpty(date))
                    {
                        DateTime dt = new DateTime();
                        if (DateTime.TryParse(date, out dt))
                        {
                            var dateEnd = dt.AddDays(1);
                            var dStart = Convert.ToInt64(CommonHelper.GetTimeStamp(dt)) / 1000;
                            var dEnd = Convert.ToInt64(CommonHelper.GetTimeStamp(dateEnd)) / 1000;
                            lst = await b.GetLstSum(dStart.ToString(), dEnd.ToString(), addrFrom, addrTo);
                        }
                    }
                    res = new Result<List<MPowPoolDailyReward>>(ResultCode.Ok, null, lst);
                    return JsonConvert.SerializeObject(res);
                }
                else
                {
                    BPowPoolDailyReward bll = new BPowPoolDailyReward(connStr);
                    lst = await bll.GetBySomething(addrFrom, addrTo, date);
                    // 这里对相同地址的数据求和

                    var reLst = lst.GroupBy(p => new { p.addrFrom, p.addrTo, p.settlementDate })
                        .Select(g => new
                        {
                            id = 0,
                            addrFrom = g.Key.addrFrom,
                            addrTo = g.Key.addrTo,
                            settlementDate = g.Key.settlementDate,
                            reward = g.Sum(p => p.reward)
                        }).ToList();

                    var lst_tmp = new List<MPowPoolDailyReward>();
                    foreach (var item in reLst)
                    {
                        var mo = new MPowPoolDailyReward();
                        mo.addrFrom = item.addrFrom;
                        mo.addrTo = item.addrTo;
                        mo.id = 0;
                        mo.reward = item.reward;
                        mo.settlementDate = item.settlementDate;
                        lst_tmp.Add(mo);
                    }

                    res = new Result<List<MPowPoolDailyReward>>(ResultCode.Ok, null, lst_tmp);
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception ex)
            {
                res = new Result<List<MPowPoolDailyReward>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }                        
        }

        /// <summary>
        /// Insert Reward Info into PowPoolDailyReward
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Return the count of inserted info</returns>
        [HttpPost]
        public async Task<string> Post([FromBody] MPowPoolDailyRewardWithValid obj)
        {
            BAppInfo bll = new BAppInfo(connStr);
            BPowPoolDailyReward bllp = new BPowPoolDailyReward(connStr);
            Result<int> res = new Result<int>();
            try
            {
                var mo = await bll.GetAppInfo(obj.appID);
                if (mo == null)
                {
                    res = new Result<int>(ResultCode.Fail, " There's no such app that appID is " + obj.appID, 0);
                    return JsonConvert.SerializeObject(res);
                }
                string secretKey = mo.secretKey.Trim();
                var isOK = DataValid.Valid(obj.requestSign, obj.appID, obj.signPlain, obj.timeSpan, secretKey);
                if (isOK)
                {
                    //insert into PowPoolDailyReward
                    var lst = obj.rewardLst;
                    var tasks = new List<Task>();
                    foreach (var v in lst)
                    {
                        //Thread.Sleep(10);
                        tasks.Add(Task.Factory.StartNew(() => bllp.InsertOne(v)));
                    }

                    try
                    {
                        Task.WaitAll(tasks.ToArray());
                        res = new Result<int>(ResultCode.Ok, null, lst.Count);
                    }
                    catch (AggregateException ae)
                    {
                        var str = string.Empty;
                        foreach (var ex in ae.InnerExceptions)
                        {
                            str += "Name: " + ex.GetType().Name + ", msg:" + ex.Message + " | ";
                        }
                        res = new Result<int>(ResultCode.Fail, str, 0);
                    }
                    //var tr = bllp.InsertOne(obj.rewardLst[0]);
                    //res = new Result<int>(ResultCode.Ok, null, 1);
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
