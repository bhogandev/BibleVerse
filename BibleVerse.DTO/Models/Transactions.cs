using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BibleVerse.DTO
{
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionUID { get; set; }

        public string ClientDevice { get; set; }

        public string ClientAddress { get; set; }

        public string ClientIdentity { get; set; }

        public string ClientVersion { get; set; }

        public string TransactionType { get; set; }

        public string TransactionController { get; set; }

        public string TransactionPayload { get; set; }

        public DateTime TransactionTime { get; set; }
    } 
}
