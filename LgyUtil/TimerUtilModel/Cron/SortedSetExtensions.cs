using System;
using System.Collections.Generic;
using System.Text;

namespace LgyUtil.TimerUtilModel.Cron
{
    internal static class SortedSetExtensions
    {
        internal static SortedSet<int> TailSet(this SortedSet<int> set, int value)
        {
            return set.GetViewBetween(value, 9999999);
        }
    }
}
