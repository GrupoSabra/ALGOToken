using System;
using System.Numerics;
using System.Threading.Tasks;
using AdvancedAlgos.AlgoToken.AlgoErc20Token;
using AdvancedAlgos.AlgoToken.AlgoTokenDistribution.Algorithms;
using AdvancedAlgos.AlgoToken.Framework.Ethereum;
using AdvancedAlgos.AlgoToken.Framework.Ethereum.Exceptions;
using AdvancedAlgos.AlgoToken.Framework.Ethereum.IntegrationTest;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Xunit;

namespace AdvancedAlgos.AlgoToken.AlgoTokenDistribution.IntegrationTests
{
    public class AlgoStreamTests
    {
        [Fact]
        public async Task Stream_Basic_Workflow_Test()
        {
            EthNetwork.UseDefaultTestNet();

            var prefundedAccount = new Account(EthNetwork.Instance.PrefundedPrivateKey);

            var tokenOwnerAccount = EthAccountFactory.Create();
            var streamOwnerAccount = EthAccountFactory.Create();
            var streamHolderAccount = EthAccountFactory.Create();
            var referralAccount = EthAccountFactory.Create();

            BigInteger expectedHolderBalance;
            BigInteger expectedReferralBalance;

            await EthNetwork.Instance.RefillAsync(tokenOwnerAccount);
            await EthNetwork.Instance.RefillAsync(streamOwnerAccount);
            await EthNetwork.Instance.RefillAsync(streamHolderAccount);

            // Create the ERC20 token...
            var token = new AlgoTokenV1(EthNetwork.Instance.GetWeb3(tokenOwnerAccount), EthNetwork.Instance.GasPriceProvider);
            await token.DeployAsync();

            // Create the stream...
            var stream = new AlgoStream(EthNetwork.Instance.GetWeb3(streamOwnerAccount), EthNetwork.Instance.GasPriceProvider);
            await stream.DeployAsync(1, 2, streamHolderAccount.Address, referralAccount.Address, token.ContractAddress);

            // Add the stream as minter...
            await token.AddMinterAsync(stream.ContractAddress);

            // Setup the clock...
            var clock = await Clock.FromAsync(2019, 3, 1, 15, 30, x => stream.SetCurrentDateTimeAsync(x));

            // Activate the stream...
            await stream.ActivateStreamAsync();

            // Collect the funds...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamHolderAccount));
            await clock.AddDaysAsync(5);
            await stream.CollectAsync();

            expectedHolderBalance = AlgoStreamFeeAlgorithm.CalculateFees(size: 1, days: 5);
            expectedReferralBalance = expectedHolderBalance * 10 / 100;
            Assert.Equal(expectedHolderBalance, await token.BalanceOfAsync(streamHolderAccount.Address));
            Assert.Equal(expectedReferralBalance, await token.BalanceOfAsync(referralAccount.Address));

            // Pause the stream...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamOwnerAccount));
            await stream.PauseStreamingAsync();

            // Ensure that the skipped days are not taking into account...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamHolderAccount));
            await clock.AddDaysAsync(3);
            await stream.CollectAsync();

            // Ensure the balance is not changed...
            Assert.Equal(expectedHolderBalance, await token.BalanceOfAsync(streamHolderAccount.Address));

            // Resume the stream...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamOwnerAccount));
            await stream.ResumeStreamingAsync();

            // Collect the funds...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamHolderAccount));
            await clock.AddDaysAsync(2);
            await stream.CollectAsync();

            expectedHolderBalance = AlgoStreamFeeAlgorithm.CalculateFees(size: 1, days: 7);
            expectedReferralBalance = expectedHolderBalance * 10 / 100;
            Assert.Equal(expectedHolderBalance, await token.BalanceOfAsync(streamHolderAccount.Address));
            Assert.Equal(expectedReferralBalance, await token.BalanceOfAsync(referralAccount.Address));
        }

        [Fact]
        public async Task Stream_Extended_Workflow_Test()
        {
            EthNetwork.UseDefaultTestNet();

            var prefundedAccount = new Account(EthNetwork.Instance.PrefundedPrivateKey);

            var tokenOwnerAccount = EthAccountFactory.Create();
            var streamOwnerAccount = EthAccountFactory.Create();
            var streamHolderAccount = EthAccountFactory.Create();
            var referralAccount = EthAccountFactory.Create();

            BigInteger expectedHolderBalance;
            BigInteger expectedReferralBalance;

            await EthNetwork.Instance.RefillAsync(tokenOwnerAccount);
            await EthNetwork.Instance.RefillAsync(streamOwnerAccount);
            await EthNetwork.Instance.RefillAsync(streamHolderAccount);

            // Create the ERC20 token...
            var token = new AlgoTokenV1(EthNetwork.Instance.GetWeb3(tokenOwnerAccount), EthNetwork.Instance.GasPriceProvider);
            await token.DeployAsync();

            // Create the stream...
            var stream = new AlgoStream(EthNetwork.Instance.GetWeb3(streamOwnerAccount), EthNetwork.Instance.GasPriceProvider);
            await stream.DeployAsync(1, 2, streamHolderAccount.Address, referralAccount.Address, token.ContractAddress);

            // Add the stream as minter...
            await token.AddMinterAsync(stream.ContractAddress);

            // Setup the clock...
            var clock = await Clock.FromAsync(2019, 3, 1, 15, 30, x => stream.SetCurrentDateTimeAsync(x));

            // Activate the stream...
            await stream.ActivateStreamAsync();

            // Collect the funds...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamHolderAccount));
            await clock.AddDaysAsync(15);
            await stream.CollectAsync();

            expectedHolderBalance = AlgoStreamFeeAlgorithm.CalculateFees(size: 1, days: 15);
            expectedReferralBalance = expectedHolderBalance * 10 / 100;
            Assert.Equal(expectedHolderBalance, await token.BalanceOfAsync(streamHolderAccount.Address));
            Assert.Equal(expectedReferralBalance, await token.BalanceOfAsync(referralAccount.Address));

            // Collect the funds...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamHolderAccount));
            await clock.AddDaysAsync(15);
            await stream.CollectAsync();

            expectedHolderBalance = AlgoStreamFeeAlgorithm.CalculateFees(size: 1, days: 30);
            expectedReferralBalance = expectedHolderBalance * 10 / 100;
            Assert.Equal(expectedHolderBalance, await token.BalanceOfAsync(streamHolderAccount.Address));
            Assert.Equal(expectedReferralBalance, await token.BalanceOfAsync(referralAccount.Address));

            // Collect the funds...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamHolderAccount));
            await clock.AddDaysAsync(100);
            await stream.CollectAsync();

            expectedHolderBalance = AlgoStreamFeeAlgorithm.CalculateFees(size: 1, days: 130);
            expectedReferralBalance = expectedHolderBalance * 10 / 100;
            Assert.Equal(expectedHolderBalance, await token.BalanceOfAsync(streamHolderAccount.Address));
            Assert.Equal(expectedReferralBalance, await token.BalanceOfAsync(referralAccount.Address));

            // Collect the funds...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamHolderAccount));
            await clock.AddDaysAsync(400);
            await stream.CollectAsync();

            expectedHolderBalance = AlgoStreamFeeAlgorithm.CalculateFees(size: 1, days: 530);
            expectedReferralBalance = expectedHolderBalance * 10 / 100;
            Assert.Equal(expectedHolderBalance, await token.BalanceOfAsync(streamHolderAccount.Address));
            Assert.Equal(expectedReferralBalance, await token.BalanceOfAsync(referralAccount.Address));

            // Collect the funds...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamHolderAccount));
            await clock.AddDaysAsync(3170);
            await stream.CollectAsync();

            expectedHolderBalance = AlgoStreamFeeAlgorithm.CalculateFees(size: 1, days: 3700);
            expectedReferralBalance = expectedHolderBalance * 10 / 100;
            Assert.Equal(expectedHolderBalance, await token.BalanceOfAsync(streamHolderAccount.Address));
            Assert.Equal(expectedReferralBalance, await token.BalanceOfAsync(referralAccount.Address));
        }

        [Fact]
        public async Task Stream_Terminate_Test()
        {
            EthNetwork.UseDefaultTestNet();

            var prefundedAccount = new Account(EthNetwork.Instance.PrefundedPrivateKey);

            var tokenOwnerAccount = EthAccountFactory.Create();
            var streamOwnerAccount = EthAccountFactory.Create();
            var streamHolderAccount = EthAccountFactory.Create();
            var referralAccount = EthAccountFactory.Create();

            await EthNetwork.Instance.RefillAsync(tokenOwnerAccount);
            await EthNetwork.Instance.RefillAsync(streamOwnerAccount);
            await EthNetwork.Instance.RefillAsync(streamHolderAccount);

            // Create the ERC20 token...
            var token = new AlgoTokenV1(EthNetwork.Instance.GetWeb3(tokenOwnerAccount), EthNetwork.Instance.GasPriceProvider);
            await token.DeployAsync();

            // Store the current balance of the token owner...
            var tokenOwnerAccountBalance = await token.BalanceOfAsync(tokenOwnerAccount.Address);

            // Create the stream...
            var stream = new AlgoStream(EthNetwork.Instance.GetWeb3(streamOwnerAccount), EthNetwork.Instance.GasPriceProvider);
            await stream.DeployAsync(1, 2, streamHolderAccount.Address, referralAccount.Address, token.ContractAddress);

            // Add the stream as minter...
            await token.AddMinterAsync(stream.ContractAddress);

            // Setup the clock...
            var clock = await Clock.FromAsync(2019, 3, 1, 15, 30, x => stream.SetCurrentDateTimeAsync(x));

            // Activate the stream...
            await stream.ActivateStreamAsync();

            // Collect the funds...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamHolderAccount));
            await clock.AddDaysAsync(15);
            await stream.CollectAsync();

            // Transfer some tokens to the stream...
            await token.TransferAsync(stream.ContractAddress, 100.Algo());

            // Ensure the stream got the tokens...
            Assert.Equal(100.Algo(), await token.BalanceOfAsync(stream.ContractAddress));

            // Terminate the stream...
            stream.Bind(EthNetwork.Instance.GetWeb3(streamOwnerAccount));
            await stream.TerminateAsync();

            // Ensure the stream returned all the tokens...
            Assert.Equal(0, await token.BalanceOfAsync(stream.ContractAddress));
            Assert.Equal(tokenOwnerAccountBalance - 100.Algo(), await token.BalanceOfAsync(tokenOwnerAccount.Address));

            // Ensure the stream is not a minter anymore...
            Assert.False(await token.IsMinterAsync(stream.ContractAddress));
        }
    }
}
