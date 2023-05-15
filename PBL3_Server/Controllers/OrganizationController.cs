using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PBL3_Server.Models;
using PBL3_Server.Services.OrganizationService;
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
        public async Task<ActionResult<List<Organization>>> GetAllOrganizations(int pageNumber = 1, int pageSize = 50, string organizationType = "", string searchQuery = "")
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
