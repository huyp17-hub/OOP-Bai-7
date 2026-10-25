/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

using TeachingDeviceAllocation.Exceptions;

namespace TeachingDeviceAllocation.Strategies
{
    /// <summary>
    /// Chọn phần tử đầu tiên trong danh sách ứng viên (thứ tự do repository quyết định).
    /// </summary>
    public class FirstAvailableStrategy<T> : ISelectionStrategy<T>
    {
        /// <inheritdoc/>
        public T Select(IReadOnlyList<T> candidates)
        {
            if (candidates is null || candidates.Count == 0)
            {
                throw new NoSuitableItemException("FirstAvailableStrategy: danh sách ứng viên rỗng.");
            }
            return candidates[0];
        }
    }
}
