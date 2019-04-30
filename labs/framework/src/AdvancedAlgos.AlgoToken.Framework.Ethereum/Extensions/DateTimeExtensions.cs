using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedAlgos.AlgoToken.Framework.Ethereum.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToEpoch(this DateTime date)
        {
            return (long)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}
