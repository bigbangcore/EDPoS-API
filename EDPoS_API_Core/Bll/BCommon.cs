using Dapper;
using EDPoS_API_Core.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Bll
{
    public class BCommon
    {
        static string connStr = string.Empty;
        public BCommon(string str)
        {
            connStr = str;
        }

        public async Task<long> GetMaxHeight(string date = "", string dposAddr = "")
        {
            using (var conn = new MySqlConnection(connStr))
            {
                try
                {
                    StringBuilder sb = new StringBuilder("SELECT max(block_height) AS height FROM DposRewardDetails WHERE 1=1 ");
                    if (!string.IsNullOrEmpty(date))
                    {
                        sb.Append("AND reward_date = '" + date + "' ");
                    }
                    if (!string.IsNullOrEmpty(dposAddr))
                    {
                        sb.Append("AND dpos_addr = '" + dposAddr + "' ");
                    }
                    var query = conn.QueryAsync<long>(sb.ToString());
                    return (await query).ToList<long>().FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<List<MVotingTokens>> GetVotingTokens(long height)
        {
            using (var conn = new MySqlConnection(connStr))
            {
                string strSql = "SELECT SUM(vote_amount) as audit_money,dpos_addr FROM DposRewardDetails WHERE block_height = ?height GROUP BY dpos_addr;";
                var query = conn.QueryAsync<MVotingTokens>(strSql,new { height = height });
                return (await query).ToList();
            }
        }

        /// <summary>
        /// Get the total rewards of the day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<List<DposAddrDaily>> GetPayment_money(string date)
        {
            using (var conn = new MySqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT dpos_addr, SUM(payment_money) as payment_money,'" + date + "' as payment_date ");
                sb.Append(" from DposDailyReward where payment_date = '");
                sb.Append(date);
                sb.Append("' GROUP BY dpos_addr");

                var query = conn.QueryAsync<DposAddrDaily>(sb.ToString());
                return (await query).ToList();
            }
        }

        public async Task<List<MBlockPa>> GetBlock(string reward_address, string date)
        {
             using(var conn =new MySqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT reward_address,reward_money from Block ");
                sb.Append("WHERE type = 'primary-dpos' AND is_useful = 1");

                if (!string.IsNullOrEmpty(reward_address))
                {
                    sb.Append(" AND reward_address = '"+ reward_address + "'");
                }

                if (!string.IsNullOrEmpty(date))
                {
                    sb.Append(" AND from_unixtime(time, '%Y-%m-%d') = '" + date + "'");
                }

                var query = conn.QueryAsync<MBlockPa>(sb.ToString());
                return (await query).ToList();
            }

        }

        /// <summary>
        /// Voting and reward details at an address on a given day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<List<DposRewardDetails>> GetDposRewardDetails(string dateTime = "", int height = 0, string dpos_addr = "", string client_addr = "")
        {
            using (var conn = new MySqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder("SELECT dpos_addr,client_addr,vote_amount,reward_money,reward_date,block_height FROM DposRewardDetails where 1=1 ");
                Task<IEnumerable<DposRewardDetails>> query;
                if (!string.IsNullOrEmpty(dateTime))
                {
                    sb.Append("AND reward_date ='" + dateTime + "'");
                }

                if (height > 0)
                {
                    sb.Append("AND block_height = " + height + " ");
                }

                if (!string.IsNullOrEmpty(dpos_addr))
                {
                    sb.Append("AND dpos_addr ='"+ dpos_addr + "'");
                }

                if (!string.IsNullOrEmpty(client_addr))
                {
                    sb.Append("AND client_addr = '" + client_addr + "'");
                }

                query = conn.QueryAsync<DposRewardDetails>(sb.ToString());
                return (await query).ToList();
            }
        }
    }
}
