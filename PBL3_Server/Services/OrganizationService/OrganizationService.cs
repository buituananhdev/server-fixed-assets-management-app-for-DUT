using PBL3_Server.Models;

namespace PBL3_Server.Services.OrganizationService
{
    public class OrganizationService : IOrganizationService
    {
        private static List<Organization> Organizations = new List<Organization>();

        private readonly DataContext _context;

        public OrganizationService(DataContext context)
        {
            _context = context;
        }
        public async Task<List<Organization>> AddOrganization(Organization organization)
        {
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();
            return Organizations;
        }

        public async Task<List<Organization>?> DeleteOrganization(int id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization is null)
                return null;
            _context.Remove(organization);
            await _context.SaveChangesAsync();
            return Organizations;
        }

        public async Task<List<Organization>> GetAllOrganizations()
        {
            var organizations = await _context.Organizations.ToListAsync();
            return organizations;
        }

        public async Task<List<Organization>?> UpdateOrganization(int id, Organization request)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization is null)
                return null;

            organization.OrganizationID = request.OrganizationID;
            organization.OrganizationName = request.OrganizationName;
            organization.OrganizationType = request.OrganizationType;

            await _context.SaveChangesAsync();

            return Organizations;
        }
    }
}
