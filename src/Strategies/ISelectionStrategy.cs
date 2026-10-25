/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

namespace TeachingDeviceAllocation.Strategies
{
    /// <summary>
    /// Chiến lược chọn một phần tử từ danh sách ứng viên (Strategy pattern).
    /// Mỗi lớp cài đặt là một thuật toán chọn khác nhau; dịch vụ gọi Select() một cách đa hình.
    /// Tham số kiểu T: loại đối tượng ứng viên.
    /// </summary>
    public interface ISelectionStrategy<T>
    {
        /// <summary>
        /// Chọn một phần tử. Không được sửa danh sách đầu vào.
        /// </summary>
        /// <exception cref="Exceptions.NoSuitableItemException">Khi danh sách rỗng.</exception>
        T Select(IReadOnlyList<T> candidates);

        /// <summary>Tên hiển thị của chiến lược (phục vụ log/kiểm thử).</summary>
        string Name => GetType().Name.Split('`')[0];
    }
}
