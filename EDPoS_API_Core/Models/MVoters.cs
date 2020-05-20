using System;

namespace EDPoS_API_Core.Models
{
    public class MVoters
    {
        public int height { get; set; }
        public string votedate { get; set; }
        public string from { get; set; }
        public string amount { get; set; }
        public string type { get; set; }
    }

    public class MVotingTokens
    {
        public string dpos_addr { get; set; }
        public decimal audit_money { get; set; }
    }

    public class MVotersReward : MVotingTokens
    {            
        public decimal payment_money { get; set; }
        public decimal profit { get; set; }
        public decimal avg_profit { get; set; }
    }
}
