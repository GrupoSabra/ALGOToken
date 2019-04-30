﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AdvancedAlgos.AlgoToken.AlgoTokenDistribution;
using AdvancedAlgos.AlgoToken.AlgoTokenPlayground.Commands.ContractManagement;
using AdvancedAlgos.AlgoToken.AlgoTokenPlayground.Runtime;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace AdvancedAlgos.AlgoToken.AlgoTokenPlayground.Commands.AlgoStreamContract
{
    public class AlgoStreamDisableStreamCommand : EthInvokeTransactionalFunctionCommand
    {
        protected override async Task<TransactionReceipt> ExecuteAsync(RuntimeContext context, string contractAddress, Web3 web3)
        {
            var algoStream = new AlgoStream(contractAddress, web3, context.GasPriceProvider);

            return await algoStream.DisableStreamAsync();
        }
    }
}
