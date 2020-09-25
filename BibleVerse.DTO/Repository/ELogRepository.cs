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

        public async Task<string> StoreELog(ELog log)
        {
            string returnString = string.Empty;

            try
            {
                log.CreateDateTime = DateTime.UtcNow; //Update CreateDatetime In Log
                _context.ELogs.Add(log);
                _context.SaveChanges();
            }catch(Exception ex)
            {
                returnString = ex.ToString();
            }

            returnString = "Success";
            return returnString;
        }
    }
}
