using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PBL3_Server.Models;
using PBL3_Server.Services.OrganizationService;
using X.PagedList;

namespace PBL3_Server.Controllers
{
    [Route("api/organization")]
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
        public async Task<ActionResult<List<Organization>>> GetAllOrganizations(int pageNumber = 1, int pageSize = 10, string type = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok(new { message = "You don't have permission to access this page" });
            }

            var organizations = await _OrganizationService.GetAllOrganizations();
            if (!string.IsNullOrEmpty(type))
            {
                organizations = organizations.Where(a => a.OrganizationType.ToLower() == type.ToLower()).ToList();
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
        [HttpPost]
        // Hàm thêm tài sản
        public async Task<ActionResult<List<Organization>>> AddOrganization(Organization organization)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _OrganizationService.AddOrganization(organization);
            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPut("{id}")]
        //Hàm cập nhật tài sản
        public async Task<ActionResult<List<Organization>>> UpdateOrganization(int id, Organization request)
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
        public async Task<ActionResult<List<Organization>>> DeleteOrganization(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _OrganizationService.DeleteOrganization(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Organization not found!" });

            return Ok(new { status = "success", data = result });
        }
    }
}
