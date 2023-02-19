using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleTicket
{
    public class CaptchaResult
    {
        public string Captchacode { get; set; }

        public byte[] CaptchaByteData { get; set; }

        public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteData);

        public DateTime Timestamp { get; set; }
    }
}
