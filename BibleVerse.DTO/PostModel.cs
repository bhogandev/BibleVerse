using System;
using System.Collections.Generic;

namespace BibleVerse.DTO
{
    public class PostModel
    {
        public  string UserName { get; set; }

        public string Body { get; set; }

        public int Likes { get; set; }

        public List<CommentModel> Comments { get; set; }

        public List<string> Images { get; set; }

        public List<string> Videos { get; set; }
    }
}
