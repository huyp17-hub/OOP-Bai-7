/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

using TeachingDeviceAllocation.Models;

namespace TeachingDeviceAllocation.Repositories
{
    /// <summary>
    /// Kho lưu trữ tổng quát.
    /// Tham số kiểu T: loại đối tượng được lưu (ProjectorRemote, WirelessMicrophone, ...).
    /// Ràng buộc "where T : IIdentifiable" bảo đảm mọi T đều có thuộc tính Id,
    /// nhờ đó kho có thể kiểm tra trùng mã và tìm theo mã mà không cần biết lớp cụ thể.
    /// </summary>
    public interface IRepository<T> where T : IIdentifiable
    {
        /// <summary>Thêm phần tử. Lỗi nếu null hoặc trùng Id.</summary>
        void Add(T item);

        /// <summary>Xóa theo mã; trả về false nếu không tìm thấy.</summary>
        bool Remove(string id);

        /// <summary>Tìm theo mã; trả về null (default) nếu không có.</summary>
        T? FindById(string id);

        /// <summary>Lấy toàn bộ phần tử dưới dạng danh sách chỉ đọc (bản sao).</summary>
        IReadOnlyList<T> GetAll();

        /// <summary>Lọc phần tử theo điều kiện truyền từ bên ngoài.</summary>
        IReadOnlyList<T> FindAll(Predicate<T> condition);
    }
}
