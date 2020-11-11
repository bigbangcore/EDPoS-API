using Dapper;
using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using Microsoft.CodeAnalysis.CSharp;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Bll
{
    public class BDpos
    {
        static string connStr = string.Empty;
        public BDpos(string str)
        {
            connStr = str;
        }

        /// <summary>
        /// Get Pool List
        /// </summary>
        /// <returns></returns>
        public async Task<List<Pool>> GetPoolList()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                var query = await conn.QueryAsync<Pool>("SELECT * FROM `Pool` where type = 'dpos'");
                var lst = query.ToList();
                foreach (var obj in lst)
                {
                    obj.key = "******";
                }
                return lst;
            }
        }

        /// <summary>
        /// Voting and reward details at an address on a given day
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="height"></param>
        /// <param name="dpos_addr"></param>
        /// <param name="client_addr"></param>
        /// <returns></returns>
        public async Task<List<DposRewardDetails>> GetDposRewardDetails(string dateTime = "", int height = 0, string dpos_addr = "", string client_addr = "")
        {
            var lst = new List<DposRewardDetails>();
            if (SqlAttack.IsDangerous(ref dateTime) || SqlAttack.IsDangerous(ref dpos_addr) || SqlAttack.IsDangerous(ref client_addr))
            {
                return lst;
            }
            using (var conn = new MySqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT dpos_addr,client_addr,vote_amount,reward_money,reward_date,block_height FROM DposRewardDetails where 1=1 ");
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
                    sb.Append("AND dpos_addr ='" + dpos_addr + "'");
                }

                if (!string.IsNullOrEmpty(client_addr))
                {
                    sb.Append("AND client_addr = '" + client_addr + "'");
                }

                var query = conn.QueryAsync<DposRewardDetails>(sb.ToString());

                return (await query).ToList();
            }
        }

        /// <summary>
        /// Get the max height of the table DposRewardDetails
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dposAddr"></param>
        /// <returns></returns>
        public async Task<long> GetMaxHeight(string date = "", string dposAddr = "")
        {
            if (SqlAttack.IsDangerous(ref date) || SqlAttack.IsDangerous(ref dposAddr))
            {
                return 0;
            }
            using (var conn = new MySqlConnection(connStr))
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
        }

        /// <summary>
        /// Get the situation of voting
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public async Task<List<MVotingTokens>> GetVotingTokens(long height = 0)
        {
            using (var conn = new MySqlConnection(connStr))
            {
                if (height == 0)
                {
                    height = await GetMaxHeight();
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT SUM(vote_amount) as audit_money,dpos_addr FROM DposRewardDetails WHERE ");
                sb.Append("block_height = ?height ");
                sb.Append("GROUP BY dpos_addr");
                var query = conn.QueryAsync<MVotingTokens>(sb.ToString(), new { height = height });
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
            if (SqlAttack.IsDangerous(ref date))
            {
                return new List<DposAddrDaily>();
            }
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


        public async Task<List<int>> GetResidue(string date)
        {
            using (var conn = new MySqlConnection(connStr))
            {
                string sql = "select count(id) as count from DposRewardDetails where reward_date = '"+date+"' and reward_state = 0";
                var query = conn.QueryAsync<int>(sql);
                return (await query).ToList();

            }
        }

        public async Task<List<int>> Delete(string date)
        {
            using (var conn = new MySqlConnection(connStr))
            {
                string sql = "select count(id) as count from DposRewardDetails where reward_date = '" + date + "' and reward_state = 0";
                var query = conn.QueryAsync<int>(sql);
                return (await query).ToList();

            }
        }
    }
}
