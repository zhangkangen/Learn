using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformCommon
{
    public class ValidateCode
    {
        public ValidateCode()
        {

        }

        public int MaxLength { get { return 10; } }
        public int MinLength { get { return 1; } }


        public string CreateValidateCode(int length)
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumbersStr = "";
            int seekSeek = 0;
            return validateNumbersStr;
        }
    }
}
