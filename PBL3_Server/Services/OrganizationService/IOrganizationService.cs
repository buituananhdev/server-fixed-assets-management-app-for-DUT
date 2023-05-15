namespace PBL3_Server.Services.OrganizationService
{
    public interface IOrganizationService
    {
        Task<List<Organization>> GetAllOrganizations();
        Task<Organization?> GetSingleOrganization(string organizationID);
        Task<List<Organization>> AddOrganization(Organization organization);
        Task<List<Organization>?> UpdateOrganization(string id, Organization request);
        Task<List<Organization>?> DeleteOrganization(string id);
    }
}
