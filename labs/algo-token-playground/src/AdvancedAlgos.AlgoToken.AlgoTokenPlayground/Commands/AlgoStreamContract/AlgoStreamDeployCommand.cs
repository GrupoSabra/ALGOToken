using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdvancedAlgos.AlgoToken.AlgoTokenDistribution;
using AdvancedAlgos.AlgoToken.AlgoTokenPlayground.Commands.ContractManagement;
using AdvancedAlgos.AlgoToken.AlgoTokenPlayground.Runtime;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace AdvancedAlgos.AlgoToken.AlgoTokenPlayground.Commands.AlgoStreamContract
{
    public class AlgoStreamDeployCommand : DeployContractCommand
    {
        public override string DefaultName => "AlgoStream";

        public byte Size { get; set; }
        public byte GracePeriodDays { get; set; }
        public string StreamHolderAddress { get; set; }
        public string ReferralAddress { get; set; }
        public string TokenAddress { get; set; }

        protected override async Task<TransactionReceipt> DeployContractAsync(RuntimeContext context, Web3 web3)
        {
            var algoStream = new AlgoStream(web3, context.GasPriceProvider);
            return await algoStream.DeployAsync(
                Size,
                GracePeriodDays,
                context.ResolveAccountReference(StreamHolderAddress),
                context.ResolveAccountReference(ReferralAddress),
                context.ResolveContractReference(TokenAddress));
        }
    }
}
