using System;

namespace EDPoS_API_Core.Models
{
    public class DposAddrDailyAll
    {
        /// <summary>
        /// EDPOS address, It is used to reward voters
        /// </summary>
        public string dpos_addr { get; set; }

        /// <summary>
        /// The amount of pay out
        /// </summary>
        public decimal payment_money { get; set; }

    }
    /// <summary>
    /// Dpos address daily reward
    /// </summary>
    public class DposAddrDaily : DposAddrDailyAll
    {
        /// <summary>
        /// The datetime of reward voters
        /// </summary>
        public DateTime payment_date { get; set; }
    }
    /// <summary>
    /// Daily reward
    /// </summary>
    public class DposDailyReward : DposAddrDaily
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id { get; set; }        

        /// <summary>
        /// voter address
        /// </summary>
        public string client_addr { get; set; }

        /// <summary>
        /// The return id of dpos node had payed
        /// </summary>
        public string txid { get; set; }
    }
}
