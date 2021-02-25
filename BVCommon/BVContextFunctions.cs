using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BVCommon
{
    public class BVContextFunctions
    {
        private static BibleVerse.DALV2.BVIdentityContext context;

        public static bool WriteToDb(string entityType, string entityObject, BibleVerse.DALV2.BVIdentityContext _context)
        {
            //Determine Entity Type
            Type t = BVContextHelper.GetType(entityType);

            bool result = BVContextHelper.WriteObject(_context, t, entityObject);

            return result;
            
        }

        public static bool DeleteFromDb(string entityType, string entityObject, BibleVerse.DALV2.BVIdentityContext _context)
        {
            //Determine Entity Type
            Type t = BVContextHelper.GetType(entityType);

            bool result = BVContextHelper.DeleteObject(context, t, entityObject);

            return result;
        }

        public static bool UpdateToDb(string entityType, string entityObject, BibleVerse.DALV2.BVIdentityContext _context)
        {
            //Determine Entity Type
            Type t = BVContextHelper.GetType(entityType);

            bool result = BVContextHelper.UpdateObject(_context, t, entityObject);

            return result;
        }

    }
}
