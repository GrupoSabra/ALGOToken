using AdvancedAlgos.AlgoToken.Framework.Ethereum;
using AdvancedAlgos.AlgoToken.Framework.Ethereum.Exceptions;
using AdvancedAlgos.AlgoToken.Framework.Ethereum.IntegrationTest;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace AdvancedAlgos.AlgoToken.AlgoErc20Token.IntegrationTests
{
    public class AlgoTokenV1Tests
    {
        private const long INITIAL_SUPPLY = 1000000000;

        [Fact]
        public async Task Deploy_Contract_And_Transfer_Tokens_Test()
        {
            EthNetwork.UseDefaultTestNet();

            var ownerAccount = new Account(EthNetwork.Instance.PrefundedPrivateKey);
            var holderAccount1 = EthAccountFactory.Create();

            // Create the ERC20 token...
            var contract = new AlgoTokenV1(EthNetwork.Instance.GetWeb3(ownerAccount), EthNetwork.Instance.GasPriceProvider);
            await contract.DeployAsync();

            // Ensure that the initial supply is allocated to the owner...
            Assert.Equal(INITIAL_SUPPLY.Algo(), await contract.BalanceOfAsync(ownerAccount.Address));

            // Perform a transfer...
            await contract.TransferAsync(holderAccount1.Address, 2.Algo());

            // Ensure the receiver got the tokens...
            Assert.Equal(2.Algo(), await contract.BalanceOfAsync(holderAccount1.Address));
        }

        [Fact]
        public async Task Minter_Role_Test()
        {
            EthNetwork.UseDefaultTestNet();

            var ownerAccount = new Account(EthNetwork.Instance.PrefundedPrivateKey);
            var holderAccount1 = EthAccountFactory.Create();
            var newMinterAccount = EthAccountFactory.Create();

            await EthNetwork.Instance.RefillAsync(newMinterAccount);

            // Create the ERC20 token...
            var contract = new AlgoTokenV1(EthNetwork.Instance.GetWeb3(ownerAccount), EthNetwork.Instance.GasPriceProvider);
            await contract.DeployAsync();

            // Check the total supply...
            Assert.Equal(INITIAL_SUPPLY.Algo(), await contract.TotalSupplyAsync());

            // Mint more tokens...
            await contract.MintAsync(holderAccount1.Address, 100.Algo());

            // Check the total supply...
            Assert.Equal((INITIAL_SUPPLY + 100).Algo(), await contract.TotalSupplyAsync());

            // Check balance of the target account...
            Assert.Equal(100.Algo(), await contract.BalanceOfAsync(holderAccount1.Address));

            // Add a new minter...
            await contract.AddMinterAsync(newMinterAccount.Address);

            // Mint more tokens...
            contract = new AlgoTokenV1(contract.ContractAddress, EthNetwork.Instance.GetWeb3(newMinterAccount), EthNetwork.Instance.GasPriceProvider);
            await contract.MintAsync(holderAccount1.Address, 100.Algo());

            // Check balance of the target account...
            Assert.Equal(200.Algo(), await contract.BalanceOfAsync(holderAccount1.Address));
        }

        [Fact]
        public async Task Pausable_Feature_Test()
        {
            EthNetwork.UseDefaultTestNet();

            var ownerAccount = new Account(EthNetwork.Instance.PrefundedPrivateKey);
            var holderAccount1 = EthAccountFactory.Create();

            // Create the ERC20 token...
            var contract = new AlgoTokenV1(EthNetwork.Instance.GetWeb3(ownerAccount), EthNetwork.Instance.GasPriceProvider);
            await contract.DeployAsync();

            // Perform a transfer and check the result...
            await contract.TransferAsync(holderAccount1.Address, 2.Algo());
            Assert.Equal(2.Algo(), await contract.BalanceOfAsync(holderAccount1.Address));

            // Pause the contract...
            await contract.PauseAsync();

            // Ensure the contract cannot be used while is in paused state...
            await Assert.ThrowsAsync<TransactionRejectedException>(
                () => contract.TransferAsync(holderAccount1.Address, 2.Algo()));

            // Ensure the balance was not modified...
            Assert.Equal(2.Algo(), await contract.BalanceOfAsync(holderAccount1.Address));

            // Unpause the contract...
            await contract.UnpauseAsync();

            // Try a new transfer and ensure it succeeded...
            await contract.TransferAsync(holderAccount1.Address, 2.Algo());
            Assert.Equal(4.Algo(), await contract.BalanceOfAsync(holderAccount1.Address));
        }
    }
}
