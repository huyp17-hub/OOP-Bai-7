/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

namespace TeachingDeviceAllocation.Models
{
    /// <summary>
    /// Micro không dây.
    /// - Sẵn sàng khi Available, pin &gt;= 30% và tín hiệu &gt;= 40%.
    /// - Điểm ưu tiên = trung bình (pin + tín hiệu).
    /// </summary>
    public class WirelessMicrophone : TeachingDevice
    {
        public const int MinBatteryLevel = 30;
        public const int MinSignalQuality = 40;

        /// <summary>Mức pin (0..100).</summary>
        public int BatteryLevel { get; }

        /// <summary>Chất lượng tín hiệu (0..100).</summary>
        public int SignalQuality { get; }

        public WirelessMicrophone(string id, string name, string storageLocation,
                                  int batteryLevel, int signalQuality)
            : base(id, name, storageLocation)
        {
            BatteryLevel = RequirePercent(batteryLevel, nameof(batteryLevel));
            SignalQuality = RequirePercent(signalQuality, nameof(signalQuality));
        }

        /// <inheritdoc/>
        public override bool IsReadyForUse()
            => Status == DeviceStatus.Available
               && BatteryLevel >= MinBatteryLevel
               && SignalQuality >= MinSignalQuality;

        /// <inheritdoc/>
        public override double GetDefaultPriorityScore() => (BatteryLevel + SignalQuality) / 2.0;

        public override string ToString()
            => $"{base.ToString()} | Pin={BatteryLevel}% TínHiệu={SignalQuality}%";
    }
}
