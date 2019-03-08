pragma solidity 0.5.4;

import "openzeppelin-solidity/contracts/token/ERC20/IERC20.sol";
import "openzeppelin-solidity/contracts/token/ERC20/SafeERC20.sol";

import "./AlgoMinerMock.sol";

contract AlgoMiner is AlgoMinerMock {

    constructor(MinerType minerType, uint8 category, address minerAccountAddress, address referralAccountAddress, address tokenAddress)
        AlgoMinerMock(minerType, category, minerAccountAddress, referralAccountAddress, tokenAddress)
        public {
    }

}
