using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BibleVerse.DTO
{
    public class ELog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ElogID { get; set; }

        public int Severity { get; set; }

        public string Service { get; set; }

        public string Message { get; set; }

        public DateTime CreateDateTime { get; set; }

        public static string GenerateElogFromException(Exception x)
        {

            return String.Format("Message: {0}\n InnerException: {1}\n StackTrace: {2}\n Source: {3}", x.Message, x.InnerException, x.StackTrace, x.Source);
        }
    }
}
