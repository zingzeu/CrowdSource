using System;

namespace CrowdSource.Helpers.AutoCorrectors 
{
    public class BanJiaoAutoCorrector : IAutoCorrector {

        public string Apply(string input) {
            return GetBanJiao(input);
        }
        private string GetBanJiao(string input)
        {
            //http://www.cnblogs.com/songtzu/archive/2012/08/07/2627239.html
            if (input == null)
            {
                return null;
            }
            char[] cc = input.ToCharArray();
            for (int i = 0; i < cc.Length; i++)
            {
                if (cc[i] == 12288)
                {
                    // 表示空格  
                    cc[i] = (char)32;
                    continue;
                }
                if (cc[i] > 65280 && cc[i] < 65375)
                {
                    cc[i] = (char)(cc[i] - 65248);
                }

            }
            return new string(cc);
        }
    }
}