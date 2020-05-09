using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    /// <summary>
    /// EDPoS details
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DposDetailsController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;

        public DposDetailsController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        // GET: api/DposDetails
        [HttpGet]
        public async Task<string> Get(string dataTime = "", int height = 0, string dpos_addr = "", string client_addr = "")
        {
            Result<List<DposRewardDetails>> res = new Result<List<DposRewardDetails>>();
            using (var conn = new MySqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder("SELECT dpos_addr,client_addr,vote_amount,reward_money,reward_date,block_height FROM DposRewardDetails where 1=1 ");
                Task<IEnumerable<DposRewardDetails>> query;
                if (!string.IsNullOrEmpty(dataTime))
                {
                    sb.Append("AND reward_date =\"");
                    sb.Append(dataTime);
                    sb.Append("\" ");
                }

                if (height > 0)
                {
                    sb.Append("AND block_height = ");
                    sb.Append(height);
                    sb.Append(" ");
                }

                if (!string.IsNullOrEmpty(dpos_addr))
                {
                    sb.Append("AND dpos_addr =\"");
                    sb.Append(dpos_addr);
                    sb.Append("\" ");
                }

                if (!string.IsNullOrEmpty(client_addr))
                {
                    sb.Append("AND client_addr =\"");
                    sb.Append(client_addr);
                    sb.Append("\" ");
                }
                query = conn.QueryAsync<DposRewardDetails>(sb.ToString());

                var list = (await query).ToList();
                res = new Result<List<DposRewardDetails>>(ResultCode.Ok, null, list);
                return JsonConvert.SerializeObject(res);
            }
        }
    }
}
