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
    public class BReward
    {
        static string connStr = string.Empty;
        public BReward(string str)
        {
            connStr = str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dpos_addr"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<List<DposAddrDaily>> GetTotalReward(string dpos_addr, string date)
        {
            if (SqlAttack.IsDangerous(ref dpos_addr) || SqlAttack.IsDangerous(ref date))
            {
                return new List<DposAddrDaily>();
            }

            using (var conn = new MySqlConnection(connStr))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT dpos_addr,sum(payment_money) as payment_money,payment_date FROM `DposDailyReward` where 1=1 ");

                if (!string.IsNullOrEmpty(dpos_addr))
                {
                    sb.Append(" AND dpos_addr = '" + dpos_addr.Trim() + "' ");
                }

                if (!string.IsNullOrEmpty(date))
                {
                    DateTime dateTmp = new DateTime();
                    if (DateTime.TryParse(date, out dateTmp))
                    {
                        sb.Append(" AND payment_date = '" + dateTmp.ToString("yyyy-MM-dd") + "' ");
                    }
                }

                if ((string.IsNullOrEmpty(date) && string.IsNullOrEmpty(dpos_addr)) || !string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(dpos_addr))
                {
                    sb.Append("GROUP BY dpos_addr,payment_date");
                }
                else
                {
                    if (!string.IsNullOrEmpty(date))
                    {
                        sb.Append("GROUP BY dpos_addr");
                    }
                    else
                    {
                        sb.Append("GROUP BY payment_date");
                    }
                }                
                return (await conn.QueryAsync<DposAddrDaily>(sb.ToString())).ToList();                
            }
        }

        /// <summary>
        /// Get Dpos Daily Reward
        /// </summary>
        /// <param name="dpos_addr"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<List<DposDailyReward>> GetDposDailyReward(string dpos_addr, DateTime date)
        {
            if (SqlAttack.IsDangerous(ref dpos_addr))
            {
                return new List<DposDailyReward>();
            }
            using (var conn = new MySqlConnection(connStr))
            {
                Task<IEnumerable<DposDailyReward>> query;
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT id,dpos_addr,client_addr,payment_date,payment_money,txid FROM `DposDailyReward` where 1=1 ");
                if (!string.IsNullOrEmpty(dpos_addr))
                {
                    sb.Append(" AND dpos_addr = @dpos_addr ");
                }

                if (date != DateTime.MinValue)
                {
                    sb.Append(" AND payment_date = @payment_date ");
                }

                if (!string.IsNullOrEmpty(dpos_addr) && date != DateTime.MinValue)
                {
                    query = conn.QueryAsync<DposDailyReward>(sb.ToString(), new { dpos_addr = dpos_addr, payment_date = date.Date });
                }
                else
                {
                    if (string.IsNullOrEmpty(dpos_addr) && date == DateTime.MinValue)
                    {
                        query = conn.QueryAsync<DposDailyReward>(sb.ToString());
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dpos_addr))
                        {
                            query = conn.QueryAsync<DposDailyReward>(sb.ToString(), new { dpos_addr = dpos_addr });
                        }
                        else
                        {
                            query = conn.QueryAsync<DposDailyReward>(sb.ToString(), new { payment_date = date.Date });
                        }
                    }                    
                }
                return (await query).ToList();
            }
        }
    }
}
