pragma solidity 0.5.7;

import "openzeppelin-solidity/contracts/token/ERC20/IERC20.sol";
import "openzeppelin-solidity/contracts/token/ERC20/SafeERC20.sol";

import "./AlgoStreamImpl.sol";

contract AlgoStreamMock is AlgoStreamImpl {

    uint256 private _currentDateTime;

    constructor(uint8 size, uint8 gracePeriodDays, address streamHolderAddress, address referralAddress, address tokenAddress)
        AlgoStreamImpl(size, gracePeriodDays, streamHolderAddress, referralAddress, tokenAddress)
        public {
    }

    function setCurrentDateTime(uint256 value) public {
        _currentDateTime = value;    
    }

    function _getCurrentDateTime() internal view returns (uint256) {
        return _currentDateTime;
    }
}