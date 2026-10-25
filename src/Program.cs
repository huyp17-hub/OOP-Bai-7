/****************/
// Mã sinh viên: 202418915
// Họ tên: Phạm Quốc Huy
/****************/

using System.Text;
using TeachingDeviceAllocation.Exceptions;
using TeachingDeviceAllocation.Models;
using TeachingDeviceAllocation.Repositories;
using TeachingDeviceAllocation.Services;
using TeachingDeviceAllocation.Strategies;

namespace TeachingDeviceAllocation
{
    /// <summary>
    /// Chương trình kiểm thử theo mục 9 của đề bài.
    /// Mỗi kiểm thử in ra chi tiết và kết luận [PASS]/[FAIL]; cuối cùng in tổng kết.
    /// </summary>
    public static class Program
    {
        private static int passed;
        private static int failed;

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            // ===== Dữ liệu mẫu =====
            // 3 điều khiển máy chiếu, pin khác nhau (PR02 pin thấp -> không sẵn sàng)
            var pr01 = new ProjectorRemote("PR01", "Remote máy chiếu Epson", "Tủ A - Ngăn 1", 85);
            var pr02 = new ProjectorRemote("PR02", "Remote máy chiếu Sony", "Tủ A - Ngăn 2", 15);
            var pr03 = new ProjectorRemote("PR03", "Remote máy chiếu Panasonic", "Tủ A - Ngăn 3", 60);

            // 2 điều khiển điều hòa, tập phòng tương thích khác nhau
            var ac01 = new AirConditionerRemote("AC01", "Remote điều hòa Daikin", "Tủ B - Ngăn 1",
                                                new[] { "A201", "A202", "A203" });
            var ac02 = new AirConditionerRemote("AC02", "Remote điều hòa LG", "Tủ B - Ngăn 2",
                                                new[] { "B101", "B102" });

            // 3 micro: MC01 pin thấp (không sẵn sàng), MC02 & MC03 sẵn sàng
            var mc01 = new WirelessMicrophone("MC01", "Micro Shure 1", "Tủ C - Ngăn 1", 25, 90);
            var mc02 = new WirelessMicrophone("MC02", "Micro Shure 2", "Tủ C - Ngăn 2", 80, 70);
            var mc03 = new WirelessMicrophone("MC03", "Micro Sennheiser", "Tủ C - Ngăn 3", 95, 85);

            // Cùng MỘT lớp generic InMemoryRepository<T> dùng cho 3 loại thiết bị
            var projectorRepo = new InMemoryRepository<ProjectorRemote>();
            var acRepo = new InMemoryRepository<AirConditionerRemote>();
            var micRepo = new InMemoryRepository<WirelessMicrophone>();

            // ---------------------------------------------------------------
            Header("TEST 1: Thêm thiết bị vào repository");
            projectorRepo.Add(pr01); projectorRepo.Add(pr02); projectorRepo.Add(pr03);
            acRepo.Add(ac01); acRepo.Add(ac02);
            micRepo.Add(mc01); micRepo.Add(mc02); micRepo.Add(mc03);
            PrintList("Máy chiếu", projectorRepo.GetAll());
            PrintList("Điều hòa", acRepo.GetAll());
            PrintList("Micro", micRepo.GetAll());
            Check("Repo máy chiếu có 3, điều hòa có 2, micro có 3",
                  projectorRepo.Count == 3 && acRepo.Count == 2 && micRepo.Count == 3);

            // ---------------------------------------------------------------
            Header("TEST 2: Thêm hai thiết bị trùng mã");
            var duplicate = new ProjectorRemote("PR01", "Remote trùng mã", "Tủ D", 90);
            ExpectException<DuplicateIdException>("Thêm PR01 lần 2 -> DuplicateIdException",
                () => projectorRepo.Add(duplicate),
                ex => Console.WriteLine($"   DuplicateId = {ex.DuplicateId}"));
            ExpectException<DuplicateIdException>("Thêm 'pr01' (khác hoa thường) -> vẫn bị coi là trùng",
                () => projectorRepo.Add(new ProjectorRemote("pr01", "x", "y", 50)));
            ExpectException<ArgumentNullException>("Thêm null -> ArgumentNullException",
                () => projectorRepo.Add(null!));
            Check("Số lượng vẫn là 3 sau các lần thêm lỗi", projectorRepo.Count == 3);

            // ---------------------------------------------------------------
            Header("TEST 3: Tìm thiết bị theo mã");
            var found = micRepo.FindById("MC03");
            Console.WriteLine($"   FindById(\"MC03\") -> {found}");
            Check("Tìm thấy MC03", ReferenceEquals(found, mc03));
            Check("FindById(\"XX99\") trả về null", micRepo.FindById("XX99") is null);
            Check("Remove(\"XX99\") trả về false", micRepo.Remove("XX99") == false);

            // ---------------------------------------------------------------
            Header("TEST 4: Lọc các thiết bị sẵn sàng");
            var readyProjectors = projectorRepo.FindAll(d => d.IsReadyForUse());
            var readyMics = micRepo.FindAll(d => d.IsReadyForUse());
            PrintList("Máy chiếu sẵn sàng", readyProjectors);
            PrintList("Micro sẵn sàng", readyMics);
            Check("PR02 (pin 15%) bị loại, còn PR01, PR03",
                  Ids(readyProjectors) == "PR01,PR03");
            Check("MC01 (pin 25%) bị loại, còn MC02, MC03",
                  Ids(readyMics) == "MC02,MC03");

            // Kiểm tra GetAll() không làm lộ collection nội bộ
            var snapshot = projectorRepo.GetAll();
            bool cannotModify = snapshot is not IList<ProjectorRemote> list || list.IsReadOnly;
            Check("GetAll() trả về danh sách chỉ đọc", cannotModify);

            // ---------------------------------------------------------------
            Header("TEST 5: Chọn điều khiển máy chiếu có điểm cao nhất");
            var projectorService = new AllocationService<ProjectorRemote>(
                projectorRepo,
                new HighestPriorityStrategy<ProjectorRemote>(d => d.GetDefaultPriorityScore()));
            ProjectorRemote selectedProjector = projectorService.Allocate(device => true);
            Console.WriteLine($"   Được chọn: {selectedProjector}");
            Check("Chọn PR01 (pin 85% cao nhất trong các thiết bị sẵn sàng)", selectedProjector.Id == "PR01");

            // ---------------------------------------------------------------
            Header("TEST 6: Chọn điều khiển điều hòa tương thích với phòng A203");
            var airConditionerService = new AllocationService<AirConditionerRemote>(
                acRepo, new FirstAvailableStrategy<AirConditionerRemote>());
            AirConditionerRemote selectedAc = airConditionerService.Allocate(
                device => device.IsCompatibleWith("A203"));
            Console.WriteLine($"   Được chọn: {selectedAc}");
            Check("Chọn AC01 (tương thích A203)", selectedAc.Id == "AC01");

            var selectedAcB = airConditionerService.Allocate(d => d.IsCompatibleWith("B102"));
            Console.WriteLine($"   Phòng B102 -> {selectedAcB.Id}");
            Check("Phòng B102 -> AC02", selectedAcB.Id == "AC02");

            // ---------------------------------------------------------------
            Header("TEST 7: Chọn micro đầu tiên sẵn sàng");
            var micService = new AllocationService<WirelessMicrophone>(
                micRepo, new FirstAvailableStrategy<WirelessMicrophone>());
            var firstMic = micService.Allocate(null);
            Console.WriteLine($"   Được chọn: {firstMic}");
            Check("Chọn MC02 (MC01 không sẵn sàng nên bị bỏ qua)", firstMic.Id == "MC02");

            // ---------------------------------------------------------------
            Header("TEST 8: Đổi chiến lược trong thời gian chạy");
            micService.ReturnDevice("MC02"); // trả lại để MC02 & MC03 cùng là ứng viên
            var candidates = micService.GetCandidates();
            PrintList("Ứng viên micro", candidates);
            foreach (var m in candidates)
            {
                Console.WriteLine($"   Điểm {m.Id} = {m.GetDefaultPriorityScore()}");
            }

            string beforeOrder = Ids(candidates);
            var byFirst = micService.CurrentStrategy.Select(candidates);
            Console.WriteLine($"   Chiến lược {micService.CurrentStrategy.Name} -> {byFirst.Id}");

            micService.ChangeStrategy(new HighestPriorityStrategy<WirelessMicrophone>(
                d => d.GetDefaultPriorityScore()));
            var byHighest = micService.Allocate(null);
            Console.WriteLine($"   Chiến lược {micService.CurrentStrategy.Name} -> {byHighest.Id}");
            Check("FirstAvailable chọn MC02, HighestPriority chọn MC03",
                  byFirst.Id == "MC02" && byHighest.Id == "MC03");
            Check("Chiến lược không thay đổi danh sách đầu vào", Ids(candidates) == beforeOrder);

            // Chiến lược tùy biến bằng lambda: ưu tiên tín hiệu tốt nhất
            micService.ChangeStrategy(new HighestPriorityStrategy<WirelessMicrophone>(d => d.SignalQuality));
            var bySignal = micService.Allocate(null);
            Console.WriteLine($"   Chiến lược 'tín hiệu cao nhất' -> {bySignal.Id}");
            Check("Còn MC02 là ứng viên duy nhất -> chọn MC02", bySignal.Id == "MC02");

            // ---------------------------------------------------------------
            Header("TEST 9: Cấp phát khi không có ứng viên phù hợp");
            ExpectException<NoSuitableItemException>("Điều hòa cho phòng C999 -> NoSuitableItemException",
                () => airConditionerService.Allocate(d => d.IsCompatibleWith("C999")));
            ExpectException<NoSuitableItemException>("Hết micro sẵn sàng -> NoSuitableItemException",
                () => micService.Allocate(null));
            ExpectException<NoSuitableItemException>("Strategy.Select(danh sách rỗng) -> NoSuitableItemException",
                () => new FirstAvailableStrategy<ProjectorRemote>().Select(new List<ProjectorRemote>()));

            // ---------------------------------------------------------------
            Header("TEST 10: Thiết bị được chọn chuyển sang Borrowed");
            Console.WriteLine($"   PR01: {pr01.Status}, AC01: {ac01.Status}, MC03: {mc03.Status}");
            Check("PR01, AC01, MC03 có trạng thái Borrowed",
                  pr01.Status == DeviceStatus.Borrowed
                  && ac01.Status == DeviceStatus.Borrowed
                  && mc03.Status == DeviceStatus.Borrowed);
            ExpectException<DeviceNotReadyException>("Mượn lại PR01 đang Borrowed -> DeviceNotReadyException",
                () => pr01.Borrow(),
                ex => Console.WriteLine($"   DeviceId={ex.DeviceId}, CurrentStatus={ex.CurrentStatus}"));
            projectorService.ReturnDevice("PR01");
            Check("Trả PR01 -> Available", pr01.Status == DeviceStatus.Available);
            ExpectException<DeviceNotReadyException>("Trả PR03 khi chưa mượn -> DeviceNotReadyException",
                () => pr03.Return());

            // ---------------------------------------------------------------
            Header("TEST 11: Repository & service generic dùng cho nhiều loại thiết bị");
            // Một hàm generic duy nhất in thống kê cho mọi repository — không sao chép mã.
            PrintStats(projectorRepo);
            PrintStats(acRepo);
            PrintStats(micRepo);

            // Loại thiết bị MỚI: không sửa InMemoryRepository / Strategy / AllocationService
            var displayRepo = new InMemoryRepository<InteractiveDisplayRemote>();
            displayRepo.Add(new InteractiveDisplayRemote("ID01", "Remote màn hình tương tác 1", "Tủ D", 70, true, false));
            displayRepo.Add(new InteractiveDisplayRemote("ID02", "Remote màn hình tương tác 2", "Tủ D", 65, true, true));
            displayRepo.Add(new InteractiveDisplayRemote("ID03", "Remote màn hình tương tác 3", "Tủ D", 99, false, true));
            var displayService = new AllocationService<InteractiveDisplayRemote>(
                displayRepo,
                new HighestPriorityStrategy<InteractiveDisplayRemote>(d => d.GetDefaultPriorityScore()));
            var display = displayService.Allocate(null);
            PrintStats(displayRepo);
            Console.WriteLine($"   Được chọn: {display}");
            Check("ID02 (65 + 10 cảm ứng = 75) thắng ID01 (70); ID03 chưa ghép nối bị loại",
                  display.Id == "ID02");

            // Đa hình qua lớp cơ sở: một repository chứa nhiều loại thiết bị khác nhau
            var allDevices = new InMemoryRepository<TeachingDevice>();
            foreach (TeachingDevice d in new TeachingDevice[] { pr01, pr02, pr03, ac01, ac02, mc01, mc02, mc03 })
            {
                allDevices.Add(d);
            }
            Console.WriteLine("   Kho tổng hợp (gọi đa hình IsReadyForUse / GetDefaultPriorityScore):");
            foreach (var d in allDevices.GetAll())
            {
                Console.WriteLine($"     {d.Id,-5} {d.GetType().Name,-22} ready={d.IsReadyForUse(),-5} score={d.GetDefaultPriorityScore()}");
            }
            Check("Repository<TeachingDevice> chứa 8 thiết bị thuộc 3 loại", allDevices.Count == 8);

            // ===== Tổng kết =====
            Console.WriteLine();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"TỔNG KẾT: {passed} PASS, {failed} FAIL");
            Console.WriteLine(new string('=', 60));
        }

        // ================= Các hàm hỗ trợ kiểm thử =================

        /// <summary>In tiêu đề một nhóm kiểm thử.</summary>
        private static void Header(string title)
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', 60));
            Console.WriteLine(title);
            Console.WriteLine(new string('-', 60));
        }

        /// <summary>Ghi nhận và in kết quả của một khẳng định.</summary>
        private static void Check(string description, bool condition)
        {
            if (condition) passed++; else failed++;
            Console.WriteLine($"   [{(condition ? "PASS" : "FAIL")}] {description}");
        }

        /// <summary>Khẳng định hành động phát sinh đúng loại ngoại lệ TEx.</summary>
        private static void ExpectException<TEx>(string description, Action action, Action<TEx>? inspect = null)
            where TEx : Exception
        {
            try
            {
                action();
                Check(description + " (không có ngoại lệ!)", false);
            }
            catch (TEx ex)
            {
                Console.WriteLine($"   Bắt được {typeof(TEx).Name}: {ex.Message}");
                inspect?.Invoke(ex);
                Check(description, true);
            }
            catch (Exception ex)
            {
                Check($"{description} (sai loại: {ex.GetType().Name})", false);
            }
        }

        /// <summary>In danh sách thiết bị.</summary>
        private static void PrintList<T>(string label, IEnumerable<T> items)
        {
            Console.WriteLine($"   {label}:");
            foreach (var item in items)
            {
                Console.WriteLine($"     - {item}");
            }
        }

        /// <summary>Nối các mã thành chuỗi "A,B,C" để so sánh.</summary>
        private static string Ids<T>(IEnumerable<T> items) where T : IIdentifiable
            => string.Join(",", items.Select(i => i.Id));

        /// <summary>
        /// Hàm generic dùng chung cho MỌI repository thiết bị (minh họa tái sử dụng, không sao chép mã).
        /// </summary>
        private static void PrintStats<T>(IRepository<T> repo) where T : TeachingDevice
        {
            int total = repo.GetAll().Count;
            int ready = repo.FindAll(d => d.IsReadyForUse()).Count;
            int borrowed = repo.FindAll(d => d.Status == DeviceStatus.Borrowed).Count;
            Console.WriteLine($"   Repository<{typeof(T).Name}>: tổng={total}, sẵn sàng={ready}, đang mượn={borrowed}");
        }
    }
}
