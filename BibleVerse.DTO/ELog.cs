using System;
namespace BibleVerse.DTO
{
    public class ELog
    {
        public int ElogID { get; set; }

        public int Severity { get; set; }

        public string Service { get; set; }

        public string Message { get; set; }

        public DateTime CreateDateTime { get; set; }

    }
}
