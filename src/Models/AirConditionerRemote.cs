/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

namespace TeachingDeviceAllocation.Models
{
    /// <summary>
    /// Điều khiển điều hòa.
    /// - Sẵn sàng khi Available.
    /// - Khi chọn cho một phòng phải tương thích với phòng đó (IsCompatibleWith).
    /// - Điểm ưu tiên mặc định = 100 nếu tương thích với phòng mục tiêu, ngược lại 0.
    /// </summary>
    public class AirConditionerRemote : TeachingDevice
    {
        /// <summary>Tập phòng tương thích (chỉ đọc, không phân biệt hoa thường).</summary>
        public IReadOnlySet<string> CompatibleRooms { get; }

        /// <summary>
        /// Phòng đang cần dùng (tùy chọn). Dùng để tính điểm ưu tiên mặc định.
        /// Nếu null, điểm ưu tiên = 100 khi điều khiển dùng được cho ít nhất một phòng.
        /// </summary>
        public string? TargetRoom { get; set; }

        public AirConditionerRemote(string id, string name, string storageLocation,
                                    IEnumerable<string> compatibleRooms)
            : base(id, name, storageLocation)
        {
            if (compatibleRooms is null)
            {
                throw new ArgumentNullException(nameof(compatibleRooms));
            }

            // Sao chép sang HashSet mới để bên ngoài không sửa được tập phòng.
            var rooms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var room in compatibleRooms)
            {
                rooms.Add(RequireText(room, nameof(compatibleRooms)));
            }
            CompatibleRooms = rooms;
        }

        /// <summary>Kiểm tra điều khiển có dùng được cho phòng roomId không.</summary>
        public bool IsCompatibleWith(string roomId)
            => !string.IsNullOrWhiteSpace(roomId) && CompatibleRooms.Contains(roomId.Trim());

        /// <inheritdoc/>
        public override bool IsReadyForUse() => Status == DeviceStatus.Available;

        /// <inheritdoc/>
        public override double GetDefaultPriorityScore()
        {
            bool compatible = TargetRoom is null
                ? CompatibleRooms.Count > 0
                : IsCompatibleWith(TargetRoom);
            return compatible ? 100 : 0;
        }

        public override string ToString()
            => $"{base.ToString()} | Phòng={string.Join(",", CompatibleRooms)}";
    }
}
