using System;
using System.Collections.Generic;
using Sikiro.Model;

namespace Sikiro.SMSService.Model
{
    public class AddSmsModel
    {
        public string Content { get; set; }

        public SmsEnums.SmsType Type { get; set; }

        public List<string> Mobiles { get; set; }

        public DateTime? TimeSendDateTime { get; set; }

        /// <summary>
        /// 模板编码
        /// </summary>
        public string TemplateCode { get; set; }
        /// <summary>
        /// 扩展参数（使用场景：微信模板消息、支付宝生活号消息）
        /// </summary>
        public string Params { get; set; }
        /// <summary>
        /// 微信token
        /// </summary>
        public string Token { get; set; }
    }
}
