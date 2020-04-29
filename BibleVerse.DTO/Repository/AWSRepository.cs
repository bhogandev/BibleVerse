using System;
using Microsoft.AspNetCore.Builder;
using BibleVerse.DTO;
using Amazon.S3;
using Amazon.Runtime;
using Amazon.S3.Util;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.Net;
using BVCommon;

namespace BibleVerse.DTO.Repository
{
    public class AWSRepository
    {
        private readonly BVIdentityContext _context;
        private readonly IAmazonS3 _client;

        public AWSRepository(IAmazonS3 client, BVIdentityContext context)
        {
            this._context = context;
            this._client = client;
        }

        
        public async Task<ApiResponseModel> CreateUserDir(Users user)
        {
            string userDir = user.UserId.ToLower();
            ApiResponseModel apiResponse = new ApiResponseModel();
            apiResponse.ResponseErrors = new List<string>();
            apiResponse.ResponseBody = new List<string>();

            try
            {
                // Check if user folder already exists
                var listbucketRequst = new ListObjectsV2Request()
                {
                    BucketName = user.OrganizationId.ToLower()
                    , Prefix = user.UserId.ToLower()
                };

                var bucketList = await _client.ListObjectsV2Async(listbucketRequst);

                if(bucketList.KeyCount == 0)
                {
                    //create user folder
                    var userBucketRequest = new PutObjectRequest()
                    {
                        BucketName = user.OrganizationId.ToLower(),
                        Key = user.UserId.ToLower() + "/"
                    };

                    var userBucketResponse = await _client.PutObjectAsync(userBucketRequest);

                    if(userBucketResponse.HttpStatusCode == HttpStatusCode.OK)
                    {
                        //create sub folders
                        var userPubBucketRequest = new PutObjectRequest()
                        {
                            BucketName = user.OrganizationId.ToLower(),
                            Key = user.UserId.ToLower() + "/" + user.UserId.ToLower() + "_pub/"
                        };

                        var userPrivBucketRequest = new PutObjectRequest()
                        {
                            BucketName = user.OrganizationId.ToLower(),
                            Key = user.UserId.ToLower() + "/" + user.UserId.ToLower() + "_priv/"
                        };

                        var pubResponse = await _client.PutObjectAsync(userPubBucketRequest);
                        var privResponse = await _client.PutObjectAsync(userPrivBucketRequest);

                        if((pubResponse.HttpStatusCode == HttpStatusCode.OK) && (privResponse.HttpStatusCode == HttpStatusCode.OK))
                        {
                            apiResponse.ResponseMessage = "Success";
                            UserAWS useraws = new UserAWS()
                            {
                                ID = user.UserId,
                                Bucket = user.OrganizationId.ToLower(),
                                PublicDir = user.UserId.ToLower() + "/" + user.Id.ToLower() + "_pub/",
                                PrivateDir = user.UserId.ToLower() + "/" + user.Id.ToLower() + "_priv/",
                                ChangeDateTime = DateTime.Now,
                                CreateDateTime = DateTime.Now
                            };

                            // Store information in UserAWS
                            _context.UserAWS.Add(useraws);
                            _context.SaveChanges();
                        }
                    }

                    return apiResponse;
                }

            }catch(Exception ex)
            {
                apiResponse.ResponseMessage = "Failure";
                apiResponse.ResponseErrors.Add(ex.InnerException.ToString());
            }

            return apiResponse;
        }

    
        public async Task<ApiResponseModel> CreateOrgBucket(Organization org)
        {
            string orgBucket = org.OrganizationId.ToLower();
            ApiResponseModel apiResponse = new ApiResponseModel();
            apiResponse.ResponseErrors = new List<string>();
            apiResponse.ResponseBody = new List<string>();
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, orgBucket) == false)
                {
                    var putBucketRequest = new PutBucketRequest()
                    {
                        BucketName = orgBucket,
                        //BucketRegion = "us-east-1" // Eventually take out this hardcoded region and base region on user location
                    };

                    var response = await _client.PutBucketAsync(putBucketRequest);

                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        string orgInit = BVFunctions.CreateInit(org.Name);

                        //Create org Dir
                        var putdirRequest = new PutObjectRequest()
                        {
                            BucketName = orgBucket,
                            Key = org.OrganizationId + "_init/",
                            ContentBody = orgInit
                        };

                        var initResponse = await _client.PutObjectAsync(putdirRequest);

                        apiResponse.ResponseMessage = "Success";
                        //Add bucket information to tables

                        var dbOrg = from c in _context.Organization
                                    where c.OrganizationId == org.OrganizationId
                                    select c;

                        Organization realOrg = dbOrg.First();

                        realOrg.Bucket = orgBucket;
                        realOrg.ChangeDateTime = DateTime.Now;
                        _context.Organization.Update(realOrg);
                        _context.SaveChanges();
                    }
                    else
                    {
                        apiResponse.ResponseMessage = "Failure";
                        apiResponse.ResponseErrors.Add("Error on storage creation");
                        apiResponse.ResponseErrors.Add(response.ToString());
                    }


                }
                else
                {
                    apiResponse.ResponseMessage = "Failure";
                    apiResponse.ResponseErrors.Add("Org Bucket Already Exists With These Credentials");
                }
            }
            catch (Exception ex)
            {
                //Create ELog Error
                ELog e = new ELog()
                {
                    Message = ex.Message.ToString(),
                    Service = "AWS",
                    Severity = 3,
                    CreateDateTime = DateTime.Now
                };

                _context.ELogs.Add(e);
                _context.SaveChanges();

                apiResponse.ResponseMessage = "Failure";
                apiResponse.ResponseBody.Add(ex.ToString());
            }
                return apiResponse;
           
        }
        
    }
}
