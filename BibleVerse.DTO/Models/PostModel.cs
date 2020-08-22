 using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace BibleVerse.DTO
{
    public class PostModel
    {
        public string UserName { get; set; }

        public string UserId { get; set; }

        public string OrganizationId { get; set; }

        public string Body { get; set; }

        public int Likes { get; set; }

        public List<CommentModel> Comments { get; set; }

        public List<UserUpload> Images { get; set; }

        public List<UserUpload> Videos { get; set; }

    }
}
