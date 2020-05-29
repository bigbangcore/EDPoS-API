using Dapper;
using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Bll
{
    public class BVoter
    {
        static string connStr = string.Empty;
        public BVoter(string str)
        {
            connStr = str;
        }

        /// <summary>
        /// Get the Voting of voters by dposaddr,if has the voterAddr parameter,return the voter's information
        /// </summary>
        /// <param name="dposAddr">super node address</param>
        /// <param name="voterAddr">voter address</param>
        /// <returns></returns>
        public async Task<List<MVoters>> GetVoting(string dposAddr, string voterAddr = "")
        {
            if (SqlAttack.IsDangerous(ref dposAddr) || SqlAttack.IsDangerous(ref voterAddr))
            {
                return new List<MVoters>();
            }
            using (var conn = new MySqlConnection(connStr))
            {
                string strSql = GetSqlNew(dposAddr, voterAddr);
                return (await conn.QueryAsync<MVoters>(strSql)).ToList();
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
