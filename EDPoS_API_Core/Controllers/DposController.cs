using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDPoS_API_Core.Bll;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    /// <summary>
    /// About EDPoS
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DposController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;

        /// <summary>
        /// constructed function
        /// </summary>
        /// <param name="option"></param>
        public DposController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        /// <summary>
        ///  Get the list of users who participate in the dpos supernode campaign
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/[controller]/nodes")]
        public async Task<string> Get()
        {
            Result<List<Pool>> res = new Result<List<Pool>>();
            BDpos bll = new BDpos(connStr);
            try
            {
                var lst = await bll.GetPoolList();
                res = new Result<List<Pool>>(lst);
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<Pool>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }

        /// <summary>
        /// Get the reward list by datetime
        /// </summary>
        /// <param name="date">dataTime,formate:2020-01-01</param>
        /// <returns></returns>
        [HttpGet("{date}", Name = "dataTime")]
        public async Task<string> Get(DateTime date)
        {
            Result<List<DposAddrDaily>> res = new Result<List<DposAddrDaily>>();
            BDpos bll = new BDpos(connStr);            
            try
            {
                string strDate = date.ToString("yyyy-MM-dd");
                var query = bll.GetPayment_money(strDate);
                var list = (await query).ToList();

                res = new Result<List<DposAddrDaily>>(ResultCode.Ok, null, list);
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<DposAddrDaily>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }            
        }

        /// <summary>
        /// Get avg_profit and profit by super node address and date
        /// </summary>
        /// <param name="dposAddr"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("/api/[controller]/")]
        public async Task<string> Get(string dposAddr, DateTime date)
        {
            Result<List<ProfitDaily>> res = new Result<List<ProfitDaily>>();
            if (string.IsNullOrEmpty(dposAddr))
            {
                res = new Result<List<ProfitDaily>>(ResultCode.Forbidden, "dposAddr is null.", null);
                return JsonConvert.SerializeObject(res);
            }

            if (date == DateTime.MinValue)
            {
                res = new Result<List<ProfitDaily>>(ResultCode.Forbidden, "date is null.", null);
                return JsonConvert.SerializeObject(res);
            }

            BDpos bll = new BDpos(connStr);

            try
            {               
                string strDate = date.ToString("yyyy-MM-dd");
                var queryRewardLst = bll.GetDposRewardDetails(strDate, 0);  
                var rewardLst = await queryRewardLst;
                Dictionary<string, List<DposRewardDetails>> dic=new Dictionary<string, List<DposRewardDetails>>();

                foreach (var item in rewardLst)
                {
                    var moo = item;
                    if (!dic.ContainsKey(item.dpos_addr))
                    {
                        var lt = new List<DposRewardDetails>();
                        lt.Add(moo);
                        dic.Add(moo.dpos_addr, lt);
                    }
                    else
                    {
                        dic[moo.dpos_addr].Add(moo);
                    }
                }

                Dictionary<string, ForComput> dicTmp = new Dictionary<string, ForComput>();
                foreach(var d in dic)
                {
                    ArrayList arrHeight = new ArrayList();
                    decimal payment_money_tmp = 0;
                    decimal audit_money_tmp = 0;

                    foreach (var item in d.Value)
                    {
                        if (!arrHeight.Contains(item.block_height))
                        {
                            arrHeight.Add(item.block_height);
                        }
                        payment_money_tmp += item.reward_money;
                        audit_money_tmp += item.vote_amount;
                    }

                    var moo = new ForComput();
                    moo.arrHeight = arrHeight;
                    moo.voteAmount = audit_money_tmp;
                    moo.paymentMoney = payment_money_tmp;
                    dicTmp.Add(d.Key, moo);
                }

                decimal profit = 0;
                decimal avg_profit = 0;
                decimal payment_money = 0;
                decimal audit_money = 0;

                if (dicTmp.ContainsKey(dposAddr.Trim()))
                {
                    var val = dicTmp[dposAddr.Trim()];
                    payment_money = val.paymentMoney;
                    audit_money = (val.voteAmount / val.arrHeight.Count);
                    profit = val.paymentMoney / audit_money;
                }

                foreach (var d in dicTmp)
                {
                    var val = d.Value;
                    var avg = val.paymentMoney / (val.voteAmount / val.arrHeight.Count);
                    avg_profit += avg;
                }

                avg_profit = avg_profit / dicTmp.Count;

                var mo = new ProfitDaily();
                mo.dpos_addr = dposAddr;
                mo.audit_money = decimal.Round(audit_money, 6);
                mo.avg_profit = decimal.Round(avg_profit, 6);
                mo.payment_money = decimal.Round(payment_money, 6);
                mo.profit = decimal.Round(profit, 6);

                List<ProfitDaily> lst = new List<ProfitDaily>();
                lst.Add(mo);
                res = new Result<List<ProfitDaily>>(ResultCode.Ok, null, lst);
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<ProfitDaily>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }
    }
}