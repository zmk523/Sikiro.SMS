using Sikiro.Model;
using Sikiro.SMSService.Base;
using Sikiro.SMSService.Interfaces;

namespace Sikiro.SMSService.Sms
{
    public class SmsFactory : IService
    {
        private readonly WoDongSMS _woDongSms;
        private readonly JianZhouSMS _jianZhouSms;
        private readonly EXunTongSMS _eXunTongSms;
        private readonly AliyunSMS _aliyunSms;
        private readonly WeChatSMS _weChatSms;
        private readonly JiGuangSMS _jiGuangSms;

        public SmsFactory(WoDongSMS woDongSms, JianZhouSMS jianZhouSms, EXunTongSMS eXunTongSms, AliyunSMS aliyunSms, WeChatSMS weChatSms, JiGuangSMS jiGuangSms)
        {
            _woDongSms = woDongSms;
            _jianZhouSms = jianZhouSms;
            _eXunTongSms = eXunTongSms;
            _aliyunSms = aliyunSms;
            _weChatSms = weChatSms;
            _jiGuangSms = jiGuangSms;

        }

        public BaseSMS Create(SmsEnums.SmsType type)
        {
            switch (type)
            {
                case SmsEnums.SmsType.JisnZhou: return _jianZhouSms;
                case SmsEnums.SmsType.WoDong: return _woDongSms;
                case SmsEnums.SmsType.EXunTong: return _eXunTongSms;
                case SmsEnums.SmsType.Aliyun: return _aliyunSms;
                case SmsEnums.SmsType.WeChat: return _weChatSms;
                case SmsEnums.SmsType.JiGuang: return _jiGuangSms;

                default: throw new SmsException("无法识别的type");
            }
        }
    }
}
