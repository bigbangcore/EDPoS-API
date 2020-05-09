using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
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
        /// <param name="dpos_addr">dpos address</param>
        /// <param name="date">date,formate:2020-01-01</param>
        /// <returns>list</returns>
        [HttpGet]
        public async Task<string> Get(string dpos_addr, DateTime date)
        {
            using (var conn = new MySqlConnection(connStr))
            {
                Result<List<DposDailyReward>> res = new Result<List<DposDailyReward>>();
                IEnumerable<DposDailyReward> query = null;
                if (string.IsNullOrEmpty(dpos_addr))
                {
                    query = await conn.QueryAsync<DposDailyReward>("SELECT * FROM `DposDailyReward` where payment_date = @payment_date",
                    new { payment_date = date.Date });
                }
                else
                {
                    query = await conn.QueryAsync<DposDailyReward>("SELECT * FROM `DposDailyReward` where dpos_addr = @dpos_addr and payment_date = @payment_date",
                    new { dpos_addr = dpos_addr, payment_date = date.Date });
                }
                
                var lst = query.ToList();
                if(lst.Count > 0)
                {
                    res = new Result<List<DposDailyReward>>(ResultCode.Ok, null, lst);
                }
                else
                {
                    res = new Result<List<DposDailyReward>>(ResultCode.NoRecord, null, null);
                }
                return JsonConvert.SerializeObject(res);
            }
        }

        /// <summary>
        /// The client pays the token to the voter, and then submits the voting information
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">JsonPayment model</param>
        /// <returns>Missing dpos node/OK/key error</returns>
        [HttpPut("{id}")]
        public string Put(int id, [FromBody]string value)
        {
            JsonPayment jp = JsonConvert.DeserializeObject<JsonPayment>(value);
            Result<string> res = new Result<string>();
            using (var conn = new MySqlConnection(connStr))
            {
                var list = conn.Query<Pool>("SELECT Pool.`key` FROM DposPayment INNER JOIN Pool on Pool.address = DposPayment.dpos_addr where DposPayment.id = @id;", new { id = id }).ToList();
                if (list.Count == 0)
                {
                    res = new Result<string>(ResultCode.NoRecord, "Missing dpos node", null);
                    return JsonConvert.SerializeObject(res);
                }
                else
                {
                    if (Encrypt.EncryptByMD5(jp.txid + list[0].key) == jp.md5)
                    {
                        conn.Execute("update DposPayment set txid=@txid where id=@id ", new { txid = jp.txid, id = id });
                        res = new Result<string>(ResultCode.Ok, null, null);
                        return JsonConvert.SerializeObject(res);
                    }
                    else
                    {
                        res = new Result<string>(ResultCode.Fail, "Key error", null);
                        return JsonConvert.SerializeObject(res);
                    }
                }
            }
        }
    }
}
