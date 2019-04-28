pragma solidity 0.5.7;

import "openzeppelin-solidity/contracts/token/ERC20/IERC20.sol";
import "openzeppelin-solidity/contracts/token/ERC20/SafeERC20.sol";

import "./Terminable.sol";

contract ERC20TokenHolder is Terminable {
    using SafeERC20 for IERC20;

    address internal _tokenAddress;

    constructor(address tokenAddress) Terminable() internal {
        _tokenAddress = tokenAddress;
    }

    function _terminate() internal {
        IERC20 token = IERC20(_tokenAddress);

        uint256 currentBalance = token.balanceOf(address(this));
        
        if(currentBalance > 0) {
            token.safeTransfer(owner(), currentBalance);
        }
        
        Terminable._terminate();
    }
}