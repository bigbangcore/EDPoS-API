using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDPoS_API_Core.Common
{
    /// <summary>
    /// Valid class
    /// </summary>
    public class DataValid
    {
        /// <summary>
        /// Valid.Ignore case
        /// </summary>
        /// <param name="requestSign">Result of Client Signature,appID + ":" + timeSpan + ":" + signPlain</param>
        /// <param name="appID">appid</param>
        /// <param name="signPlain">The data that awaiting signature</param>
        /// <param name="timeSpan">TimeSpan</param>
        /// <param name="secretKey">Key</param>
        /// <returns></returns>
        public static bool Valid(string requestSign,string appID, string signPlain, string timeSpan, string secretKey)
        {
            if (string.IsNullOrEmpty(timeSpan) || string.IsNullOrEmpty(requestSign) || string.IsNullOrEmpty(appID) || string.IsNullOrEmpty(signPlain))
            {
                return false;                
            }

            long requestTime = 0;
            if (long.TryParse(timeSpan, out requestTime))
            {
                var dnow = DateTime.Now.ToUniversalTime().Ticks;
                var dstart = 621355968000000000;
                var dinterval = 5 * 60;
                var max = (dnow - dstart) / 10000000;
                var min = (dnow - dstart) / 10000000 - dinterval;

                if (!(max >= requestTime && min <= requestTime))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            //hashmac
            signPlain = appID + ":" + timeSpan + ":" + signPlain;
            var sign = Encrypt.HmacSHA256(secretKey, signPlain);
            return requestSign.Equals(sign, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
