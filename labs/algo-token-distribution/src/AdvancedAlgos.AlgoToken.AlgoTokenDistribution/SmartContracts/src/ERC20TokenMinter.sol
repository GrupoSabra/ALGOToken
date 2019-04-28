pragma solidity 0.5.7;

import "openzeppelin-solidity/contracts/token/ERC20/ERC20Mintable.sol";

import "./ERC20TokenHolder.sol";

contract ERC20TokenMinter is ERC20TokenHolder {

    constructor(address tokenAddress) ERC20TokenHolder(tokenAddress) internal { }

    function _terminate() internal {
        ERC20Mintable token = ERC20Mintable(_tokenAddress);

        token.renounceMinter();
        
        ERC20TokenHolder._terminate();
    }
}