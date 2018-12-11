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
        /// 扩展参数（使用场景：微信模板消息、支付宝生活号消息）
        /// 微信：{"touser":"","template_id":"","data":{"first":{"value":"订单取消","color":"#173177"},"keyword1":{"value":"1234545787878","color":"#173177"},"keyword2":{"value":"支付超时","color":"#173177"},"keyword3":{"value":"2018-10-04 08:54:35","color":"#173177"},"keyword4":{"value":"2018-10-04 08:55:35","color":"#173177"},"remark":{"value":"感谢您的关注","color":"#173177"}}}
        /// 支付宝：
        /// 极光：
        /// </summary>
        /// <remarks>
        /// <remark>
        /// 微信：https://www.cnblogs.com/stoneniqiu/p/7091501.html
        /// </remark>
        /// </remarks>
        public string Params { get; set; }
        ///// <summary>
        ///// 跳转地址
        ///// </summary>
        //public string Url { get; set; }
        ///// <summary>
        ///// 标题颜色
        ///// </summary>
        //public string TopColor { get; set; }
        ///// <summary>
        ///// 模板数据
        ///// </summary>
        //public TemplateData Data { get; set; }


    }

    /// <summary>
    /// 模板数据
    /// </summary>
    public class TemplateData
    {
        /// <summary>
        /// 
        /// </summary>
        public TempItem First { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TempItem Keyword1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TempItem Keyword2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TempItem Remark { get; set; }
    }

    /// <summary>
    /// 模板项
    /// </summary>
    public class TempItem
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public TempItem(string value, string color = "#173177")
        {
            Value = value;
            Color = color;
        }
        /// <summary>
        /// 模板项值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 模板项颜色
        /// </summary>
        public string Color { get; set; }
    }
}
