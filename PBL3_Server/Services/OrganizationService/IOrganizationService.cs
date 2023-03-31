namespace PBL3_Server.Services.OrganizationService
{
    public interface IOrganizationService
    {
        Task<List<Organization>> GetAllOrganizations();
        Task<List<Organization>> AddOrganization(Organization organization);
        Task<List<Organization>?> UpdateOrganization(int id, Organization request);
        Task<List<Organization>?> DeleteOrganization(int id);
    }
}
