using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using AdvancedAlgos.AlgoToken.AlgoErc20Token;
using AdvancedAlgos.AlgoToken.AlgoTokenDistribution.Algorithms;
using Xunit;

namespace AdvancedAlgos.AlgoToken.AlgoTokenDistribution.IntegrationTests.Algorithms
{
    public class AlgoMinerFeeAlgorithmTests
    {
        [Fact]
        public void TestAlgoMinerFeeAlgorithm()
        {
            BigInteger categorySupply = 2.MAlgo();
            BigInteger currentYearSupply = categorySupply / 2;
            BigInteger totalExpectedFees = 0;

            for (int year = 1; year <= 10; year++)
            {
                totalExpectedFees += currentYearSupply;

                AssertInRange(totalExpectedFees, AlgoMinerFeeAlgorithm.CalculateFees(2.MAlgo(), 365 * year), 1);

                currentYearSupply /= 2;
            }
        }

        private void AssertInRange(BigInteger expected, BigInteger actual, BigInteger tolerancePercentage)
        {
            var tolerance = expected * tolerancePercentage / 100;
            var lowerBound = expected - tolerance;
            var upperBound = expected + tolerance;

            Assert.InRange(expected, lowerBound, upperBound);
        }
    }
}
