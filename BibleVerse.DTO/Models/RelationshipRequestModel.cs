using System;
namespace BibleVerse.DTO
{
    public class RelationshipRequestModel
    {
        public string FirstUser { get; set; }

        public string SecondUser { get; set; }

        public string RelationshipType { get; set; }

        public string RequestType { get; set; }
    }
}
