using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedAlgos.AlgoToken.Framework.Ethereum.IntegrationTest
{
    public class Clock
    {
        private BigInteger _currentEpochTime;

        private Clock() { }

        public Func<BigInteger, Task> OnClockChanged { get; private set; }

        public static Task<Clock> FromAsync(int year, int month, int day, Func<BigInteger, Task> onClockChanged)
            => FromAsync(year, month, day, 0, 0, 0, onClockChanged);

        public static Task<Clock> FromAsync(int year, int month, int day, int hour, int minute, Func<BigInteger, Task> onClockChanged)
            => FromAsync(year, month, day, hour, minute, 0, onClockChanged);

        public static async Task<Clock> FromAsync(int year, int month, int day, int hour, int minute, int second, Func<BigInteger, Task> onClockChanged)
        {
            var clock = new Clock
            {
                _currentEpochTime = EpochFrom(year, month, day, hour, minute, second),
                OnClockChanged = onClockChanged
            };

            await clock.UpdateAsync();

            return clock;
        }

        private static BigInteger EpochFrom(int year, int month, int day, int hour, int minute, int second) =>
            EpochFrom(new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc));

        private static BigInteger EpochFrom(DateTime dateTime) =>
            new BigInteger((dateTime - DateTime.UnixEpoch).TotalSeconds);

        public DateTime Current => DateTime.UnixEpoch.AddSeconds((double)_currentEpochTime);

        public Task AddAsync(TimeSpan value)
        {
            _currentEpochTime += (BigInteger)value.TotalSeconds;

            return UpdateAsync();
        }

        public Task AddDaysAsync(int value) => AddAsync(TimeSpan.FromDays(value));

        private Task UpdateAsync()
        {
            return OnClockChanged(_currentEpochTime);
        }
    }
}
