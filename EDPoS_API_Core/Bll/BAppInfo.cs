using Dapper;
using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Bll
{
    public class BAppInfo
    {
        static string connStr = string.Empty;
        public BAppInfo(string str)
        {
            connStr = str;
        }

        public async Task<List<MAppInfo>> GetAppList(int id = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id,appID,appName,addTime,secretKey,note FROM `AppInfo` WHERE 1=1 ");
            if (id > 0)
            {
                sb.Append("AND id = " + id);
            }
            using (var conn = new MySqlConnection(connStr))
            {                
                var query = await conn.QueryAsync<MAppInfo>(sb.ToString());
                return query.ToList();
            }
        }

        public async Task<MAppInfo> GetAppInfo(string appid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id,appID,appName,addTime,secretKey,note FROM `AppInfo` ");
            sb.Append("WHERE appID = '" + appid + "'");

            using (var conn = new MySqlConnection(connStr))
            {
                var re = await conn.QueryFirstOrDefaultAsync<MAppInfo>(sb.ToString());
                return re;
            }
        }

        /// <summary>
        /// Add One App
        /// </summary>
        /// <param name="mo"></param>
        /// <returns></returns>
        public async Task<bool> InsertOne(MAppInfo mo)
        {
            bool res = false;
            string appID = mo.appID;
            string appName = mo.appName;
            string note = mo.note;
            string secretKey = mo.secretKey;

            if (SqlAttack.IsDangerous(ref appID) || 
                SqlAttack.IsDangerous(ref appName) || 
                SqlAttack.IsDangerous(ref note) || 
                SqlAttack.IsDangerous(ref secretKey))
            {
                return res;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO `AppInfo` (id,appID,appName,addTime,secretKey,note) VALUES (");
            sb.Append("@id, @appID, @appName, @addTime, @secretKey, @note");
            sb.Append(")");

            using (var conn = new MySqlConnection(connStr))
            {
                var re = await conn.ExecuteAsync(sb.ToString(), new
                {
                    id = 0,
                    appID = mo.appID.Trim(),
                    appName = mo.appName.Trim(),
                    addTime = mo.addTime,
                    secretKey = mo.secretKey,
                    note = mo.note
                });
                if (re > 0)
                {
                    res = true;
                }
                return res;
            }
        }
    }
}
