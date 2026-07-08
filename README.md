# HeartCare AI

HeartCare AI là website ASP.NET Core MVC hỗ trợ demo đề tài **Đánh giá nguy cơ mắc bệnh tim bằng mô hình mạng nơ-ron nhân tạo ANN**.

Website cho phép nhập thông tin bệnh nhân, mô phỏng phân tích nguy cơ bệnh tim, hiển thị kết quả có giải thích, lưu lịch sử đánh giá và trình bày dashboard thống kê trực quan.

## Công nghệ sử dụng

- ASP.NET Core MVC `net10.0`
- Razor View
- Bootstrap 5
- HTML, CSS, JavaScript
- Lưu dữ liệu demo bằng JSON tại `App_Data/predictions.json`
- Biểu đồ tự vẽ bằng Canvas JavaScript

## Chức năng chính

- Trang chủ giới thiệu HeartCare AI, quy trình phân tích, bài viết demo và đánh giá website.
- Trang đánh giá nguy cơ nhập thông tin bệnh nhân.
- Trang kết quả hiển thị phần trăm nguy cơ, mức nguy cơ, yếu tố ảnh hưởng và biểu đồ đóng góp.
- Trang dashboard thống kê theo mức nguy cơ, tuổi, giới tính, ECG, hút thuốc, đường huyết, cholesterol và huyết áp.
- Trang mô hình ANN mô tả dataset, tiền xử lý, kiến trúc mạng và chỉ số đánh giá.
- Trang lịch sử đánh giá có tìm kiếm, lọc theo mức nguy cơ và xem chi tiết từng bệnh nhân.
- Giao diện có preloader, page transition, sticky navbar, dark/light mode, toast notification, skeleton loading và animation.

## Yêu cầu môi trường

Cần cài đặt:

- .NET SDK 10
- Trình duyệt web như Chrome, Edge hoặc Firefox

Kiểm tra .NET:

```bash
dotnet --info
```

## Cách chạy website

Mở terminal tại thư mục project:

```bash
cd "C:\Users\nguye\OneDrive\文档\550"
```

Restore project:

```bash
dotnet restore
```

Build project:

```bash
dotnet build
```

Chạy website:

```bash
dotnet run --urls http://localhost:5050
```

Sau đó mở trình duyệt và truy cập:

```text
http://localhost:5050
```

## Hướng dẫn sử dụng

### 1. Trang chủ

Vào menu **Trang chủ** để xem giới thiệu hệ thống, quy trình hoạt động, bài viết demo và phần đánh giá website.

Tại phần đánh giá website, người dùng có thể nhập tên, chọn số sao và gửi nhận xét. Nhận xét sẽ hiển thị ngay trên giao diện để phục vụ demo.

### 2. Đánh giá nguy cơ

Vào menu **Đánh giá nguy cơ** và nhập thông tin bệnh nhân:

- Họ tên
- Tuổi
- Giới tính
- Huyết áp tâm thu
- Cholesterol
- Đường huyết lúc đói
- Kết quả ECG
- Tiền sử hút thuốc
- Loại đau ngực
- Nhịp tim tối đa
- Đau thắt ngực khi gắng sức
- Chỉ số ST depression

Nhấn **Phân tích nguy cơ bằng AI** để xem kết quả.

### 3. Kết quả dự đoán

Trang kết quả hiển thị:

- Phần trăm nguy cơ bệnh tim
- Mức nguy cơ: thấp, trung bình hoặc cao
- Diễn giải kết quả
- Biểu đồ đóng góp nguy cơ của từng yếu tố
- Bảng điểm chi tiết từng yếu tố
- Khuyến nghị tiếp theo
- Cảnh báo kết quả chỉ mang tính tham khảo

### 4. Dashboard thống kê

Vào menu **Thống kê** để xem dashboard gồm:

- Tổng số lượt đánh giá
- Số ca nguy cơ cao
- Xác suất nguy cơ trung bình
- Số ca ECG bất thường
- Huyết áp trung bình
- Cholesterol trung bình
- Biểu đồ tỷ lệ nguy cơ
- Biểu đồ xu hướng xác suất
- Biểu đồ theo nhóm tuổi
- Biểu đồ giới tính
- Biểu đồ ECG
- Biểu đồ hút thuốc
- Biểu đồ đường huyết
- Bảng lượt đánh giá gần đây

### 5. Lịch sử đánh giá

Vào menu **Lịch sử** để xem danh sách bệnh nhân đã được đánh giá.

Có thể:

- Tìm kiếm theo họ tên
- Lọc theo nguy cơ thấp, trung bình hoặc cao
- Bấm **Chi tiết** để xem lại kết quả từng bệnh nhân

### 6. Mô hình ANN

Vào menu **Mô hình ANN** để xem phần mô tả phục vụ báo cáo:

- Dataset Heart Disease
- Các bước tiền xử lý dữ liệu
- Kiến trúc ANN đề xuất
- Pipeline huấn luyện
- Accuracy, Precision, Recall, F1-score, ROC-AUC

## Logic dự đoán hiện tại

Phiên bản hiện tại dùng service mô phỏng tại:

```text
Services/RuleBasedRiskPredictionService.cs
```

Service này tính điểm nguy cơ dựa trên các yếu tố:

- Tuổi
- Huyết áp
- Cholesterol
- Đường huyết
- ECG
- Hút thuốc
- Giới tính
- Đau ngực
- Nhịp tim tối đa
- Đau thắt ngực khi gắng sức
- ST depression

Kết quả được quy đổi thành xác suất từ `0%` đến `100%`:

- Dưới `40%`: nguy cơ thấp
- Từ `40%` đến dưới `70%`: nguy cơ trung bình
- Từ `70%` trở lên: nguy cơ cao

Khi có model ANN thật, có thể thay logic trong `RuleBasedRiskPredictionService` bằng API hoặc service load model thật.

## Cấu trúc thư mục chính

```text
Controllers/
  HomeController.cs
  RiskAssessmentController.cs
  DashboardController.cs
  PredictionHistoryController.cs
  ModelInfoController.cs

Models/
  Patient.cs
  Prediction.cs
  RiskContribution.cs
  RiskLevel.cs
  ModelMetric.cs

ViewModels/
  RiskInputViewModel.cs
  RiskResultViewModel.cs
  DashboardViewModel.cs
  PredictionHistoryViewModel.cs

Services/
  IRiskPredictionService.cs
  RuleBasedRiskPredictionService.cs

Repositories/
  IPredictionRepository.cs
  JsonPredictionRepository.cs

Views/
  Home/
  RiskAssessment/
  Dashboard/
  PredictionHistory/
  ModelInfo/
  Shared/

wwwroot/
  css/site.css
  js/site.js
  images/heartcare-hero.png

App_Data/
  predictions.json

DB.txt
```

## Dữ liệu demo

Dữ liệu lịch sử đánh giá được lưu tại:

```text
App_Data/predictions.json
```

Schema tham khảo để chuyển sang SQL Server hoặc SQLite nằm trong:

```text
DB.txt
```

## Lưu ý y tế

Website chỉ phục vụ mục đích học tập và demo báo cáo. Kết quả đánh giá nguy cơ chỉ mang tính tham khảo, không thay thế chẩn đoán, tư vấn hoặc điều trị của bác sĩ chuyên khoa.

## Lệnh kiểm tra nhanh

Build:

```bash
dotnet build --no-restore
```

Kiểm tra JavaScript:

```bash
node --check wwwroot/js/site.js
```

Chạy local:

```bash
dotnet run --no-build --urls http://localhost:5050
```
