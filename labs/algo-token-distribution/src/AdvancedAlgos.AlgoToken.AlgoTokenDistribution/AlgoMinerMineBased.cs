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
    public class AlgoMinerMineBased : AlgoMinerBase
    {
        private Function _mine;

        public AlgoMinerMineBased(Web3 web3, IGasPriceProvider gasPriceProvider) : base(web3, gasPriceProvider) { }
        public AlgoMinerMineBased(string contractAddress, Web3 web3, IGasPriceProvider gasPriceProvider) : base(contractAddress, web3, gasPriceProvider) { }

        protected override void Initialize(Contract contractDescriptor)
        {
            base.Initialize(contractDescriptor);

            _mine = contractDescriptor.GetFunction("mine");
        }

        public Task<TransactionReceipt> MineAsync() =>
            InvokeAsync(_mine, 900000);
    }
}
