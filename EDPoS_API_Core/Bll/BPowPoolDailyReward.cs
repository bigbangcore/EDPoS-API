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
    public class BPowPoolDailyReward
    {
        static string connStr = string.Empty;
        public BPowPoolDailyReward(string str)
        {
            connStr = str;
        }

        /// <summary>
        /// Get One
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<MPowPoolDailyReward>> GetOne(long id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT `id`,`addrFrom`,`addrTo`,`reward`,`settlementDate` FROM PowPoolDailyReward WHERE id = @id");
            using (var conn = new MySqlConnection(connStr))
            {
                return (await conn.QueryAsync<MPowPoolDailyReward>(sb.ToString(), new { id = id })).ToList();
            }
        }

        /// <summary>
        /// Get info by addrFrom, addrTo or settlement date
        /// </summary>
        /// <param name="addrFrom"></param>
        /// <param name="addrTo"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<List<MPowPoolDailyReward>> GetBySomething(string addrFrom = "", string addrTo = "", string date = "")
        {
            if (SqlAttack.IsDangerous(ref addrFrom) || SqlAttack.IsDangerous(ref addrTo) || SqlAttack.IsDangerous(ref date))
            {
                return new List<MPowPoolDailyReward>();
            }
            StringBuilder sb = new StringBuilder();
            
            sb.Append("SELECT `id`,`addrFrom`,`addrTo`,`reward`,`settlementDate` FROM PowPoolDailyReward WHERE 1=1 ");
            if (!string.IsNullOrEmpty(date))
            {
                DateTime dt = new DateTime();
                if (DateTime.TryParse(date, out dt))
                {
                    date = dt.ToString("yyyy-MM-dd");
                    sb.Append("AND settlementDate = '" + date + "' ");
                }                
            }

            if (!string.IsNullOrEmpty(addrFrom))
            {
                sb.Append("AND addrFrom = '" + addrFrom + "' ");
            }
            if (!string.IsNullOrEmpty(addrTo))
            {
                sb.Append(" AND addrTo = '" + addrTo + "'");
            }

            using (var conn = new MySqlConnection(connStr))
            {
                var re = await conn.QueryAsync<MPowPoolDailyReward>(sb.ToString());
                return re.ToList();
            }
        }

        /// <summary>
        /// insert one
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        public async Task<bool> InsertOne(MPowPoolDailyReward mo)
        {
            string addrFrom = mo.addrFrom;
            string addrTo = mo.addrTo;
            bool res = false;
            if (SqlAttack.IsDangerous(ref addrFrom) || SqlAttack.IsDangerous(ref addrTo))
            {
                return res;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO `PowPoolDailyReward` (`id`,`addrFrom`,`addrTo`,`reward`,`settlementDate`) VALUES (");
            sb.Append("@id, @addrFrom, @addrTo, @reward, @settlementDate");
            sb.Append(")");

            using (var conn = new MySqlConnection(connStr))
            {
                //If it has one,skip
                var g = await GetBySomething(mo.addrFrom, mo.addrTo, mo.settlementDate);
                if(g.Count > 0)
                {
                    return false;
                }
                var re = await conn.ExecuteAsync(sb.ToString(), new { 
                    id = 0, 
                    addrFrom = mo.addrFrom.Trim(), 
                    addrTo = mo.addrTo.Trim(), 
                    reward = mo.reward, 
                    settlementDate = Convert.ToDateTime(mo.settlementDate).ToString("yyyy-MM-dd")
                });
                // var re = await conn.ExecuteAsync(sb.ToString(), mo);
                if (re > 0)
                {
                    res = true;
                }
                return res;
            }
        }

        /// <summary>
        /// Multi insert
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public async Task<bool> InsertLst(List<MPowPoolDailyReward> lst)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO `PowPoolDailyReward` (`id`,`addrFrom`,`addrTo`,`reward`,`settlementDate`) VALUES (");
            sb.Append("@id, @addrFrom, @addrTo, @reward, @settlementDate");
            sb.Append(")");

            using (var conn = new MySqlConnection(connStr))
            {
                var re = await conn.ExecuteAsync(sb.ToString(),lst);
                if (re > 0)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
