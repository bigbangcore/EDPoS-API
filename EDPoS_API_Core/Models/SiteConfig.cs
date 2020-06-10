using System.Text;

namespace EDPoS_API_Core.Models
{
    public class SiteConfig
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string DataBase { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        //public string Charset { get; set; }
    }
    
    public class SqlConn
    {
        public static string GetConn(SiteConfig config)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("server=");
            sb.Append(config.Host);
            sb.Append(";database=");
            sb.Append(config.DataBase);
            sb.Append(";PORT=");
            sb.Append(config.Port);
            sb.Append(";uid=");
            sb.Append(config.UserName);
            sb.Append(";pwd=");
            sb.Append(config.Password);
            //sb.Append(";Charset=");
            //sb.Append(config.Charset);
            return sb.ToString();
        }
    }
}
