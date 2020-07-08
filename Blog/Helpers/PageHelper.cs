using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Helpers
{
    public static class PageHelper //static cause doesn't depend on any state, we dont need any instatiation, we just say go to this class and use this 
    {
        public static IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {
            int midPoint = pageNumber;

            if (midPoint < 3)
                midPoint = 3;
            else if (midPoint > pageCount - 2)
                if (pageCount - 2 < 3)
                    midPoint = 3;
                else
                    midPoint = pageCount - 2;

            int lowerBound = midPoint - 2;
            int upperBound = midPoint + 2;

            if (lowerBound != 1)
            {
                yield return 1;
                if (lowerBound - 1 > 1)
                {
                    yield return -1;
                }
            }

            for (int i = midPoint - 2; i <= upperBound; i++)
            {
                if (i <= pageCount)
                    yield return i;
            }

            if (upperBound != pageCount && upperBound < pageCount)
            {
                if (pageCount - upperBound > 1)
                {
                    yield return -1;
                }
                yield return pageCount;
            }
        }
    }
}
