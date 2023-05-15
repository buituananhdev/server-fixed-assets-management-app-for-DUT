using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PBL3_Server.Services.DisposedAssetService;
using PBL3_Server.Services.RoomService;
using System.Data;
using X.PagedList;

namespace PBL3_Server.Controllers
{
    [Route("api/disposed_assets")]
    [ApiController]
    public class DisposedAssetController : ControllerBase
    {
        private readonly IDisposedAssetService _DisposedAssetService;
        private readonly IRoomService _RoomService;

        public DisposedAssetController(IDisposedAssetService DisposedAssetService, IRoomService RoomService)
        {
            _DisposedAssetService = DisposedAssetService;
            _RoomService = RoomService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<DisposedAsset>>> GetAllDisposedAssets(int pageNumber = 1, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, string organization_id = "", string searchQuery = "", bool isConvert = false)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var assets = await _DisposedAssetService.GetAllDisposedAssets();

            // Lọc tài sản theo mã khoa của phòng
            if (!string.IsNullOrEmpty(organization_id))
            {
                var rooms = await _RoomService.GetAllRooms();
                rooms = rooms.Where(r => r.organizationID.ToLower() == organization_id.ToLower()).ToList();
                assets = assets.Where(a => rooms.Any(r => r.RoomID == a.RoomID)).ToList();
            }

            // lọc tài sản theo ngày thanh lý nằm trong start date và end date
            if (startDate.HasValue && endDate.HasValue)
            {
                assets = assets.Where(a => a.DateDisposed >= startDate.Value && a.DateDisposed <= endDate.Value).ToList();
            }

            // tìm kiếm tài sản
            if (!string.IsNullOrEmpty(searchQuery))
            {
                assets = assets.Where(a =>
                    a.AssetID.ToLower() == searchQuery.ToLower() ||
                    a.DeviceID.ToLower().Contains(searchQuery.ToLower()) ||
                    a.AssetName.ToLower().Contains(searchQuery.ToLower()) ||
                    a.Cost.ToString().ToLower().Contains(searchQuery.ToLower()) ||
                    a.RoomID.ToLower().Contains(searchQuery.ToLower()) ||
                    a.YearOfUse.ToString().ToLower().Contains(searchQuery.ToLower()) ||
                    a.Quantity.ToString().ToLower().Contains(searchQuery.ToLower()) ||
                    a.TechnicalSpecification.ToLower().Contains(searchQuery.ToLower()) ||
                    a.Quantity.ToString().ToLower().Contains(searchQuery.ToLower()) ||
                    a.Notes.ToLower().Contains(searchQuery.ToLower())
                ).ToList();
            }

            // convert to excel
            if (isConvert)
            {
                var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Danh sách");
                    worksheet.Cells[1, 1].Value = "Trường Đại học Bách khoa";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[2, 1].Value = "Khoa Công nghệ thông tin";
                    worksheet.Cells[2, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 1].Value = "BẢNG KIỂM KÊ, ĐÁNH GIÁ TÀI SẢN ĐÃ THANH LÝ";
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 1].Style.Font.Size = 22;

                    // Thêm tiêu đề cho sheet
                    worksheet.Cells[6, 1].Value = "Mã TS";
                    worksheet.Cells[6, 2].Value = "Mã số TB";
                    worksheet.Cells[6, 3].Value = "Tên tài sản";
                    worksheet.Cells[6, 4].Value = "Năm sử dụng";
                    worksheet.Cells[6, 5].Value = "Thông số kỹ thuật";
                    worksheet.Cells[6, 6].Value = "Số lượng";
                    worksheet.Cells[6, 7].Value = "Thành tiền";
                    worksheet.Cells[6, 8].Value = "Trạng thái";
                    worksheet.Cells[6, 9].Value = "Ngày thanh lý";
                    worksheet.Cells[6, 10].Value = "Ghi chú";

                    // Add data từ mảng assets vào file Excel
                    for (int i = 0; i < assets.Count; i++)
                    {
                        worksheet.Cells[i + 7, 1].Value = assets[i].AssetID;
                        worksheet.Cells[i + 7, 2].Value = assets[i].DeviceID;
                        worksheet.Cells[i + 7, 3].Value = assets[i].AssetName;
                        worksheet.Cells[i + 7, 4].Value = assets[i].YearOfUse;
                        worksheet.Cells[i + 7, 5].Value = assets[i].TechnicalSpecification;
                        worksheet.Cells[i + 7, 6].Value = assets[i].Quantity;
                        worksheet.Cells[i + 7, 7].Value = assets[i].Cost;
                        worksheet.Cells[i + 7, 8].Value = assets[i].Status;
                        worksheet.Cells[i + 7, 9].Value = assets[i].DateDisposed.ToString("dd/MM/yyyy");
                        worksheet.Cells[i + 7, 10].Value = assets[i].Notes;
                    }

                    // Áp dụng định dạng cho header
                    using (var range = worksheet.Cells[6, 1, 6, 10])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Font.Size = 10;
                    }

                    // Tự động căn chỉnh cột
                    worksheet.Cells.AutoFitColumns();
                    // Đặt tên file Excel
                    var fileName = "SoTheoDoiTSCD.xlsx";

                    // Xuất file Excel
                    package.Save();

                    // Thiết lập HTTP header để trình duyệt có thể tải xuống file
                    Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.ContentLength = stream.Length;

                    // Đưa dữ liệu trong file Excel ra response
                    stream.Seek(0, SeekOrigin.Begin);
                    return File(stream, Response.ContentType, fileName);
                }
            }

            var pagedAssets = assets.ToPagedList(pageNumber, pageSize);
            //Tạo đối tượng paginationInfo để lưu thông tin phân trang
            var paginationInfo = new PaginationInfo
            {
                TotalPages = pagedAssets.PageCount,
                CurrentPage = pagedAssets.PageNumber,
                HasPreviousPage = pagedAssets.HasPreviousPage,
                HasNextPage = pagedAssets.HasNextPage,
                PageSize = pagedAssets.PageSize
            };
            return Ok(new { status = "success", data = pagedAssets, meta = paginationInfo });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<DisposedAsset>> GetSingleDisposedAsset(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _DisposedAssetService.GetSingleDisposedAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<DisposedAsset>>> AddDisposedAsset(DisposedAsset asset)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            asset.AssetID = Guid.NewGuid().ToString();
            var result = await _DisposedAssetService.AddDisposedAsset(asset);
            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<DisposedAsset>>> UpdateDisposedAsset(string id, DisposedAsset request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _DisposedAssetService.UpdateDisposedAsset(id, request);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<DisposedAsset>>> DeleteDisposedAsset(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _DisposedAssetService.DeleteDisposedAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPost("{id}")]
        // Hàm hủy thanh lý tài sản theo ID
        public async Task<ActionResult<List<DisposedAsset>>> CancelDisposeAsset(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _DisposedAssetService.CancelDisposeAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }
    }
}
