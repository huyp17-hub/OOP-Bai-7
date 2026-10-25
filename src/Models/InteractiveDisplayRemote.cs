/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

namespace TeachingDeviceAllocation.Models
{
    /// <summary>
    /// Loại thiết bị BỔ SUNG để chứng minh khả năng mở rộng (Open/Closed):
    /// chỉ cần thêm lớp này, KHÔNG sửa InMemoryRepository, strategy hay AllocationService.
    /// - Sẵn sàng khi Available, pin &gt;= 25% và đã ghép nối (paired).
    /// - Điểm ưu tiên = mức pin, cộng 10 nếu hỗ trợ cảm ứng.
    /// </summary>
    public class InteractiveDisplayRemote : TeachingDevice
    {
        public int BatteryLevel { get; }
        public bool IsPaired { get; }
        public bool SupportsTouch { get; }

        public InteractiveDisplayRemote(string id, string name, string storageLocation,
                                        int batteryLevel, bool isPaired, bool supportsTouch)
            : base(id, name, storageLocation)
        {
            BatteryLevel = RequirePercent(batteryLevel, nameof(batteryLevel));
            IsPaired = isPaired;
            SupportsTouch = supportsTouch;
        }

        public override bool IsReadyForUse()
            => Status == DeviceStatus.Available && BatteryLevel >= 25 && IsPaired;

        public override double GetDefaultPriorityScore()
            => BatteryLevel + (SupportsTouch ? 10 : 0);
    }
}
