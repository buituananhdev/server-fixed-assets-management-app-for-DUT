﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PBL3_Server.Models;
using PBL3_Server.Services.RoomService;
using System.Data;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PBL3_Server.Controllers
{
    [Route("api/asset")] // thiết lập đường dẫn tương đối cho API
    [ApiController] // ApiController đảm bảo rằng nếu một truy vấn không hợp lệ được thực hiện, sẽ trả về kết quả BadRequest (400)
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _AssetService;
        private readonly IRoomService _RoomService;

        public AssetController(IAssetService AssetService, IRoomService RoomService)
        {
            _AssetService = AssetService;
            _RoomService = RoomService;
        }

        [Authorize]
        [HttpGet]
        // Hàm trả về danh sách tài sản 
        public async Task<ActionResult<List<Asset>>> GetAllAssets(int pageNumber = 1, int pageSize = -1, string status = "", string room_id = "", string organization_id = "", string searchQuery = "", bool isConvert = false)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok(new { message = "You don't have permission to access this page" });
            }
            // Lấy danh sách tài sản và join với bảng Room để lấy thông tin phòng tài sản

            var assets = await _AssetService.GetAllAssets();
            if (pageSize == -1)
            {
                pageSize = assets.Count;
            }
            // Lọc tài sản theo mã khoa của phòng
            if (!string.IsNullOrEmpty(organization_id))
            {
                var rooms = await _RoomService.GetAllRooms();
                rooms = rooms.Where(r => r.organizationID.ToLower() == organization_id.ToLower()).ToList();
                assets = assets.Where(a => rooms.Any(r => r.RoomID == a.RoomID)).ToList();
            }

            // Lọc tài sản theo trạng thái nếu status khác rỗng
            if (!string.IsNullOrEmpty(status))
            {
                assets = assets.Where(a => a.Status.ToLower() == status.ToLower()).ToList();
            }

            // Lọc tài sản theo mã phòng nếu room_id khác rỗng
            if (!string.IsNullOrEmpty(room_id))
            {
                assets = assets.Where(a => a.RoomID.ToLower() == room_id.ToLower()).ToList();
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
                    a.Status.ToLower().Contains(searchQuery.ToLower()) ||
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
                    worksheet.Cells[4, 1].Value = "BẢNG KIỂM KÊ, ĐÁNH GIÁ TÀI SẢN";
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 1].Style.Font.Size = 22;

                    // Thêm tiêu đề cho sheet
                    worksheet.Cells[6, 1].Value = "Mã TS";
                    worksheet.Cells[6, 2].Value = "Mã số TB";
                    worksheet.Cells[6, 3].Value = "Năm sử dụng";
                    worksheet.Cells[6, 4].Value = "Thông số kỹ thuật";
                    worksheet.Cells[6, 5].Value = "Số lượng";
                    worksheet.Cells[6, 6].Value = "Thành tiền";
                    worksheet.Cells[6, 7].Value = "Trạng thái";
                    worksheet.Cells[6, 8].Value = "Ghi chú";

                    // Add data từ mảng assets vào file Excel
                    for (int i = 0; i < assets.Count; i++)
                    {
                        worksheet.Cells[i + 7, 1].Value = assets[i].AssetID;
                        worksheet.Cells[i + 7, 2].Value = assets[i].DeviceID;
                        worksheet.Cells[i + 7, 3].Value = assets[i].YearOfUse;
                        worksheet.Cells[i + 7, 4].Value = assets[i].TechnicalSpecification;
                        worksheet.Cells[i + 7, 5].Value = assets[i].Quantity;
                        worksheet.Cells[i + 7, 6].Value = assets[i].Cost;
                        worksheet.Cells[i + 7, 7].Value = assets[i].Status;
                        worksheet.Cells[i + 7, 8].Value = assets[i].Notes;
                    }

                    // Áp dụng định dạng cho header
                    using (var range = worksheet.Cells[6, 1, 6, 8])
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
        // Hàm trả về thông tin của tài sản qua ID
        public async Task<ActionResult<Asset>> GetSingleAsset(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _AssetService.GetSingleAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });
            return Ok(new { status = "success", data = result});
        }

        [Authorize]
        [HttpPost]
        // Hàm thêm tài sản
        public async Task<ActionResult<List<Asset>>> AddAsset(Asset asset)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            asset.AssetID = Guid.NewGuid().ToString();
            var result = await _AssetService.AddAsset(asset);
            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPut("{id}")]
        //Hàm cập nhật tài sản
        public async Task<ActionResult<List<Asset>>> UpdateAsset(string id, Asset request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _AssetService.UpdateAsset(id, request);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        // Hàm xóa tài sản theo ID
        public async Task<ActionResult<List<Asset>>> DeleteAsset(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _AssetService.DeleteAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPost("{id}")]
        // Hàm thanh lý tài sản theo ID
        public async Task<ActionResult<List<Asset>>> DisposedAsset(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _AssetService.DisposedAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }
    }
}
