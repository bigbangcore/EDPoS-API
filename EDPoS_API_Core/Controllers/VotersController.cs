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
                string strSql = GetSqlNew(dposAddr, voterAddr);

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

        private string GetSqlNew(string dposAddr, string voterAddr)
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
                        b.time AS votedate,
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

        private async Task<IEnumerable<string>> GetTo(string dposAddr)
        {
            // string sql = "SELECT DISTINCT `to` FROM Tx WHERE LEFT ( `to`, 4 ) = '20w0' AND dpos_in = '"+ dposAddr + "'";
            string sql = "SELECT `to` FROM Tx WHERE LEFT ( `to`, 4 ) = '20w0' AND dpos_in = '" + dposAddr + "'";
            using (var conn = new MySqlConnection(connStr))
            {
                return await conn.QueryAsync<string>(sql);
            }
        }
    }
}