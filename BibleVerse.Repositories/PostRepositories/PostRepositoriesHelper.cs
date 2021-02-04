using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Repositories.PostRepositories
{
    public class PostRepositoriesHelper
    {
        private static string StackTrace = "BibleVerse.Repositories -> PostRepositories -> PostRepositoriesHelper";

        #region Public Methods
        
        public static string DeleteUserPost(BibleVerse.DTO.Posts _removablePost, BibleVerse.DTO.Users _user)
        {
            if (_removablePost.Username == _user.UserName)
            {
                _removablePost.IsDeleted = true;

                string entType = _removablePost.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(_removablePost);

                bool postIsDeleted = BVCommon.BVContextFunctions.UpdateToDb(entType, entObj);

                return postIsDeleted ? "Success" : "Failure";
            }
            else
            {
                return "You do not have access to delete this post.";
            }
        }

        #endregion

        #region Private Methods
        #endregion
    }
}
