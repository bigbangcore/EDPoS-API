using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    /// <summary>
    /// About Voters information
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VotersController : Controller
    {
        private SiteConfig Config;
        static string connStr = string.Empty;
        public VotersController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        /// <summary>
        /// Get the Voting of voters by dposaddr,if has the voterAddr parameter,return the voter's information
        /// </summary>
        /// <param name="dposAddr"></param>
        /// <param name="voterAddr"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Get(string dposAddr, string voterAddr = "")
        {
            Result<List<MVoters>> res = new Result<List<MVoters>>();
            using (var conn =new MySqlConnection(connStr))
            {
                string strSql = GetSql(dposAddr, voterAddr);

                try
                {
                    var query = conn.QueryAsync<MVoters>(strSql);

                    var list = (await query).ToList();
                    if (list.Count > 0)
                    {
                        res = new Result<List<MVoters>>(ResultCode.Ok, null, list);
                    }
                    else
                    {
                        res = new Result<List<MVoters>>(ResultCode.NoRecord, null, null);
                    }
                    return JsonConvert.SerializeObject(res);
                }
                catch (Exception ex)
                {
                    res = new Result<List<MVoters>>(ResultCode.Fail, ex.Message, null);
                    return JsonConvert.SerializeObject(res);
                }

            }
        }

        private string GetSql(string dposAddr,string voterAddr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"SELECT 
                        max(height) AS height,
                        max(votedate) AS votedate,
                        voter AS 'from',
                        sum(amount) AS amount,
                        max(type) AS type ");
            sb.Append(@"FROM (");
            sb.Append(@"SELECT 
                        b.height,
                        t.txid,
                        t.form,
                        from_unixtime(b.time, '%Y-%m-%d %H:%i:%s') AS votedate,
                        t.`to`,
                        t.amount,
                        t.type, (");
            sb.Append(@"CASE ");
            sb.Append(@"WHEN t.client_in IS NULL AND(t.n = 0 OR t.type = 'certification') THEN
                        t.form ");
            sb.Append(@"WHEN t.client_in IS NULL  AND t.n = 1 AND t.type = 'token' THEN            
                        (SELECT client_in FROM Tx WHERE `to` = t.`to` AND n = 0 LIMIT 1) ");
            sb.Append(@"ELSE ");
            sb.Append(@"t.client_in ");
            sb.Append(@"END ");
            sb.Append(@") AS voter,t.n ");
            sb.Append(@"FROM ");
            sb.Append(@"Tx t JOIN Block b ON b.`hash` = t.block_hash ");
            sb.Append(@"WHERE ");

            sb.Append(@"(");
            sb.Append(@"t.`to` IN ( SELECT DISTINCT `to` FROM Tx WHERE LEFT ( `to`, 4 ) = '20w0' AND dpos_in = '");
            sb.Append(dposAddr);
            sb.Append("'");
            sb.Append(@") OR t.`to` = '" + dposAddr + "' ");
            sb.Append(@") ");

            sb.Append(@"AND ( t.type = 'token' OR t.type = 'certification' ) ");
            sb.Append(@"AND t.spend_txid IS NULL ");
            sb.Append(@") c ");

            sb.Append(@"GROUP BY voter ");
            sb.Append(@"ORDER BY amount DESC");
            
            if (!string.IsNullOrEmpty(voterAddr))
            {
                string strSql = string.Empty;
                strSql = "select * FROM (";
                strSql += sb.ToString();
                strSql += ") as tt ";
                strSql += "WHERE tt.from = '" + voterAddr + "'";
                return strSql;
            }
            return sb.ToString();
        }

        /*
        // GET: api/Voters
        [HttpGet]
        public async Task<string> Get(string dposaddr, string voteraddr = "", string orderby = "", uint pageIndex = 1, uint pageSize = 10)
        {
            Result<List<MVoters>> res = new Result<List<MVoters>>();
            using (var conn = new MySqlConnection(connStr))
            {
                StringBuilder strAppend = new StringBuilder();
                uint startRow = pageSize * (pageIndex - 1);

                StringBuilder sb = new StringBuilder();
                sb.Append("select sql_calc_found_rows max(b.height) as height,");
                sb.Append("FROM_UNIXTIME(max(b.time),'%Y-%m-%d %H:%i:%s') as votedate,");
                sb.Append("t.form as `from`,sum(t.amount) as amount,max(t.type) as type ");
                sb.Append("from Pool p join Tx t ");
                sb.Append("on (p.address = t.`to` or p.address = t.dpos_in) ");
                sb.Append("join Block b on b.`hash`= t.block_hash ");
                sb.Append("where 1=1 ");

                if (!string.IsNullOrEmpty(dposaddr))
                {
                    sb.Append("and p.address = \"" + dposaddr + "\" ");
                }
                
                sb.Append("and (t.type='token' or t.type='certification') and t.spend_txid is null ");

                if (!string.IsNullOrEmpty(voteraddr))
                {
                    sb.Append("and t.form=\""+ voteraddr  + "\"");
                }

                sb.Append(" group by t.form");

                if (!string.IsNullOrEmpty(orderby))
                {
                    sb.Append(" order by amount " + orderby);
                }
                else
                {
                    sb.Append(" order by height desc");
                }

                sb.Append(" limit "+ startRow + ","+ pageSize);

                try
                {
                    var query = conn.QueryAsync<MVoters>(sb.ToString());

                    var list = (await query).ToList();
                    if (list.Count > 0)
                    {
                        res = new Result<List<MVoters>>(ResultCode.Ok, null, list);
                    }
                    else
                    {
                        res = new Result<List<MVoters>>(ResultCode.NoRecord, null, null);
                    }
                    return JsonConvert.SerializeObject(res);
                }
                catch (Exception ex)
                {
                    res = new Result<List<MVoters>>(ResultCode.Fail, ex.Message, null);
                    return JsonConvert.SerializeObject(res);
                }
            }
        }*/
    }
}