using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BibleVerse.Repositories.AWSRepositories
{
    public class AWSHelper
    {
        private static string StackTrace = "BibleVerse.Repositories -> AWSRepositories -> AWSHelper";


        public static bool UploadPhotoToDb(BibleVerse.DTO.Photos _photo, BibleVerse.DALV2.BVIdentityContext _context)
        {
            try
            {
                if (_photo != null)
                {
                    
                    string entType = _photo.GetType().Name;

                    string entObj = JsonConvert.SerializeObject(_photo);

                    bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);

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

                string entType = exception.GetType().Name;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);

                return false;
            }
        }

        public static bool UploadVideoToDb(BibleVerse.DTO.Videos _video, BibleVerse.DALV2.BVIdentityContext _context)
        {
            try
            {
                if (_video != null)
                {

                    string entType = _video.GetType().Name;

                    string entObj = JsonConvert.SerializeObject(_video);

                    bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);

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

                string entType = exception.GetType().Name;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);

                return false;
            }
        }

        public static bool CreateNewPostRelationship(BibleVerse.DTO.PostsRelations _newPostRelationship, BibleVerse.DALV2.BVIdentityContext _context)
        {
            try
            {
                if (_newPostRelationship != null)
                {

                    string entType = _newPostRelationship.GetType().Name;

                    string entObj = JsonConvert.SerializeObject(_newPostRelationship);

                    bool result = BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);

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

                string entType = exception.GetType().Name;

                string entObj = JsonConvert.SerializeObject(exception);

                BVCommon.BVContextFunctions.WriteToDb(entType, entObj, _context);

                return false;
            }
        }
    }
}
