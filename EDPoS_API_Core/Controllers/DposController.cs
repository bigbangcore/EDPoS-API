using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
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
            using (var conn = new MySqlConnection(connStr))
            {
                var query = await conn.QueryAsync<Pool>("SELECT * FROM `Pool` where type = 'dpos'", null);
                var list = query.ToList();
                foreach (var obj in list)
                {
                    obj.key = "******";
                }
                res = new Result<List<Pool>>(list);
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
            Bll.BCommon bll = new Bll.BCommon(connStr);
            string strDate = date.ToString("yyyy-MM-dd");
            var query = bll.GetPayment_money(strDate);
            var list = (await query).ToList();

            res = new Result<List<DposAddrDaily>>(ResultCode.Ok, null, list);
            return JsonConvert.SerializeObject(res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dposAddr"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("/api/[controller]/")]
        public async Task<string> Get(string dposAddr, DateTime date)
        {
            try
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
                Bll.BCommon bll = new Bll.BCommon(connStr);
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
                throw ex;
            }
        }

        /// <summary>
        /// Change the edpos node name
        /// </summary>
        /// <param name="id">node id</param>
        /// <param name="value">JsonPool,what contains name information</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public string Put(int id, [FromBody]string value)
        {
            JsonPool jp = JsonConvert.DeserializeObject<JsonPool>(value);
            using (var conn = new MySqlConnection(connStr))
            {
                var list = conn.Query<Pool>("SELECT * FROM Pool where id = @id;", new { id = id }).ToList();
                if (list.Count == 0)
                {
                    return "Missing dpos node";
                }
                else
                {
                    if (Encrypt.EncryptByMD5(id + list[0].key) == jp.md5)
                    {
                        conn.Execute("update Pool set name = @name where id = @id ", new { name = jp.name, id = id });
                        return "OK";
                    }
                    else
                    {
                        return "key error";
                    }
                }
            }
        }

        private string GetSqlNew(string dposAddr, string voterAddr = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT  
                        sql_calc_found_rows max(height) AS height,
                        max(votedate) AS votedate,
                        voter AS 'from',
                        sum(amount) AS amount, 
                        max(type) AS type FROM (");
            sb.Append(@"SELECT 
                        b.height,
                        from_unixtime(b.time, '%Y-%m-%d %H:%i:%s') AS votedate,
                        t.form,
                        t.`to`, 
                        t.amount, 
                        t.type,(");
            sb.Append(@"CASE 
                        WHEN t.client_in is null AND t.type='stake' 
                        THEN t.`to`
                        WHEN t.client_in is null AND t.n=0 
                        THEN t.form
                        WHEN t.client_in is null AND t.`to`= '" + dposAddr + @"' AND t.n=1 
                        THEN t.`to`
                        WHEN t.client_in is null AND t.n=1 AND t.type='token' 
                        THEN
                        (SELECT client_in FROM Tx WHERE `to`= t.`to` AND n = 0 LIMIT 1) 
                        ELSE 
                        t.client_in 
                        END) AS voter, t.n 
                        FROM Tx t 
                        JOIN Block b ON b.`hash`= t.block_hash
                        WHERE 
                        (t.`to` IN (SELECT DISTINCT `to` FROM Tx WHERE LEFT(`to`, 4) = '20w0' AND dpos_in = '");
            sb.Append(dposAddr);
            sb.Append("') OR t.`to`= '");
            sb.Append(dposAddr);
            sb.Append("')");
            sb.Append(@"and t.type <> 'certification' and t.spend_txid is null) c where 1=1 ");

            if (!string.IsNullOrEmpty(voterAddr))
            {
                sb.Append(" AND voter = '" + voterAddr + "' ");
            }

            sb.Append("group by voter");

            return sb.ToString();
        }
    }
}