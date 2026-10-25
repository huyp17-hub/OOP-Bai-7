/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

namespace TeachingDeviceAllocation.Models
{
    /// <summary>
    /// Đối tượng có mã định danh duy nhất.
    /// Cho phép các cấu trúc generic (như repository) tìm đối tượng theo mã
    /// mà không cần biết lớp cụ thể của đối tượng.
    /// </summary>
    public interface IIdentifiable
    {
        /// <summary>Mã định danh duy nhất của đối tượng.</summary>
        string Id { get; }
    }
}
