pragma solidity 0.5.7;

import "openzeppelin-solidity/contracts/token/ERC20/ERC20Mintable.sol";

import "./ERC20TokenMinter.sol";

contract AlgoStreamImpl is ERC20TokenMinter {

    uint256 private constant TOKEN_FACTOR = 10 ** uint256(18);
    uint256 private constant DRIP_VALUE = 100000 * TOKEN_FACTOR;
    uint256 private constant DAYS_PER_YEAR = 365;
    uint256 private constant DAY_SECS = 86400;

    enum StreamState {
        Deactivated,
        Streaming,
        Paused,
        Disabled
    }

    uint8 private _size;
    uint8 private _gracePeriodDays;
    address private _streamHolderAddress;
    address private _referralAddress;
    StreamState private _state;
    uint256 private _collectedDays;
    uint256 private _lastCollectionDay;
    uint256 private _lastStateChange;

    constructor(uint8 size, uint8 gracePeriodDays, address streamHolderAddress, address referralAddress, address tokenAddress)
        ERC20TokenMinter(tokenAddress)
        public {
        
        require(size >= 1 && size <= 50);
        require(gracePeriodDays >= 0 && gracePeriodDays <= 10);
        require(streamHolderAddress != address(0));
        require(referralAddress != address(0));

        _size = size;
        _gracePeriodDays = gracePeriodDays;
        _streamHolderAddress = streamHolderAddress;
        _referralAddress = referralAddress;

        _state = StreamState.Deactivated;
        _collectedDays = 0;
        _lastStateChange = _getCurrentDateTime();
    }

    modifier onlyOwnerOrStreamHolder() {
        require(msg.sender == owner() || msg.sender == _streamHolderAddress);
        _;
    }
    
    modifier atState(StreamState state) {
        require(_state == state);
        _;
    }

    modifier ensureGracePeriod() {
        require((_getCurrentDateTime() - _lastStateChange) >= (_gracePeriodDays * DAY_SECS));
        _;
    }
    
    function activateStream() public notTerminated onlyOwner atState(StreamState.Deactivated) {
        _state = StreamState.Streaming;
        _lastCollectionDay = _getCurrentEpochDay();
        _lastStateChange = _getCurrentDateTime();
    }

    function pauseStreaming() public notTerminated onlyOwner atState(StreamState.Streaming) ensureGracePeriod {
        _tryCollect();

        _state = StreamState.Paused;
    }

    function resumeStreaming() public notTerminated onlyOwner atState(StreamState.Paused) {
        _state = StreamState.Streaming;
        _lastCollectionDay = _getCurrentEpochDay();
        _lastStateChange = _getCurrentDateTime();
    }

    function disableStream() public notTerminated onlyOwner ensureGracePeriod {
        require(_state != StreamState.Disabled);

        _tryCollect();

        _state = StreamState.Disabled;
        _streamHolderAddress = address(0);
        _referralAddress = address(0);
        _lastStateChange = _getCurrentDateTime();
    }

    function resetStream(address newStreamHolderAddress, address newReferralAddress) public
        notTerminated onlyOwner atState(StreamState.Disabled) {

        _state = StreamState.Deactivated;
        _streamHolderAddress = newStreamHolderAddress;
        _referralAddress = newReferralAddress;
        _lastStateChange = _getCurrentDateTime();
    }

    function changeStreamSize(uint8 size) public notTerminated onlyOwner atState(StreamState.Paused) ensureGracePeriod {
        require(size >= 1 && size <= 50);
        _size = size;
    }

    function collect() public notTerminated onlyOwnerOrStreamHolder {
        _tryCollect();
    }

    function terminate() public onlyOwner {
        _terminate();
    }

    function getSize() public view returns (uint8) {
        return _size;
    }

    function getGracePeriodDays() public view returns (uint8) {
        return _gracePeriodDays;
    }

    function getStreamHolderAddress() public view returns (address) {
        return _streamHolderAddress;
    }

    function getReferralAddress() public view returns (address) {
        return _referralAddress;
    }

    function getState() public view returns (StreamState) {
        return _state;
    }

    function getCollectedDays() public view returns (uint256) {
        return _collectedDays;
    }

    function getLastCollectionDay() public view returns (uint256) {
        return _lastCollectionDay;
    }

    function getCurrentIterationStreamingDays() public view returns (uint256) {
        return _getCurrentEpochDay() - _lastCollectionDay;
    }

    function _getCurrentDateTime() internal view returns (uint256) {
        return now;
    }

    function _tryCollect() private {

        if(_state != StreamState.Streaming) return;

        uint256 currentDay = _getCurrentEpochDay();

        if(currentDay == _lastCollectionDay) return;

        uint256 currentIterationStreamingDays = getCurrentIterationStreamingDays();

        uint256 holderFees = 0;

        for (uint256 day = 0; day < currentIterationStreamingDays; day++) {
            // NOTE: In Solidity, division rounds towards zero.
            uint256 year = (_collectedDays + day) / DAYS_PER_YEAR;
            uint256 yearSupply = DRIP_VALUE * _size * 12 / 10 / (year + 1);
            uint256 daySupply = yearSupply / DAYS_PER_YEAR;
            holderFees += daySupply;
        }

        _collectedDays += currentIterationStreamingDays;

        uint256 referralFees = holderFees * 10 / 100;

        require(holderFees > 0);
        require(referralFees > 0);

        ERC20Mintable token = ERC20Mintable(_tokenAddress);

        token.mint(_streamHolderAddress, holderFees);
        token.mint(_referralAddress, referralFees);

        _lastCollectionDay = _getCurrentEpochDay();
    }

    function _getCurrentEpochDay() private view returns (uint256) {
        return _getCurrentDateTime() / DAY_SECS; // NOTE: In Solidity, division rounds towards zero.
    }
}
