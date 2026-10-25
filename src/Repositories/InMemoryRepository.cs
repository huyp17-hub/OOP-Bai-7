/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

using TeachingDeviceAllocation.Exceptions;
using TeachingDeviceAllocation.Models;

namespace TeachingDeviceAllocation.Repositories
{
    /// <summary>
    /// Cài đặt kho lưu trữ trong bộ nhớ.
    /// Dùng Dictionary&lt;string, T&gt; để tìm theo mã O(1) và phát hiện trùng mã nhanh,
    /// kèm List&lt;string&gt; giữ thứ tự thêm vào (để FirstAvailableStrategy có ý nghĩa ổn định).
    /// </summary>
    public class InMemoryRepository<T> : IRepository<T> where T : IIdentifiable
    {
        // Mã được so sánh không phân biệt hoa thường: "PR01" và "pr01" coi là trùng.
        private readonly Dictionary<string, T> itemsById = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> insertionOrder = new();

        /// <summary>Số phần tử hiện có.</summary>
        public int Count => itemsById.Count;

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Khi item là null.</exception>
        /// <exception cref="DuplicateIdException">Khi Id đã tồn tại.</exception>
        public void Add(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item), "Không chấp nhận phần tử null.");
            }
            if (itemsById.ContainsKey(item.Id))
            {
                throw new DuplicateIdException(item.Id);
            }
            itemsById.Add(item.Id, item);
            insertionOrder.Add(item.Id);
        }

        /// <inheritdoc/>
        public bool Remove(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !itemsById.TryGetValue(id, out var item))
            {
                return false;
            }
            itemsById.Remove(id);
            insertionOrder.RemoveAll(k => string.Equals(k, item.Id, StringComparison.OrdinalIgnoreCase));
            return true;
        }

        /// <inheritdoc/>
        public T? FindById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return default;
            }
            return itemsById.TryGetValue(id, out var item) ? item : default;
        }

        /// <inheritdoc/>
        /// <remarks>Trả về bản sao bọc ReadOnly nên mã ngoài không sửa được dữ liệu nội bộ.</remarks>
        public IReadOnlyList<T> GetAll()
            => insertionOrder.Select(id => itemsById[id]).ToList().AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyList<T> FindAll(Predicate<T> condition)
        {
            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            return GetAll().Where(item => condition(item)).ToList().AsReadOnly();
        }
    }
}
