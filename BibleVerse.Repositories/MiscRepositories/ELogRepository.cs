using BibleVerse.DALV2;
using BibleVerse.DTO;
using System;
using System.Threading.Tasks;

namespace BibleVerse.Repositories
{
    public class ELogRepository
    {
        private readonly BibleVerse.DALV2.BVIdentityContext _context;
        protected string StackTraceRoot = "BibleVerse.DTO -> Repository -> ELogRepository: ";

        public ELogRepository(BibleVerse.DALV2.BVIdentityContext context)
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
