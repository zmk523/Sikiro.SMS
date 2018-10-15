namespace Sikiro.SMSService.Base
{
    public class SmsConfig
    {
        public Sms Sms { get; set; }
    }

    public class Sms
    {
        public string SignName { get; set; }

        public Jianzhousms JianZhouSMS { get; set; }

        public Wodongsms WoDongSMS { get; set; }

        public EXuntongsms EXunTongSMS { get; set; }

        public Aliyunsms AliyunSMS { get; set; }

        public WeChatsms WeChatSMS { get; set; }
    }

    public class Jianzhousms
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public int MaxCount { get; set; }
    }

    public class Wodongsms
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
        public int MaxCount { get; set; }
    }

    public class EXuntongsms
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
        public int MaxCount { get; set; }
    }

    public class Aliyunsms
    {
        public string EndpointName { get; set; }
        public string RegionId { get; set; }
        public string Product { get; set; }
        public string Domain { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public int MaxCount { get; set; }
    }

    public class WeChatsms {

        public string Account { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public int MaxCount { get; set; }

    }
}
