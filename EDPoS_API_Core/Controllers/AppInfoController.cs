using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDPoS_API_Core.Bll;
using EDPoS_API_Core.Common;
using EDPoS_API_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EDPoS_API_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppInfoController : ControllerBase
    {
        private SiteConfig Config;
        static string connStr = string.Empty;

        /// <summary>
        /// constructed function
        /// </summary>
        /// <param name="option"></param>
        public AppInfoController(IOptions<SiteConfig> option)
        {
            Config = option.Value;
            connStr = SqlConn.GetConn(Config);
        }

        // GET: api/AppInfo/5
        [HttpGet]
        public async Task<string> Get(int id = 0)
        {
            Result<List<MAppInfo>> res = new Result<List<MAppInfo>>();
            try
            {
                BAppInfo bll = new BAppInfo(connStr);
                var lst = await bll.GetAppList(id);
                foreach (var v in lst)
                {
                    v.secretKey = "******";
                }
                res = new Result<List<MAppInfo>>(ResultCode.Ok, null, lst);
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<List<MAppInfo>>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }

        /// <summary>
        /// Add a app info
        /// </summary>
        /// <param name="mo">MAppInfoPwd</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Post(MAppInfoPwd mo)
        {
            Result<string> res = new Result<string>();
            try
            {
                var configPWD = AppConfigurtaionServices.Configuration.GetSection("PwdOfApp").Value;
                var sign = Encrypt.HmacSHA256(configPWD, configPWD);
                if (!sign.Equals(mo.pwd))
                {
                    res = new Result<string>(ResultCode.LoginFail, null, null);
                }
                else
                {
                    Guid appID_tmp;
                    if (!Guid.TryParse(mo.appID, out appID_tmp))
                    {
                        res = new Result<string>(ResultCode.Fail, "appID must be a GUID.", "");
                    }
                    else
                    {
                        MAppInfo mot = new MAppInfo();
                        mot.id = mo.id;
                        mot.note = mo.note;
                        mot.secretKey = mo.secretKey;
                        mot.appName = mo.appName;
                        mot.appID = mo.appID;
                        mot.addTime = mo.addTime;

                        BAppInfo bll = new BAppInfo(connStr);
                        var re = await bll.InsertOne(mot);

                        res = new Result<string>(re ? ResultCode.Ok : ResultCode.Fail, null, re.ToString());
                    }                    
                }
                return JsonConvert.SerializeObject(res);
            }
            catch (Exception ex)
            {
                res = new Result<string>(ResultCode.Fail, ex.Message, null);
                return JsonConvert.SerializeObject(res);
            }
        }
    }
}
