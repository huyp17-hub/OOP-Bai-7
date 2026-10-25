/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

namespace TeachingDeviceAllocation.Models
{
    /// <summary>
    /// Trạng thái của một thiết bị dạy học.
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>Thiết bị đang ở kho, có thể cấp phát.</summary>
        Available,

        /// <summary>Thiết bị đang được mượn.</summary>
        Borrowed,

        /// <summary>Thiết bị đang bảo trì, không được cấp phát.</summary>
        UnderMaintenance
    }
}
