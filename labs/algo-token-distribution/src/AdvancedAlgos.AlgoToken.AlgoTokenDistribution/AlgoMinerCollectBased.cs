using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AdvancedAlgos.AlgoToken.Framework.Ethereum;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace AdvancedAlgos.AlgoToken.AlgoTokenDistribution
{
    public class AlgoMinerCollectBased : AlgoMinerBase
    {
        private Function _collect;

        public AlgoMinerCollectBased(Web3 web3, IGasPriceProvider gasPriceProvider) : base(web3, gasPriceProvider) { }
        public AlgoMinerCollectBased(string contractAddress, Web3 web3, IGasPriceProvider gasPriceProvider) : base(contractAddress, web3, gasPriceProvider) { }

        protected override void Initialize(Contract contractDescriptor)
        {
            base.Initialize(contractDescriptor);

            _collect = contractDescriptor.GetFunction("collect");
        }

        public Task<TransactionReceipt> CollectAsync() =>
            InvokeAsync(_collect, 1400000);
    }
}
