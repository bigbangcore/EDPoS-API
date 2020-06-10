using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Models
{
    /// <summary>
    /// Valid Info
    /// </summary>
    public class MValid
    {
        /// <summary>
        /// Result of Client Signature,appID + ":" + timeSpan + ":" + signPlain
        /// </summary>
        public string requestSign { get; set; }

        /// <summary>
        /// appID
        /// </summary>
        public string appID { get; set; }

        /// <summary>
        /// The data that awaiting signature
        /// </summary>
        public string signPlain { get; set; }

        /// <summary>
        /// TimeSpan
        /// </summary>
        public string timeSpan { get; set; }
    }
}
