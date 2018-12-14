using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sikiro.Model;

namespace Sikiro.SMS.Api.Model.Sms
{
    /// <summary>
    /// 消息实体
    /// </summary>
    public class PostModel
    {
        /// <summary>
        /// 短信内容
        /// </summary>
        [Required, Display(Name = "短信内容")]
        public string Content { get; set; }
        /// <summary>
        /// 消息类型 0 JisnZhou,1 WoDong,2 EXunTong,3 Aliyun,4 WeChat,5 JiGuang
        /// </summary>
        public SmsEnums.SmsType Type { get; set; }
        /// <summary>
        /// 手机号/微信openid
        /// </summary>
        public List<string> Mobiles { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? TimeSendDateTime { get; set; }
        
        /// <summary>
        /// 模板编码
        /// </summary>
        public string TemplateCode { get; set; }
        /// <summary>
        /// 扩展参数[json]（使用场景：微信模板消息、支付宝生活号消息）
        /// 微信：{"first":{"value":"订单取消","color":"#173177"},"keyword1":{"value":"1234545787878","color":"#173177"},"keyword2":{"value":"支付超时","color":"#173177"},"keyword3":{"value":"2018-10-04 08:54:35","color":"#173177"},"keyword4":{"value":"2018-10-04 08:55:35","color":"#173177"},"remark":{"value":"感谢您的关注","color":"#173177"}}
        /// 支付宝：
        /// 极光：{"cid":"8888888888","platform":"all","audience":{"registration_id":["1507bfd3f79532f49e3"]},"notification":{"alert":"Hello, JPush!"}}
        /// </summary>
        /// <remarks>
        /// <remark>
        /// 微信：https://www.cnblogs.com/stoneniqiu/p/7091501.html
        /// </remark>
        /// </remarks>
        public string Params { get; set; }
        /// <summary>
        /// 微信token
        /// </summary>
        public string Token { get; set; }     
    }   
}
