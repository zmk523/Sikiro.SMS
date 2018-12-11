using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Sikiro.Nosql.Mongo.Base;

namespace Sikiro.Model
{
    [Mongo(MongoKey.SmsDataBase, MongoKey.SmsCollection)]
    public class SmsModel : MongoEntity
    {
        public string Content { get; set; }

        public SmsEnums.SmsType Type { get; set; }

        public SmsEnums.SmsStatus Status { get; set; }

        public List<string> Mobiles { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDateTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? TimeSendDateTime { get; set; }

        public int SendCount { get; set; }
        /// <summary>
        /// 模板编码
        /// </summary>
        public string TemplateCode { get; set; }
        /// <summary>
        /// 扩展参数（使用场景：微信模板消息、支付宝生活号消息）
        /// </summary>
        public string Params { get; set; }
    }
}
