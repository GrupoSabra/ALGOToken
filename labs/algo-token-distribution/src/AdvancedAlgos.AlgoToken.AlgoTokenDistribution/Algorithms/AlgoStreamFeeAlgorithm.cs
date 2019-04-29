using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AdvancedAlgos.AlgoToken.AlgoTokenDistribution.Algorithms
{
    public static class AlgoStreamFeeAlgorithm
    {
        private static readonly BigInteger TOKEN_FACTOR = 1_000_000_000_000_000_000u;
        private static readonly BigInteger DRIP_VALUE = 100000u * TOKEN_FACTOR;
        private static readonly BigInteger DAYS_PER_YEAR = 365u;

        public static BigInteger CalculateFees(ulong size, ulong days, ulong startDay = 0)
        {
            BigInteger fees = 0;
            for (BigInteger day = 0; day < days; day++)
            {
                BigInteger year = (day + startDay) / DAYS_PER_YEAR;
                BigInteger yearSupply = DRIP_VALUE * size * 12u / 10u / (year + 1);
                BigInteger daySupply = yearSupply / DAYS_PER_YEAR;
                fees += daySupply;
            }
            return fees;
        }
    }
}
