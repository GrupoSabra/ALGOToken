pragma solidity 0.5.7;

import "openzeppelin-solidity/contracts/ownership/Ownable.sol";

contract Terminable is Ownable {

    bool internal _terminated;

    constructor() internal {
        _terminated = false;
    }

    modifier notTerminated() {
        require(!_terminated);
        _;
    }

    function isTerminated() public view returns (bool) {
        return _terminated;
    }

    function _terminate() internal {
        _terminated = true;
    }
}