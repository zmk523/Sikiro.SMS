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

        public override bool SendSMS(string phone, string content, string signName, string templateCode = "", object _params = null, string token = "")
        {
            try
            {
                string strJson = HttpHelper.HttpPost(string.Format(Url, token),"{\"touser\":\""+ phone + "\",\"template_id\":\""+ templateCode + "\",\"data\":"+ _params + "}" );
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
