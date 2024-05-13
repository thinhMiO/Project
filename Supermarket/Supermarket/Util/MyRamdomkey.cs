using System.Text;

namespace Supermarket.Util
{
    public class MyRamdomkey
    {
        public static string GenerateRamdoKey(int length = 5)
        {
            var patttern = @"qazxswedcvfrtgbnhyujmkilopQAZXSWEDCVFRTGBNHYUJMKILOP!@#$%^&*";
            var sb = new StringBuilder();
            var rd = new Random();
            int l = patttern.Length;
            for(int i = 0; i < length; i++)
            {
                sb.Append(patttern[rd.Next(0,l)]);
            }
            return sb.ToString();
        }
    }
}
