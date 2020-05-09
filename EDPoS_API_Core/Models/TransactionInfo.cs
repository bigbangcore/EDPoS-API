using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Models
{
    public class TransactionInfo
    {
        public int id { get; set; }
        public string block_hash { get;set;}
        public string txid { get; set; }
        public string form { get; set; }
        public string to { get; set; }
        public decimal amount { get; set; }
        public decimal free { get; set; }
        public string type { get; set; }
        public int lock_until { get; set; }
        public int n { get; set; }
        public string spend_txid { get; set; }
        public string data { get; set; }

        /// <summary>
        /// The dpos address of vote to
        /// </summary>
        public string dpos_in { get; set; }

        /// <summary>
        /// The address of voter
        /// </summary>
        public string client_in { get; set; }

        /// <summary>
        /// The dpos address of redeem to
        /// </summary>
        public string dpos_out { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string client_out { get; set; }
    }
}
