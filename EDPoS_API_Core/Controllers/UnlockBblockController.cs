using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public class UnlockBblockController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;
        static int zone = 0;

        /// <summary>
        /// constructed function
        /// </summary>
        /// <param name="option"></param>
        public UnlockBblockController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
            zone = int.Parse(AppConfigurtaionServices.Configuration.GetSection("Zone").Value);
        }

        /// <summary>
        /// Get unlocked blocks by parameters
        /// </summary>
        /// <param name="timeSpanMin"></param>
        /// <param name="timeSpanMax"></param>
        /// <param name="addrFrom"></param>
        /// <param name="addrTo"></param>
        /// <returns></returns>
        [HttpGet("/api/[controller]/unlockBlockLst")]
        public async Task<string> Get(string timeSpanMin, string timeSpanMax, string addrFrom = "", string addrTo = "")
        {
            BUnlockBlock bll = new BUnlockBlock(connStr);
            Result<List<MUnlockBlockLst>> res = new Result<List<MUnlockBlockLst>>();

            try
            {
                var lst = await bll.GetLst(timeSpanMin, timeSpanMax, addrFrom, addrTo);
                res = new Result<List<MUnlockBlockLst>>(ResultCode.Ok, null, lst);
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<MUnlockBlockLst>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }

        /// <summary>
        /// Get unlockBlocks By parameters,there are more params
        /// </summary>
        /// <param name="addrFrom"></param>
        /// <param name="addrTo"></param>
        /// <param name="date">If it has this param, then the param timeSpan is invalid</param>
        /// <param name="timeSpan"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [HttpGet("/api/[controller]/unlockBlocks")]
        public async Task<string> Get(string addrFrom = "", string addrTo = "", string date = "", long timeSpan = 0, int height = 0)
        {
            BUnlockBlock bll = new BUnlockBlock(connStr);
            Result<List<MUnlockBlock>> res = new Result<List<MUnlockBlock>>();

            try
            {
                var lst = await bll.GetLst(addrFrom, addrTo, date, timeSpan, height, zone);
                res = new Result<List<MUnlockBlock>>(ResultCode.Ok, null, lst);
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<MUnlockBlock>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }

        /// <summary>
        /// Submit UnlockBlock info
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Post([FromBody] MUnlockBlockLstWithSign obj)
        {
            Result<int> res = new Result<int>();
            try
            {
                BAppInfo bll_info = new BAppInfo(connStr);
                BUnlockBlock bllp = new BUnlockBlock(connStr);
                
                var mo = await bll_info.GetAppInfo(obj.appID);
                if (mo == null)
                {
                    res = new Result<int>(ResultCode.Fail, " There's no such app that appID is " + obj.appID, 0);
                    return JsonConvert.SerializeObject(res);
                }
                string secretKey = mo.secretKey.Trim();
                var isOK = DataValid.Valid(obj.requestSign, obj.appID, obj.signPlain, obj.timeSpan, secretKey);

                if (isOK)
                {
                    var lst = obj.balanceLst;
                    var tasks = new List<Task>();
                    foreach (var v in lst)
                    {
                        //Thread.Sleep(10);
                        tasks.Add(Task.Factory.StartNew(() => bllp.InsertOne(v, zone)));
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
