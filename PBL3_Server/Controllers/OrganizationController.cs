using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using PBL3_Server.Models;
using PBL3_Server.Services.OrganizationService;
using System;
using X.PagedList;

namespace PBL3_Server.Controllers
{
    [Route("api/organizations")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _OrganizationService;

        public OrganizationController(IOrganizationService OrganizationService)
        {
            _OrganizationService = OrganizationService;
        }

        [Authorize]
        [HttpGet]
        // Hàm trả về danh sách tài sản 
        public async Task<ActionResult<List<Organization>>> GetAllOrganizations(int pageNumber = 1, int pageSize = 50, string organizationType = "", string searchQuery = "", bool isConvert = false)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok(new { message = "You don't have permission to access this page" });
            }

            var organizations = await _OrganizationService.GetAllOrganizations();
            if (!string.IsNullOrEmpty(organizationType))
            {
                organizations = organizations.Where(a => a.OrganizationType.ToLower() == organizationType.ToLower()).ToList();
            }


            // tìm kiếm tài sản
            if (!string.IsNullOrEmpty(searchQuery))
            {
                organizations = organizations.Where(o =>
                   o.OrganizationName.ToLower().Contains(searchQuery.ToLower())
                ).ToList();
            }

            if (isConvert)
            {
                var stream = new MemoryStream();
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Danh sách");
                    worksheet.Cells[1, 1].Value = "Trường Đại học Bách khoa";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 1].Value = "DANH SÁCH TỔ CHỨC";
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 1].Style.Font.Size = 22;

                    // Thêm tiêu đề cho sheet
                    worksheet.Cells[6, 1].Value = "Mã tổ chức";
                    worksheet.Cells[6, 2].Value = "Tên tổ chức";
                    worksheet.Cells[6, 3].Value = "Loại";

                    // Add data từ mảng assets vào file Excel
                    for (int i = 0; i < organizations.Count; i++)
                    {
                        worksheet.Cells[i + 7, 1].Value = organizations[i].OrganizationID;
                        worksheet.Cells[i + 7, 2].Value = organizations[i].OrganizationName;
                        worksheet.Cells[i + 7, 3].Value = organizations[i].OrganizationType;
                    }

                    // Áp dụng định dạng cho header
                    using (var range = worksheet.Cells[6, 1, 6, 3])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Font.Size = 10;
                    }

                    // Tự động căn chỉnh cột
                    worksheet.Cells.AutoFitColumns();
                    // Đặt tên file Excel
                    var fileName = "SoTheoDoiToChuc.xlsx";

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

            var pagedOrganizations = organizations.ToPagedList(pageNumber, pageSize);

            //Tạo đối tượng paginationInfo để lưu thông tin phân trang
            var paginationInfo = new PaginationInfo
            {
                TotalPages = pagedOrganizations.PageCount,
                CurrentPage = pagedOrganizations.PageNumber,
                HasPreviousPage = pagedOrganizations.HasPreviousPage,
                HasNextPage = pagedOrganizations.HasNextPage,
                PageSize = pagedOrganizations.PageSize
            };
            return Ok(new { status = "success", data = pagedOrganizations, meta = paginationInfo });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Organization>> GetSingleOrganization(string id = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }

            var result = await _OrganizationService.GetSingleOrganization(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Organization not found!" });
            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<Organization>>> AddOrganization(Organization organization)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }

            organization.OrganizationID = Guid.NewGuid().ToString().Substring(0, 29);
            await _OrganizationService.AddOrganization(organization);
            return Ok(new { status = "success" });
        }

        [Authorize]
        [HttpPut("{id}")]
        //Hàm cập nhật tài sản
        public async Task<ActionResult<List<Organization>>> UpdateOrganization(string id, Organization request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _OrganizationService.UpdateOrganization(id, request);
            if (result is null)
                return NotFound(new { status = "failure", message = "Organization not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        // Hàm xóa tài sản theo ID
        public async Task<ActionResult<List<Organization>>> DeleteOrganization(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _OrganizationService.DeleteOrganization(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Organization not found!" });

            return Ok(new { status = "success"});
        }
    }
}
