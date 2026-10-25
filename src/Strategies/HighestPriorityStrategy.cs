/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

using TeachingDeviceAllocation.Exceptions;

namespace TeachingDeviceAllocation.Strategies
{
    /// <summary>
    /// Chọn phần tử có điểm cao nhất theo hàm tính điểm truyền từ bên ngoài.
    /// Nếu nhiều phần tử bằng điểm, chọn phần tử xuất hiện trước (ổn định).
    /// </summary>
    public class HighestPriorityStrategy<T> : ISelectionStrategy<T>
    {
        private readonly Func<T, double> scoreSelector;

        /// <param name="scoreSelector">Hàm tính điểm cho một ứng viên.</param>
        public HighestPriorityStrategy(Func<T, double> scoreSelector)
        {
            this.scoreSelector = scoreSelector ?? throw new ArgumentNullException(nameof(scoreSelector));
        }

        /// <inheritdoc/>
        public T Select(IReadOnlyList<T> candidates)
        {
            if (candidates is null || candidates.Count == 0)
            {
                throw new NoSuitableItemException("HighestPriorityStrategy: danh sách ứng viên rỗng.");
            }

            // Duyệt một lượt, chỉ đọc danh sách — không sắp xếp, không sửa đầu vào.
            T best = candidates[0];
            double bestScore = scoreSelector(best);
            for (int i = 1; i < candidates.Count; i++)
            {
                double score = scoreSelector(candidates[i]);
                if (score > bestScore)
                {
                    best = candidates[i];
                    bestScore = score;
                }
            }
            return best;
        }
    }
}
