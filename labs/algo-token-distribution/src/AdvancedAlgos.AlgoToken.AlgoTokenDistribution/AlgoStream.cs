using AdvancedAlgos.AlgoToken.Framework.Ethereum;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedAlgos.AlgoToken.AlgoTokenDistribution
{
    public class AlgoStream : SmartContract<AlgoStream>
    {
        private Function _activateStream;
        private Function _pauseStreaming;
        private Function _resumeStreaming;
        private Function _disableStream;
        private Function _resetStream;
        private Function _changeStreamSize;
        private Function _collect;
        private Function _terminate;

        private Function _setCurrentDateTime;

        public AlgoStream(Web3 web3, IGasPriceProvider gasPriceProvider) : base(web3, gasPriceProvider) { }
        public AlgoStream(string contractAddress, Web3 web3, IGasPriceProvider gasPriceProvider) : base(contractAddress, web3, gasPriceProvider) { }

        protected override string AbiResourceName => $"SmartContracts.src.bin.AlgoStream.abi";
        protected override string BinResourceName => $"SmartContracts.src.bin.AlgoStream.bin";
        protected override BigInteger DeploymentGasUnits => 3400000;

        public Task<TransactionReceipt> DeployAsync(byte size, byte gracePeriodDays, string streamHolderAddress, string referralAddress, string tokenAddress)
            => base.DeployAsync(size, gracePeriodDays, streamHolderAddress, referralAddress, tokenAddress);

        protected override void Initialize(Contract contractDescriptor)
        {
            _activateStream = contractDescriptor.GetFunction("activateStream");
            _pauseStreaming = contractDescriptor.GetFunction("pauseStreaming");
            _resumeStreaming = contractDescriptor.GetFunction("resumeStreaming");
            _disableStream = contractDescriptor.GetFunction("disableStream");
            _resetStream = contractDescriptor.GetFunction("resetStream");
            _changeStreamSize = contractDescriptor.GetFunction("changeStreamSize");
            _collect = contractDescriptor.GetFunction("collect");
            _terminate = contractDescriptor.GetFunction("terminate");

            _setCurrentDateTime = contractDescriptor.GetFunction("setCurrentDateTime");
        }

        public Task<TransactionReceipt> ActivateStreamAsync() =>
            InvokeAsync(_activateStream, 900000);

        public Task<TransactionReceipt> PauseStreamingAsync() =>
            InvokeAsync(_pauseStreaming, 900000);

        public Task<TransactionReceipt> ResumeStreamingAsync() =>
            InvokeAsync(_resumeStreaming, 900000);

        public Task<TransactionReceipt> DisableStreamAsync() =>
            InvokeAsync(_disableStream, 900000);

        public Task<TransactionReceipt> ResetStreamAsync(string newOwnerAddress, string newReferralAddress) =>
            InvokeAsync(_resetStream, 900000, newOwnerAddress, newReferralAddress);

        public Task<TransactionReceipt> ChangeStreamSizeAsync(byte size) =>
            InvokeAsync(_changeStreamSize, 900000, size);

        public Task<TransactionReceipt> CollectAsync() =>
            InvokeAsync(_collect, 3000000);

        public Task<TransactionReceipt> TerminateAsync() =>
            InvokeAsync(_terminate, 900000);

        public Task<TransactionReceipt> SetCurrentDateTimeAsync(BigInteger value) =>
            InvokeAsync(_setCurrentDateTime, 900000, value);
    }
}
