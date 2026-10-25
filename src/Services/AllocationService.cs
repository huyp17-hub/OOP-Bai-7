/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

using TeachingDeviceAllocation.Exceptions;
using TeachingDeviceAllocation.Models;
using TeachingDeviceAllocation.Repositories;
using TeachingDeviceAllocation.Strategies;

namespace TeachingDeviceAllocation.Services
{
    /// <summary>
    /// Dịch vụ cấp phát thiết bị tổng quát.
    /// - Phụ thuộc vào abstraction IRepository&lt;T&gt; và ISelectionStrategy&lt;T&gt;.
    /// - Ràng buộc "where T : TeachingDevice" cho phép gọi IsReadyForUse() và Borrow()
    ///   mà KHÔNG cần if/else kiểm tra loại cụ thể. Thêm loại thiết bị mới không phải sửa lớp này.
    /// </summary>
    public class AllocationService<T> where T : TeachingDevice
    {
        private readonly IRepository<T> repository;
        private ISelectionStrategy<T> strategy;

        /// <summary>Chiến lược đang dùng (chỉ đọc từ bên ngoài).</summary>
        public ISelectionStrategy<T> CurrentStrategy => strategy;

        public AllocationService(IRepository<T> repository, ISelectionStrategy<T> strategy)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        /// <summary>Đổi chiến lược lựa chọn tại thời gian chạy.</summary>
        public void ChangeStrategy(ISelectionStrategy<T> newStrategy)
        {
            strategy = newStrategy ?? throw new ArgumentNullException(nameof(newStrategy));
        }

        /// <summary>
        /// Lấy danh sách ứng viên: thiết bị sẵn sàng (đa hình) và thỏa điều kiện bổ sung.
        /// </summary>
        public IReadOnlyList<T> GetCandidates(Predicate<T>? additionalCondition = null)
            => repository.FindAll(d => d.IsReadyForUse()
                                       && (additionalCondition is null || additionalCondition(d)));

        /// <summary>
        /// Cấp phát một thiết bị:
        /// 1) lấy từ repository, 2) giữ thiết bị IsReadyForUse(), 3) áp dụng điều kiện bổ sung,
        /// 4) chọn bằng chiến lược hiện tại, 5) gọi Borrow(), 6) trả về thiết bị.
        /// </summary>
        /// <exception cref="NoSuitableItemException">Khi không có ứng viên phù hợp.</exception>
        public T Allocate(Predicate<T>? additionalCondition)
        {
            IReadOnlyList<T> candidates = GetCandidates(additionalCondition);
            if (candidates.Count == 0)
            {
                throw new NoSuitableItemException(
                    $"Không có thiết bị {typeof(T).Name} nào sẵn sàng và thỏa điều kiện.");
            }

            T selected = strategy.Select(candidates); // đa hình theo chiến lược
            selected.Borrow();                        // chuyển sang Borrowed
            return selected;
        }

        /// <summary>Trả thiết bị theo mã về kho.</summary>
        /// <exception cref="KeyNotFoundException">Khi không tìm thấy mã.</exception>
        public T ReturnDevice(string deviceId)
        {
            T device = repository.FindById(deviceId)
                       ?? throw new KeyNotFoundException($"Không tìm thấy thiết bị mã '{deviceId}'.");
            device.Return();
            return device;
        }
    }
}
