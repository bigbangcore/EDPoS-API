using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // 0c9f88295e9e38de2c278535da18ef3ba730b27af4ee26cd68aa4c82f141fb3f
            /*string secretKey = "bbc03.bbcpool.io";
            string plain = "9846bd82-9360-4838-b8ae-0ccd8c451600:1591939936:6666";
            var re = Encrypt.HmacSHA256(secretKey, plain);
            Console.WriteLine(re);

            var dt = Convert.ToDateTime("2020-07-06");
            var vv = Convert.ToInt64(CommonHelper.GetTimeStamp(dt)) / 1000;
            Console.WriteLine(vv);
            */
            while (true)
            {
                Console.Write("请输入日期，输入exit或q退出：");
                var str = Console.ReadLine();
                if (str.ToLower().Equals("exit") || str.ToLower().Equals("q"))
                {
                    Environment.Exit(0);
                }
                else
                {
                    DateTime dt;
                    if(DateTime.TryParse(str,out dt)){
                        str = dt.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        Console.WriteLine("日期格式不对，重新输入。\n");
                        continue;
                    }
                }
                Console.WriteLine("查询结果：");
                WebApiTest_AddProduct(str);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// eg. 2020-07-22
        /// </summary>
        /// <param name="date"></param>
        public static void WebApiTest_AddProduct(string date)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://edposapi.bbcpool.io/");

                var requestJson = JsonConvert.SerializeObject(
                new
                {
                    addrFrom = "",
                    addrTo = "",
                    data = "2020-06-27"
                });

                HttpContent httpContent = new StringContent(requestJson);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = client.GetAsync("api/PowPoolDailyReward?date=" + date).Result.Content.ReadAsStringAsync().Result;

                var obj = JsonConvert.DeserializeObject<Result<List<MPowPoolDailyReward>>>(result);

                List<Reward> lst = new List<Reward>();
                Dictionary<string, Decimal> dic = new Dictionary<string, decimal>();
                foreach (var item in obj.Data)
                {
                    if (dic.ContainsKey(item.addrFrom))
                    {
                        dic[item.addrFrom] += item.reward;
                    }
                    else
                    {
                        dic.Add(item.addrFrom, item.reward);
                    }                   
                }

                /*
                 select sum(reward_money) sumA,reward_address from Block WHERE `type` ='primary-dpos' and time > 1594022400 and time < 1594108800 and is_useful=1  GROUP BY reward_address;
                 select sum(reward) sumB,addrFrom from PowPoolDailyReward WHERE settlementDate='2020-07-13'  GROUP BY addrFrom;
                 */

                // 122410.2500000000
                // 119309.1100000000

                foreach (var item in dic)
                {
                    Console.WriteLine(item.Key + ":" + item.Value);
                }
            }
        }
    }

    public class Reward
    {
        public string addr { get; set; }
        public string date { get; set; }
        public Decimal amount { get; set; }
    }

    public class MPowPoolDailyReward
    {
        public Int64 id { get; set; }
        public string addrFrom { get; set; }
        public string addrTo { get; set; }
        public decimal reward { get; set; }
        public string settlementDate { get; set; }
    }
}
