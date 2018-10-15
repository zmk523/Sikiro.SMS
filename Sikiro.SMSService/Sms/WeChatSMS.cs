using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sikiro.SMS.Toolkits;
using Sikiro.SMSService.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sikiro.SMSService.Sms
{
    /// <summary>
    /// 微信消息
    /// <remark>
    /// 帮助文档：https://mp.weixin.qq.com/advanced/tmplmsg?action=faq&token=992938580&lang=zh_CN
    /// </remark>
    /// </summary>
    public class WeChatSMS : BaseSMS
    {
        private const string accesToken = "14_eTdkmn4D_vtNI47t8AOTuUuSBkAKDDuQR1s9I0hSrl0rIIX0RmC0qDfF2R-c5GGOD6PYYWg20pGgqlm0TLyKMTo6MmPBMlrKfWVvXQOWqWblsN4SZ7Fgq6BHkLnTBL_y71bu5M4l8j1ao4arJBRbAAARJZ";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public WeChatSMS(IConfiguration configuration) : base(configuration)
        {
            var config = configuration.Get<SmsConfig>();
            Account = config.Sms.WeChatSMS.Account;
            Password = config.Sms.WeChatSMS.Password;
            Url = config.Sms.WeChatSMS.Url;
            MaxCount = config.Sms.WeChatSMS.MaxCount;
        }

        public override bool SendSMS(string phone, string content, string signName, string templateCode = "", string _params = "")
        {
            try
            {
                string strJson = HttpHelper.HttpPost(string.Format(Url, accesToken), _params);
                var model = JsonConvert.DeserializeObject<M_WeChatSMS>(strJson);
                Console.WriteLine($"WeChatSMS：{strJson}  {DateTime.Now}{Environment.NewLine}");
                return model.errcode == 0 ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class M_WeChatSMS
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public string msgid { get; set; }
    }
}
