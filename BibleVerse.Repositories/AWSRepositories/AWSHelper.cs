using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Repositories.AWSRepositories
{
    public class AWSHelper
    {
        private static string StackTrace = "BibleVerse.Repositories -> AWSRepositories -> AWSHelper";


        public static bool UploadPhotoToDb(BibleVerse.DTO.Photos _photo)
        {
            try
            {
                if (_photo != null)
                {
                    
                    string entType = _photo.GetType().FullName;

                    string entObj = JsonConvert.SerializeObject(_photo);

                    bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                    return result;
                }
                else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return false;
            }
        }

        public static bool UploadVideoToDb(BibleVerse.DTO.Videos _video)
        {
            try
            {
                if (_video != null)
                {

                    string entType = _video.GetType().FullName;

                    string entObj = JsonConvert.SerializeObject(_video);

                    bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                    return result;
                }
                else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return false;
            }
        }

        public static bool CreateNewPostRelationship(BibleVerse.DTO.PostsRelations _newPostRelationship)
        {
            try
            {
                if (_newPostRelationship != null)
                {

                    string entType = _newPostRelationship.GetType().FullName;

                    string entObj = JsonConvert.SerializeObject(_newPostRelationship);

                    bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                    return result;
                }
                else
                {
                    Exception ex = new Exception()
                    {
                        Source = "Null Value Provided For Data Function."
                    };

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                BibleVerse.DTO.ELog exception = new DTO.ELog()
                {
                    Message = ex.Message,
                    Service = StackTrace,
                    Severity = 1,
                    CreateDateTime = DateTime.Now
                };

                string entType = exception.GetType().FullName;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj);

                return false;
            }
        }
    }
}
