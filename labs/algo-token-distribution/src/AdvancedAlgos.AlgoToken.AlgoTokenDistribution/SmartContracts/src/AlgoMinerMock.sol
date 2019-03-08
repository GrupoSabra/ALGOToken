pragma solidity 0.5.4;

import "openzeppelin-solidity/contracts/token/ERC20/IERC20.sol";
import "openzeppelin-solidity/contracts/token/ERC20/SafeERC20.sol";

import "./AlgoMinerCollectBased.sol";

contract AlgoMinerMock is AlgoMinerCollectBased {

    uint256 private _currentDateTime;

    constructor(MinerType minerType, uint8 category, address minerAccountAddress, address referralAccountAddress, address tokenAddress)
        AlgoMinerCollectBased(minerType, category, minerAccountAddress, referralAccountAddress, tokenAddress)
        public {
    }

    function setCurrentDateTime(uint256 value) public {
        _currentDateTime = value;    
    }

    function _getCurrentDateTime() internal view returns (uint256) {
        return _currentDateTime;
    }
}