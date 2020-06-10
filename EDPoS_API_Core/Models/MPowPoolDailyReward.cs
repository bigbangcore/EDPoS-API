using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Models
{
    public class MPowPoolDailyReward
    {
        public Int64 id { get; set; }
        public string addrFrom { get; set; }
        public string addrTo { get; set; }
        public decimal reward { get; set; }
        public string settlementDate { get; set; }
    }

    public class MPowPoolDailyRewardWithValid : MValid
    {
        /// <summary>
        /// 
        /// </summary>
        public List<MPowPoolDailyReward> rewardLst { get; set; }
    }

    public class MPowPoolDailyRewardWithHash
    {
        /// <summary>
        /// 
        /// </summary>
        public string hash { get; set; }
        public string hashObj { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<MPowPoolDailyReward> rewardLst { get; set; }
    }
}
