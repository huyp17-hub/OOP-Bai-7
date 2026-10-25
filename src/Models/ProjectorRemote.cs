/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

namespace TeachingDeviceAllocation.Models
{
    /// <summary>
    /// Điều khiển máy chiếu.
    /// - Sẵn sàng khi Available và pin &gt;= 20%.
    /// - Điểm ưu tiên = mức pin.
    /// </summary>
    public class ProjectorRemote : TeachingDevice
    {
        /// <summary>Ngưỡng pin tối thiểu để cấp phát (%).</summary>
        public const int MinBatteryLevel = 20;

        /// <summary>Mức pin hiện tại (0..100).</summary>
        public int BatteryLevel { get; }

        public ProjectorRemote(string id, string name, string storageLocation, int batteryLevel)
            : base(id, name, storageLocation)
        {
            BatteryLevel = RequirePercent(batteryLevel, nameof(batteryLevel));
        }

        /// <inheritdoc/>
        public override bool IsReadyForUse()
            => Status == DeviceStatus.Available && BatteryLevel >= MinBatteryLevel;

        /// <inheritdoc/>
        public override double GetDefaultPriorityScore() => BatteryLevel;

        public override string ToString() => $"{base.ToString()} | Pin={BatteryLevel}%";
    }
}
