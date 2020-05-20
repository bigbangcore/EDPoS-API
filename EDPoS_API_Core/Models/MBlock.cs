using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Models
{
    public class MBlockPa
    {
        public string reward_address { get; set; }
        public decimal reward_money { get; set; }
    }

    public class MBlockDetail: MBlockPa
    {
        public int id { get; set; }
        public string hash { get; set; }
        public string fork_hash { get; set; }
        public string prev_hash { get; set; }
        public long time { get; set; }
        public int height { get; set; }
        public string type { get; set; }
        
        public Int16 is_useful { get; set; }
        public int bits { get; set; }
        public Int16 reward_state { get; set; }
    }
}
