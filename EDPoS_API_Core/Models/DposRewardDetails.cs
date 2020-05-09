using System;

namespace EDPoS_API_Core.Models
{
    /// <summary>
    /// Reward details
    /// </summary>
    public class DposRewardDetails
    {
        /// <summary>
        /// Id
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// EDPOS address, It is used to reward voters
        /// </summary>
        public string dpos_addr { get; set; }

        /// <summary>
        /// Voter address
        /// </summary>
        public string client_addr { get; set; }

        /// <summary>
        /// Vote tokens
        /// </summary>
        public decimal vote_amount { get; set; }

        /// <summary>
        /// Reward tokens
        /// </summary>
        public decimal reward_money { get; set; }

        /// <summary>
        /// The datetime of compute
        /// </summary>
        public DateTime reward_date { get; set; }

        /// <summary>
        /// Block height
        /// </summary>
        public int block_height { get; set; }

        /// <summary>
        /// Status of payments
        /// </summary>
        public Int16 reward_state { get; set; }
    }
}
