pragma solidity 0.5.7;

import "openzeppelin-solidity/contracts/token/ERC20/IERC20.sol";
import "openzeppelin-solidity/contracts/token/ERC20/SafeERC20.sol";

import "./AlgoStreamMock.sol";

contract AlgoStream is AlgoStreamMock {

    constructor(uint8 size, uint8 gracePeriodDays, address streamHolderAddress, address referralAddress, address tokenAddress)
        AlgoStreamMock(size, gracePeriodDays, streamHolderAddress, referralAddress, tokenAddress)
        public {
    }

}
