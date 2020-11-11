using Dapper;
using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Bll
{
    public class BUnlockBlock
    {
        static string connStr = string.Empty;
        public BUnlockBlock(string str)
        {
            connStr = str;
        }

        #region Get unlocked blocks by parameters
        /// <summary>
        /// Get unlocked blocks by parameters
        /// </summary>
        /// <param name="timeSpanMin">The start timeSpan</param>
        /// <param name="timeSpanMax">The end timeSpan</param>
        /// <param name="addrFrom"></param>
        /// <param name="addrTo"></param>
        /// <returns></returns>
        public async Task<List<MUnlockBlockLst>> GetLst(string timeSpanMin, string timeSpanMax, string addrFrom = "", string addrTo = "")
        {
            if (SqlAttack.IsDangerous(ref addrFrom) || SqlAttack.IsDangerous(ref addrTo) || SqlAttack.IsDangerous(ref timeSpanMin) || SqlAttack.IsDangerous(ref timeSpanMax))
            {
                return new List<MUnlockBlockLst>();
            }
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT `id`,`addrFrom`,`addrTo`,`balance`,`timeSpan`,`date`,`height` FROM UnlockedBlock WHERE 1=1 ");
            if (!string.IsNullOrEmpty(timeSpanMin))
            {
                sb.Append("AND timeSpan >= " + long.Parse(timeSpanMin) + " ");                
            }

            if (!string.IsNullOrEmpty(timeSpanMax))
            {
                sb.Append("AND timeSpan < " + long.Parse(timeSpanMax) + " ");
            }

            if (!string.IsNullOrEmpty(addrFrom))
            {
                sb.Append("AND addrFrom = '" + addrFrom + "' ");
            }
            if (!string.IsNullOrEmpty(addrTo))
            {
                sb.Append(" AND addrTo = '" + addrTo + "'");
            }

            Console.WriteLine(sb.ToString());

            using (var conn = new MySqlConnection(connStr))
            {
                var lst = await conn.QueryAsync<MUnlockBlock>(sb.ToString());
                var tmpLst = new List<MUnlockBlockLst>();
                foreach (var item in lst)
                {
                    if (tmpLst.Where(p=>p.addrFrom.Equals(item.addrFrom) && p.date.ToString("yyyy-MM-dd").Equals(item.date.ToString("yyyy-MM-dd"))).Count() > 0)
                    {
                        var tmpMoo = new MUnlockBlockReward();
                        tmpMoo.addrTo = item.addrTo;
                        tmpMoo.balance = item.balance;
                        tmpMoo.timeSpan = item.timeSpan;
                        tmpMoo.height = item.height;

                        foreach (var v in tmpLst)
                        {
                            if (v.addrFrom.Equals(item.addrFrom) && v.date.ToString("yyyy-MM-dd").Equals(item.date.ToString("yyyy-MM-dd")))
                            {
                                v.balanceLst.Add(tmpMoo);
                                break;
                            }
                        }                        
                    }
                    else
                    {
                        var tmpMo = new MUnlockBlockLst();
                        tmpMo.addrFrom = item.addrFrom;
                        tmpMo.date = item.date;
                        tmpMo.balanceLst = new List<MUnlockBlockReward>();

                        var tmpMoo = new MUnlockBlockReward();
                        tmpMoo.addrTo = item.addrTo;
                        tmpMoo.balance = item.balance;
                        tmpMoo.timeSpan = item.timeSpan;
                        tmpMoo.height = item.height;

                        tmpMo.balanceLst.Add(tmpMoo);
                        tmpLst.Add(tmpMo);
                    }
                }

                return tmpLst;
            }
        }
        #endregion

        #region Judge if the data is exist
        public async Task<List<MUnlockBlock>> IsExist(int height, string addrFrom="", string addrTo="")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT `id`,`addrFrom`,`addrTo`,`balance`,`timeSpan`,`date`,`height` FROM UnlockedBlock WHERE 1=1 ");
            if (height != 0)
            {
                sb.Append("AND height = " + height + " ");
            }
            if (!string.IsNullOrEmpty(addrFrom))
            {
                sb.Append("AND addrFrom = '" + addrFrom + "' ");
            }
            if (!string.IsNullOrEmpty(addrTo))
            {
                sb.Append("AND addrTo = '" + addrTo + "' ");
            }
            using (var conn = new MySqlConnection(connStr))
            {
                var re = await conn.QueryAsync<MUnlockBlock>(sb.ToString());
                return re.ToList();
            }
        }
        #endregion

        #region Get unlockBlocks By parameters,there are more params
        /// <summary>
        /// Get unlockBlocks By parameters,there are more params
        /// </summary>
        /// <param name="addrFrom"></param>
        /// <param name="addrTo"></param>
        /// <param name="date">If it has this param, then the param timeSpan is invalid</param>
        /// <param name="timeSpan"></param>
        /// <param name="height"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public async Task<List<MUnlockBlock>> GetLst(string addrFrom = "", string addrTo = "", string date = "", long timeSpan = 0, int height = 0,int zone = 0)
        {
            if (SqlAttack.IsDangerous(ref addrFrom) || SqlAttack.IsDangerous(ref addrTo))
            {
                return new List<MUnlockBlock>();
            }
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT `id`,`addrFrom`,`addrTo`,`balance`,`timeSpan`,`date`,`height` FROM UnlockedBlock WHERE 1=1 ");
            if (height != 0)
            {
                sb.Append("AND height = " + height + " ");
            }

            if (!string.IsNullOrEmpty(date))
            {
                DateTime dt = new DateTime();
                if (DateTime.TryParse(date, out dt))
                {
                    var dateEnd = dt.AddDays(1);
                    var dStart = Convert.ToInt64(CommonHelper.GetTimeStamp(dt, zone)) / 1000;
                    var dEnd = Convert.ToInt64(CommonHelper.GetTimeStamp(dateEnd, zone)) / 1000;
                    sb.Append("AND timeSpan >= " + dStart + " AND  timeSpan <= " + dEnd + " ");
                }
                else
                {
                    if (timeSpan != 0)
                    {
                        sb.Append("AND timeSpan = " + timeSpan + " ");
                    }
                }
            }
            else
            {
                if (timeSpan != 0)
                {
                    sb.Append("AND timeSpan = " + timeSpan + " ");
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
                var re = await conn.QueryAsync<MUnlockBlock>(sb.ToString());
                return re.ToList();
            }
        }
        #endregion

        public async Task<List<MPowPoolDailyReward>> GetLstSum(string timeSpanMin, string timeSpanMax, string addrFrom = "", string addrTo = "")
        {            
            List<MPowPoolDailyReward> tmpLst = new List<MPowPoolDailyReward>();
            var lst = await GetLst(timeSpanMin, timeSpanMax, addrFrom, addrTo);

            foreach(var v in lst)
            {                
                var reLst =  v.balanceLst.GroupBy(p => new { p.addrTo })
                    .Select(g => new
                    {
                        addrTo = g.Key.addrTo,
                        reward = g.Sum(p => p.balance)
                    }).ToList();
                foreach(var j in reLst){
                    var mo = new MPowPoolDailyReward();
                    mo.addrFrom = v.addrFrom;
                    mo.settlementDate = v.date.ToString("yyyy-MM-dd");
                    mo.id = 0;

                    mo.addrTo = j.addrTo;
                    mo.reward = j.reward;
                    tmpLst.Add(mo);
                }
            }

            return tmpLst;
        }

        public async Task<bool> InsertOne(MUnlockBlock mo,int zone)
        {
            StringBuilder sb = new StringBuilder();
            string addrFrom = mo.addrFrom;
            string addrTo = mo.addrTo;
            bool res = false;
            if (SqlAttack.IsDangerous(ref addrFrom) || SqlAttack.IsDangerous(ref addrTo))
            {
                return res;
            }           

            using (var conn = new MySqlConnection(connStr))
            {
                string d = string.Empty;
                if (mo.date != DateTime.MinValue)
                {
                    d = mo.date.ToString("yyyy-MM-dd");
                }

                var g = await IsExist(mo.height, mo.addrFrom, mo.addrTo);
                if (g.Count > 0)
                {
                    //update
                    sb.Append("UPDATE `UnlockedBlock` SET ");
                    sb.Append("`addrFrom`=@addrFrom,`addrTo`=@addrTo,");
                    sb.Append("`balance`=@balance,`timeSpan`=@timeSpan,`date`=@date,`height`=@height ");
                    sb.Append("WHERE `id` = " + g[0].id);
                    return false;
                }
                else
                {
                    //INSERT INTO `UnlockedBlock` (`addrFrom`,`addrTo`,`balance`,`timeSpan`,`date`,`height`) VALUES (
                    //@addrFrom, @addrTo, @balance, @timeSpan, @date, @height)
                    //WHERE NOT EXISTS (SELECT 1 FROM `UnlockedBlock` WHERE addrTo=@addrTo AND height=@height);

                    //insert
                    sb.Append("INSERT INTO `UnlockedBlock` (`addrFrom`,`addrTo`,`balance`,`timeSpan`,`date`,`height`) VALUES (");
                    sb.Append("@addrFrom, @addrTo, @balance, @timeSpan, @date, @height");
                    sb.Append(")");
                }

                var re = await conn.ExecuteAsync(sb.ToString(), new
                {
                    addrFrom = mo.addrFrom.Trim(),
                    addrTo = mo.addrTo.Trim(),
                    balance = mo.balance,
                    timeSpan = mo.timeSpan,
                    date = Convert.ToDateTime(mo.date).ToString("yyyy-MM-dd"),
                    height = mo.height
                });

                if (re > 0)
                {
                    res = true;
                }
                return res;
            }
        }

        public async Task<bool> InsertLst(List<MUnlockBlock> lst)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO `UnlockedBlock` (`id`,`addrFrom`,`addrTo`,`balance`,`timeSpan`,`date`,`height`) VALUES (");
            sb.Append("@id, @addrFrom, @addrTo, @balance, @timeSpan, @date, @height");
            sb.Append(")");

            using (var conn = new MySqlConnection(connStr))
            {
                var re = await conn.ExecuteAsync(sb.ToString(), lst);
                if (re > 0)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
