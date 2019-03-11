pragma solidity 0.5.4;

import "openzeppelin-solidity/contracts/token/ERC20/IERC20.sol";
import "openzeppelin-solidity/contracts/token/ERC20/SafeERC20.sol";

import "./IAlgoMiner.sol";
import "./AlgoCommon.sol";
import "./ERC20TokenHolder.sol";
import "./AlgoSupervisorRole.sol";
import "./AlgoSystemRole.sol";
import "./AlgoCoreTeamRole.sol";

contract AlgoMinerCollectBased is AlgoCommon,
                                ERC20TokenHolder,
                                AlgoSystemRole,
                                AlgoCoreTeamRole,
                                AlgoSupervisorRole,
                                IAlgoMiner {
    using SafeERC20 for IERC20;

    uint256 private constant DAY_SECS = 86400;
    uint256 private constant FEE_LAST_AMOUNT_INIT = 1902694;
    uint256 private constant FEE_TRANSIT_COEF = 998102770;
    uint256 private constant FEE_FACTOR = 1000000000;

    enum MinerType {
        PoolBased,
        NonPoolBased
    }

    enum MinerState {
        Deactivated,
        Activated,
        Suspended,
        Stopped
    }

    MinerType private _minerType;
    uint8 private _category;
    address private _miner;
    address private _referral;

    MinerState private _state;
    bool private _mining;
    uint256 private _lastAmountTaken;
    uint256 private _lastCollectionDay;
    uint256 private _totalSupply;
    uint256 private _minedDays;

    constructor(MinerType minerType, uint8 category, address minerAccountAddress, address referralAccountAddress, address tokenAddress)
        ERC20TokenHolder(tokenAddress)
        AlgoSystemRole()
        AlgoCoreTeamRole()
        AlgoSupervisorRole()
        public {
        
        require(category <= 5);
        require(minerAccountAddress != address(0));

        if(minerType == MinerType.PoolBased) {
            require(referralAccountAddress != address(0));
        }

        _minerType = minerType;
        _category = category;
        _miner = minerAccountAddress;
        _referral = referralAccountAddress;

        _lastAmountTaken = FEE_LAST_AMOUNT_INIT;
    }

    modifier onlyMiner() {
        require(msg.sender == _miner);
        _;
    }

    modifier onlyMinerOrSystem() {
        require(msg.sender == _miner || isSystem(msg.sender));
        _;
    }
    
    function activateMiner() public notTerminated onlyCoreTeam {
        require(_state == MinerState.Deactivated);

        if(_minerType == MinerType.PoolBased && _totalSupply == 0) {

            uint256 capacity = getCapacityByCategory(_category);
            uint256 expectedBalance = capacity + capacity * 10 / 100;

            uint256 currentBalance = _token.balanceOf(address(this));

            require(currentBalance == expectedBalance);

            _totalSupply = capacity / 2;
        }

        _state = MinerState.Activated;
    }

    function deactivateMiner() public notTerminated onlyCoreTeam {
        require(_state != MinerState.Deactivated);

        _tryCollect();
        
        _state = MinerState.Deactivated;
        _mining = false;
    }

    function migrateMiner(address newMinerAddress) public onlyCoreTeam {
        require(_state == MinerState.Deactivated);

        _token.safeTransfer(newMinerAddress, _token.balanceOf(address(this)));
    }

    function pauseMining() public notTerminated onlySupervisor {
        require(_state == MinerState.Activated);

        _tryCollect();

        _state = MinerState.Suspended;
    }

    function resumeMining() public notTerminated onlySupervisor {
        require(_state == MinerState.Suspended);

        _state = MinerState.Activated;

        _startMining();
    }

    function stopAndRemoveOwnership() public notTerminated onlySupervisor {
        require(_state != MinerState.Stopped);

        _tryCollect();
        
        _state = MinerState.Stopped;
        _mining = false;
        _miner = address(0);
        _referral = address(0);
    }

    function resetMiner(address newOwnerAddress, address newReferralAddress) public notTerminated onlySupervisor {
        require(_state == MinerState.Stopped);

        _state = MinerState.Activated;
        _miner = newOwnerAddress;
        _referral = newReferralAddress;
    }

    function startMining() public notTerminated onlyMiner {
        require(_state == MinerState.Activated);
        require(!_mining);
        
        _mining = true;

        _startMining();
    }

    function stopMining() public notTerminated onlyMiner {
        require(_state == MinerState.Activated);
        require(_mining);
        
        _tryCollect();

        _mining = false;
    }

    function collect() public notTerminated onlyMinerOrSystem {
        require(_minerType == MinerType.PoolBased);
        require(_state == MinerState.Activated);
        // require(_mining); TO DEFINE: should we reject or just treat like a NOP?

        _tryCollect();
    }

    function terminate() public onlyCoreTeam {
        _terminate();
    }

    function isAlgoMiner() public pure returns (bool) {
        return true;
    }

    function getMinerType() public view returns (uint8) {
        return uint8(_minerType);
    }

    function getCategory() public view returns (uint8) {
        return _category;
    }

    function getMiner() public view returns (address) {
        return _miner;
    }

    function getReferral() public view returns (address) {
        return _referral;
    }

    function isMining() public view returns (bool) {
        return _state == MinerState.Activated && _mining;
    }

    function getTotalSupply() public view returns (uint256) {
        return _totalSupply;
    }

    function getLastCollectionDay() public view returns (uint256) {
        return _lastCollectionDay;
    }

    function getCurrentIterationMinedDays() public view returns (uint256) {
        return _getCurrentEpochDay() - _lastCollectionDay;
    }

    function _getCurrentDateTime() internal view returns (uint256) {
        return now;
    }

    function _tryCollect() private {
        if(_minerType != MinerType.PoolBased || _state != MinerState.Activated || !_mining) return;

        uint256 currentDay = _getCurrentEpochDay();

        if(currentDay == _lastCollectionDay) return;

        uint256 minerFees = 0;
        uint256 currentAmountTaken = _lastAmountTaken;
        uint256 currentIterationMinedDays = currentDay - _lastCollectionDay;

        for (uint256 day = 0; day < currentIterationMinedDays; day++) {
            // NOTE: In Solidity, division rounds towards zero.
            currentAmountTaken = currentAmountTaken * FEE_TRANSIT_COEF / FEE_FACTOR;
            minerFees += currentAmountTaken * _totalSupply / FEE_FACTOR;
        }

        _lastAmountTaken = currentAmountTaken;
        _minedDays += currentIterationMinedDays;

        uint256 referralFees = minerFees * 10 / 100;

        require(minerFees > 0);
        require(referralFees > 0);

        _token.safeTransfer(_miner, minerFees);
        _token.safeTransfer(_referral, referralFees);

        _startMining();
    }

    function _startMining() private {
        _lastCollectionDay = _getCurrentEpochDay();
    }

    function _getCurrentEpochDay() private view returns (uint256) {
        return _getCurrentDateTime() / DAY_SECS; // NOTE: In Solidity, division rounds towards zero.
    }
}
