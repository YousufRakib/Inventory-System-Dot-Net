using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastrucutre.Constants
{
    public static class AppUtil
    {
        public enum EntryType
        {
            StockEntry = 1,
            StockShift = 2,
            OrderDeduction = 3,
            FBADeduction = 4
        }
    }
}
