/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

using TeachingDeviceAllocation.Models;

namespace TeachingDeviceAllocation.Exceptions
{
    /// <summary>
    /// Lớp cơ sở cho mọi ngoại lệ nghiệp vụ của hệ thống cấp phát thiết bị.
    /// Giúp mã gọi có thể bắt chung bằng catch (DeviceAllocationException).
    /// </summary>
    public abstract class DeviceAllocationException : Exception
    {
        protected DeviceAllocationException(string message) : base(message) { }
    }

    /// <summary>Phát sinh khi thêm phần tử có Id đã tồn tại trong repository.</summary>
    public class DuplicateIdException : DeviceAllocationException
    {
        /// <summary>Mã bị trùng.</summary>
        public string DuplicateId { get; }

        public DuplicateIdException(string duplicateId)
            : base($"Đã tồn tại phần tử có mã '{duplicateId}'.")
        {
            DuplicateId = duplicateId;
        }
    }

    /// <summary>Phát sinh khi không có ứng viên nào phù hợp để chọn.</summary>
    public class NoSuitableItemException : DeviceAllocationException
    {
        public NoSuitableItemException()
            : base("Không có đối tượng phù hợp để lựa chọn.") { }

        public NoSuitableItemException(string message) : base(message) { }
    }

    /// <summary>Phát sinh khi thao tác trên thiết bị không ở trạng thái hợp lệ.</summary>
    public class DeviceNotReadyException : DeviceAllocationException
    {
        /// <summary>Mã thiết bị gây lỗi.</summary>
        public string DeviceId { get; }

        /// <summary>Trạng thái của thiết bị tại thời điểm lỗi.</summary>
        public DeviceStatus CurrentStatus { get; }

        public DeviceNotReadyException(string deviceId, DeviceStatus currentStatus, string message)
            : base(message)
        {
            DeviceId = deviceId;
            CurrentStatus = currentStatus;
        }
    }
}
