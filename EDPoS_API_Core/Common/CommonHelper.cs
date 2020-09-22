using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Common
{
    public class CommonHelper
    {
        /// <summary>
        /// Get the TimeSpan
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        /// <summary>
        /// Get the TimeSpan by DateTime,the datetime should be convert to UniversalTime first
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime dt,int zone = 0)
        {
            //var zone = (-int.Parse(DateTime.Now.ToString("%z")));
            //由于服务器上的时区为世界协调时间，为了适应东八区的查询条件，我们这里得给它减去8
            TimeSpan ts = dt.AddHours(-(8 - zone)).ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
    }
}
