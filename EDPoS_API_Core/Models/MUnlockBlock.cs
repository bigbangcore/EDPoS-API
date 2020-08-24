using System;
using System.Collections.Generic;

namespace EDPoS_API_Core.Models
{
    public class MUnlockBlockBase
    {
        public string addrFrom { get; set; }
        public DateTime date { get; set; }
    }
    public class MUnlockBlockReward
    {
        public string addrTo { get; set; }
        public Decimal balance { get; set; }
        public long timeSpan { get; set; }
        public int height { get; set; }
    }
    public class MUnlockBlock : MUnlockBlockBase
    {   
        public Int64 id { get; set; }
        public string addrTo { get; set; }
        public Decimal balance { get; set; }
        public long timeSpan { get; set; }
        public int height { get; set; }
    }

    public class MUnlockBlockLst : MUnlockBlockBase
    {
        public List<MUnlockBlockReward> balanceLst { get; set; }
    }

    public class MUnlockBlockLstWithSign : MValid
    {
        public List<MUnlockBlock> balanceLst { get; set; }
    }
}
