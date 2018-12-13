using System.Collections.Generic;
using EasyNetQ;
using System;

namespace Sikiro.Model
{
    [Queue(SmsQueueModelKey.Queue, ExchangeName = SmsQueueModelKey.Exchange)]
    public class SmsQueueModel
    {
        public string Content { get; set; }

        public SmsEnums.SmsType Type { get; set; }

        public List<string> Mobiles { get; set; }

        public int SendCount { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? TimeSendDateTime { get; set; }
        /// <summary>
        /// 模板编码
        /// </summary>
        public string TemplateCode { get; set; }
        /// <summary>
        /// 扩展参数（使用场景：微信模板消息、支付宝生活号消息）
        /// </summary>
        public object Params { get; set; }
        /// <summary>
        /// 微信token
        /// </summary>
        public string Token { get; set; }
    }

    public static class SmsQueueModelKey
    {
        public const string Queue = "Queue.SMS";
        public const string Exchange = "Exchange.SMS";
        public const string Topic = "Topic.SMS";
    }
}
