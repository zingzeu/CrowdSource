using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Services
{
    public class TextSanitizer : ITextSantizer
    {
        //http://www.cnblogs.com/songtzu/archive/2012/08/07/2627239.html
        public string BanJiao(string input)
        {
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

    public interface ITextSantizer
    {
        string BanJiao(string input);
    }
}
