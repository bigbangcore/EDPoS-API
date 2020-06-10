using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Models
{
    public class MAppInfo
    {
        public int id { get; set; }
        /// <summary>
        /// GUID
        /// </summary>
        public string appID { get; set; }

        /// <summary>
        /// App name
        /// </summary>
        public string appName { get; set; }
        public DateTime addTime { get; set; }

        /// <summary>
        /// Your secret key
        /// </summary>
        public string secretKey { get; set; }
        public string note { get; set; }
    }

    public class MAppInfoPwd : MAppInfo
    {
        /// <summary>
        /// The password owned by administrator，sha256 encryption before submission
        /// </summary>
        public string pwd { get; set; }
    }
}
