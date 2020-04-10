using BVCommon;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.Security.Policy;
using System.Net.Http;

namespace BibleVerse.DTO.Repository
{
    public class UserActionRepository
    {
        private readonly BVIdentityContext _context;

        public UserActionRepository(BVIdentityContext context)
        {
            this._context = context;
        }

        public async Task<string> CreateUserPost(PostModel newPost)
        {
            IQueryable<Posts> postID;
            bool idexists = false;
            int retryTimes = 0;

            while (idexists == false && retryTimes < 3)
            {
                //Create new PostID's
                var genPostID = BVFunctions.CreateUserID();
                postID = from c in _context.Posts
                              where c.PostId == genPostID
                              select c;

                if(postID.FirstOrDefault() == null)
                {
                    Posts userPost = new Posts()
                    {
                        PostId = genPostID,
                        Username = newPost.UserName,
                        Body = newPost.Body,
                        URL = "", //URL will get generated here at some point in future
                        CreateDateTime = DateTime.Now,
                        ChangeDateTime = DateTime.Now
                    };
                    if (userPost.Body != null)
                    {
                        _context.Posts.Add(userPost);
                        _context.SaveChanges();
                    }/*
                    else
                    {
                        userPost.Body = "this was null";
                        userPost.Username = newPost.UserName;
                        await _context.Posts.AddAsync(userPost);
                        await _context.SaveChangesAsync();
                    }
                    */
                    //Verify Post was created

                    var postCheck = from c in _context.Posts
                                    where c.PostId == userPost.PostId
                                    select c;

                    if (postCheck.FirstOrDefault() != null)
                    {
                        idexists = true;
                        return "Success";
                    } else
                    {
                        retryTimes++;
                    }
                     
                }

            }

            return "Failure";
        }
    }
}
