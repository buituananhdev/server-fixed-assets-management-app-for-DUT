using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PBL3_Server.Models;
using PBL3_Server.Services.RoomService;
using System;
using System.Drawing;
using X.PagedList;

namespace PBL3_Server.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _RoomService;

        public RoomController(IRoomService RoomService)
        {
            _RoomService = RoomService;
        }

        [Authorize]
        [HttpGet]
        // Hàm trả về danh sách phòng 
        public async Task<ActionResult<List<Asset>>> GetAllRooms(int pageNumber = -1, int pageSize = -1, string organization_id = "", string searchQuery = "", bool isConvert = false)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok(new { message = "You don't have permission to access this page" });
            }

            var rooms = await _RoomService.GetAllRooms();

            if (!string.IsNullOrEmpty(organization_id))
            {
                rooms = rooms.Where(a => a.organizationID.ToLower() == organization_id.ToLower()).ToList();
            }

            // tìm kiếm tài sản
            if (!string.IsNullOrEmpty(searchQuery))
            {
                rooms = rooms.Where(r =>
                   r.RoomID.ToLower().Contains(searchQuery.ToLower()) ||
                   r.RoomName.ToLower().Contains(searchQuery.ToLower())
                ).ToList();
            }

            if (isConvert)
            {
                var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Danh sách");

                    // Thiết lập font và kích thước cho tiêu đề
                    worksheet.Cells[1, 1].Value = "Trường Đại học Bách khoa";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Font.Size = 14;

                    worksheet.Cells[4, 1].Value = "DANH SÁCH PHÒNG";
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 1].Style.Font.Size = 18;

                    // Thiết lập border cho tiêu đề
                    worksheet.Cells[1, 1, 1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[4, 1, 4, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    // Thêm tiêu đề cho sheet
                    string[] headers = { "Mã phòng", "Tên phòng", "Mã khoa" };
                    for (int i = 1; i <= headers.Length; i++)
                    {
                        worksheet.Cells[6, i].Value = headers[i - 1];
                        worksheet.Cells[6, i].Style.Font.Bold = true;

                        // Thiết lập border và màu nền cho tiêu đề cột
                        worksheet.Cells[6, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[6, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[6, i].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }

                    // Add data từ mảng rooms vào file Excel
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        var currentRow = i + 7;

                        worksheet.Cells[currentRow, 1].Value = rooms[i].RoomID;
                        worksheet.Cells[currentRow, 2].Value = rooms[i].RoomName;
                        worksheet.Cells[currentRow, 3].Value = rooms[i].organizationID;

                        // Thiết lập border cho dòng dữ liệu
                        worksheet.Cells[currentRow, 1, currentRow, headers.Length].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    // Thiết lập border cho toàn bộ bảng dữ liệu
                    worksheet.Cells[6, 1, rooms.Count + 6, headers.Length].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[6, 1, rooms.Count + 6, headers.Length].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[6, 1, rooms.Count + 6, headers.Length].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[6, 1, rooms.Count + 6, headers.Length].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    // Áp dụng định dạng cho header
                    using (var range = worksheet.Cells[6, 1, 6, headers.Length])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Font.Size = 10;
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    // Tự động căn chỉnh cột
                    worksheet.Cells.AutoFitColumns();

                    // Đặt độ rộng cho cột "Tên phòng"
                    worksheet.Column(2).Width = 25;

                    // Đặt tên file Excel
                    var fileName = "SoTheoDoiPhong.xlsx";

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

            if (pageNumber == -1 && pageSize == -1)
            {
                return Ok(new { status = "success", data = rooms });
            }
            else
            {
                var pagedrooms = rooms.ToPagedList(pageNumber, pageSize);

                //Tạo đối tượng paginationInfo để lưu thông tin phân trang
                var paginationInfo = new PaginationInfo
                {
                    TotalPages = pagedrooms.PageCount,
                    CurrentPage = pagedrooms.PageNumber,
                    HasPreviousPage = pagedrooms.HasPreviousPage,
                    HasNextPage = pagedrooms.HasNextPage,
                    PageSize = pagedrooms.PageSize
                };
                return Ok(new { status = "success", data = pagedrooms, meta = paginationInfo });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<Room>>> AddRoom(Room room)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            await _RoomService.AddRoom(room);
            return Ok(new { status = "success" });
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetSingleRoom(string id = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }

            var result = await _RoomService.GetSingleRoom(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Room not found!" });
            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPut("{id}")]
        //Hàm cập nhật tài sản
        public async Task<ActionResult<List<Room>>> UpdateRoom(string id, Room request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _RoomService.UpdateRoom(id, request);
            if (result is null)
                return NotFound(new { status = "failure", message = "Room not found!" });

            return Ok(new { status = "success", data = result });
        }
    }
}
