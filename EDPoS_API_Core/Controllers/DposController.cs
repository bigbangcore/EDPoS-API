using System;
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
        [HttpGet]
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
            using (var conn = new MySqlConnection(connStr))
            {
                Result<List<DposAddrDaily>> res = new Result<List<DposAddrDaily>>();
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT dpos_addr, SUM(payment_money) as payment_money,'"+ date.ToString("yyyy-MM-dd") + "' as payment_date ");
                sb.Append(" from DposDailyReward where payment_date = '");
                sb.Append(date.ToString("yyyy-MM-dd"));
                sb.Append("' GROUP BY dpos_addr");

                var query = conn.QueryAsync<DposAddrDaily>(sb.ToString());
                var list = (await query).ToList();

                res = new Result<List<DposAddrDaily>>(ResultCode.Ok, null, list);
                return JsonConvert.SerializeObject(res);
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
    }
}