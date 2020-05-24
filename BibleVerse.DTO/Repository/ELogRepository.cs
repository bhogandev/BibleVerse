using System;
using System.Threading.Tasks;

namespace BibleVerse.DTO.Repository
{
    public class ELogRepository
    {
        private readonly BVIdentityContext _context;

        public ELogRepository(BVIdentityContext context)
        {
            this._context = context;
        }

        public async Task<string> LogError(string service, int severity, string message)
        {
            string returnString = "";

            try
            {
                ELog error = new ELog()
                {
                    Service = service,
                    Severity = severity,
                    Message = message,
                    CreateDateTime = DateTime.Now
                };

                _context.ELogs.Add(error);
                _context.SaveChanges();
                returnString = "Success";
            }catch(Exception ex)
            {
                returnString = ex.ToString();
            }

            return returnString;
        }
    }
}
