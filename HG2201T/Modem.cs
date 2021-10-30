using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using LibModemBase;

namespace HG2201T
{
    public class Modem : ModemBase
    {
        public static new string modem = "HG2201T";
        public static new bool needUser = true;
        public static new bool needLogin = false;
        public Modem(string host, string user, string passwd) : base(host, user, passwd) { }
        public override string getModemInfo()
        {
            string url = $"http://{host}:8080/cgi-bin/baseinfoSet.cgi";
            string body = EasyWeb.Get(url).body;
            Regex regex = new Regex("\"baseinfoSet_TELECOMACCOUNT\":\"(.*?)\",");
            string telaccount = regex.Match(body).Groups[1].Value;
            regex = new Regex("\"baseinfoSet_TELECOMPASSWORD\":\"(.*?)\",");
            string telpasswd = regex.Match(body).Groups[1].Value.Trim('&');
            string[] pcode = telpasswd.Split("&");
            telpasswd = "";
            for (int i = 0; i < pcode.Length; i++) {
                int code = int.Parse(pcode[i]);
                code = code - 4 >= 'a' ? code - 4 : code;
                telpasswd += char.ConvertFromUtf32(code);
            }
            url = $"http://{host}:8080/cgi-bin/broadbund.cgi";
            body = EasyWeb.Get(url).body;
            regex = new Regex("\"broadbund_pppusername\":\"(.*?)\",");
            string pppoeuser = regex.Match(body).Groups[1].Value;
            regex = new Regex("\"broadbund_ppppassword\":\"(.*?)\",");
            string pppoepasswd = regex.Match(body).Groups[1].Value;

            return $"型号:{modem}\n\n超级账号:{telaccount}\n超级密码:{telpasswd}\n\n宽带账号:{pppoeuser}\n宽带密码:{pppoepasswd}";
        }
    }
}
