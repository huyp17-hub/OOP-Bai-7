/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

using TeachingDeviceAllocation.Exceptions;

namespace TeachingDeviceAllocation.Models
{
    /// <summary>
    /// Lớp trừu tượng cơ sở cho mọi thiết bị dạy học.
    /// - Phần CHUNG (mã, tên, vị trí, trạng thái, mượn/trả) nằm ở lớp này.
    /// - Phần THAY ĐỔI theo loại thiết bị (kiểm tra sẵn sàng, tính điểm ưu tiên)
    ///   được khai báo abstract và cài đặt đa hình ở lớp dẫn xuất.
    /// </summary>
    public abstract class TeachingDevice : IIdentifiable
    {
        /// <summary>Mã thiết bị (duy nhất).</summary>
        public string Id { get; }

        /// <summary>Tên thiết bị.</summary>
        public string Name { get; }

        /// <summary>Vị trí lưu trữ trong kho.</summary>
        public string StorageLocation { get; }

        /// <summary>Trạng thái hiện tại; chỉ lớp này và lớp con được thay đổi.</summary>
        public DeviceStatus Status { get; protected set; }

        /// <summary>
        /// Khởi tạo thiết bị. Id, Name, StorageLocation không được rỗng.
        /// Trạng thái ban đầu luôn là Available.
        /// </summary>
        /// <exception cref="ArgumentException">Khi tham số rỗng hoặc chỉ có khoảng trắng.</exception>
        protected TeachingDevice(string id, string name, string storageLocation)
        {
            Id = RequireText(id, nameof(id));
            Name = RequireText(name, nameof(name));
            StorageLocation = RequireText(storageLocation, nameof(storageLocation));
            Status = DeviceStatus.Available;
        }

        /// <summary>
        /// Kiểm tra thiết bị có sẵn sàng cấp phát hay không.
        /// Mỗi loại thiết bị có quy tắc riêng (đa hình).
        /// </summary>
        public abstract bool IsReadyForUse();

        /// <summary>
        /// Điểm ưu tiên mặc định khi chọn thiết bị (càng cao càng được ưu tiên).
        /// Mỗi loại thiết bị có cách tính riêng (đa hình).
        /// </summary>
        public abstract double GetDefaultPriorityScore();

        /// <summary>
        /// Cho mượn thiết bị: chỉ thành công khi IsReadyForUse() trả về true.
        /// Lời gọi IsReadyForUse() ở đây là đa hình — lớp cơ sở không cần biết loại cụ thể.
        /// </summary>
        /// <exception cref="DeviceNotReadyException">Khi thiết bị chưa sẵn sàng.</exception>
        public void Borrow()
        {
            if (!IsReadyForUse())
            {
                throw new DeviceNotReadyException(Id, Status,
                    $"Thiết bị '{Id}' chưa sẵn sàng để cấp phát (trạng thái: {Status}).");
            }
            Status = DeviceStatus.Borrowed;
        }

        /// <summary>
        /// Trả thiết bị về kho: chỉ thành công khi trạng thái là Borrowed.
        /// </summary>
        /// <exception cref="DeviceNotReadyException">Khi thiết bị không ở trạng thái Borrowed.</exception>
        public void Return()
        {
            if (Status != DeviceStatus.Borrowed)
            {
                throw new DeviceNotReadyException(Id, Status,
                    $"Không thể trả thiết bị '{Id}' vì nó không ở trạng thái Borrowed (hiện tại: {Status}).");
            }
            Status = DeviceStatus.Available;
        }

        /// <summary>Đưa thiết bị vào bảo trì (chỉ khi đang ở kho).</summary>
        public void SendToMaintenance()
        {
            if (Status == DeviceStatus.Borrowed)
            {
                throw new DeviceNotReadyException(Id, Status,
                    $"Thiết bị '{Id}' đang được mượn, không thể đưa vào bảo trì.");
            }
            Status = DeviceStatus.UnderMaintenance;
        }

        /// <summary>Kết thúc bảo trì, thiết bị trở lại trạng thái Available.</summary>
        public void FinishMaintenance()
        {
            if (Status == DeviceStatus.UnderMaintenance)
            {
                Status = DeviceStatus.Available;
            }
        }

        /// <summary>Chuỗi mô tả ngắn gọn thiết bị (lớp con có thể bổ sung thông tin).</summary>
        public override string ToString()
            => $"{GetType().Name}[{Id}] \"{Name}\" @{StorageLocation} | {Status}";

        // ===== Hàm hỗ trợ kiểm tra dữ liệu dùng chung cho lớp con =====

        /// <summary>Bảo đảm chuỗi không rỗng; trả về chuỗi đã cắt khoảng trắng.</summary>
        protected static string RequireText(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} không được rỗng.", paramName);
            }
            return value.Trim();
        }

        /// <summary>Bảo đảm giá trị phần trăm nằm trong [0, 100].</summary>
        protected static int RequirePercent(int value, string paramName)
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentOutOfRangeException(paramName, value, $"{paramName} phải trong khoảng 0..100.");
            }
            return value;
        }
    }
}
