using System;
using System.Collections.Generic;
using System.Text;

namespace MooyongCommon
{
    public class TokenParameter
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }
        /// <summary>
        /// 签名签发人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// Token的有效分钟数。过了这个时间，这个Token会过期。
        /// </summary>
        public int AccessExpiration { get; set; }
        /// <summary>
        /// RefreshExpiration的有效分钟数。过了这个时间，用户需要重新登录。
        /// </summary>
        public int RefreshExpiration { get; set; }
    }
}
