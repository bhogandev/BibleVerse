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
                        apiResponse.ResponseMessage = "Success";
                        //Add bucket information to tables

                        var dbOrg = from c in _context.Organization
                                    where c.OrganizationId == org.OrganizationId
                                    select c;

                        Organization realOrg = dbOrg.First();

                        realOrg.Bucket = orgBucket;
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
                apiResponse.ResponseMessage = "Failure";
                apiResponse.ResponseBody.Add(ex.ToString());
            }
                return apiResponse;
           
        }
        
    }
}
