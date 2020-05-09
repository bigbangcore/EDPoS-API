using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Models
{
    /// <summary>
    /// About pool
    /// </summary>
    public class Pool
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// pool address
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// pool name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// pool key
        /// </summary>
        public string key { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    class JsonPool
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string md5 { get; set; }
    }
}
