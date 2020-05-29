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
    public class BCommon
    {
        static string connStr = string.Empty;
        public BCommon(string str)
        {
            connStr = str;
        }

        /// <summary>
        /// Get EDPoS Blocks Detail what is useful
        /// </summary>
        /// <param name="reward_address"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<List<MBlockDetail>> GetBlockDetail(string reward_address, string date)
        {
            if (SqlAttack.IsDangerous(ref reward_address) || SqlAttack.IsDangerous(ref date))
            {
                return new List<MBlockDetail>();
            }
            using (var conn =new MySqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT id,hash,fork_hash,prev_hash,time,height,type,reward_address,reward_money,is_useful,bits,reward_state from Block ");
                sb.Append("WHERE type = 'primary-dpos' AND is_useful = 1");

                if (!string.IsNullOrEmpty(reward_address))
                {
                    sb.Append(" AND reward_address = '"+ reward_address + "'");
                }

                if (!string.IsNullOrEmpty(date))
                {
                    sb.Append(" AND from_unixtime(time, '%Y-%m-%d') = '" + date + "'");
                }

                var query = conn.QueryAsync<MBlockDetail>(sb.ToString());
                return (await query).ToList();
            }
        }
    }
}
