using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AdvancedAlgos.AlgoToken.AlgoTokenDistribution.Algorithms
{
    public static class AlgoMinerFeeAlgorithm
    {
        private const long FEE_LAST_AMOUNT_INIT = 1902694;
        private const long FEE_TRANSIT_COEF = 998102770;
        private const long FEE_FACTOR = 1000000000;

        public static BigInteger CalculateFees(BigInteger categorySupply, int minedDays)
        {
            BigInteger currentAmountTaken = FEE_LAST_AMOUNT_INIT;
            BigInteger minerFees = 0;
            for (int day = 0; day < minedDays; day++)
            {
                currentAmountTaken = currentAmountTaken * FEE_TRANSIT_COEF / FEE_FACTOR;
                minerFees += currentAmountTaken * categorySupply / FEE_FACTOR;
            }
            return minerFees;
        }
    }
}
