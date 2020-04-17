using System;
using Microsoft.AspNetCore.Builder;
using BibleVerse.DTO;
using Amazon.S3;
using Amazon.Runtime;
using Amazon.S3.Util;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace BibleVerse.DTO.Repository
{
    public class AWSRepository
    {
        private readonly BVIdentityContext _context;
        private readonly IAmazonS3 _client;

        public AWSRepository(IAmazonS3 client, BVIdentityContext context)
        {
            this._context = context;
            this._client = client
        }

        
        public async Task<ApiResponseModel> CreateOrgBucket(Organization org)
        {
            IQueryable<string> accesskey = from c in _context.SiteConfigs
                                           where (c.Service == "AWS" && c.Name == "AccessKey")
                                           select c.Value;

            IQueryable<string> secretKey = from c in _context.SiteConfigs
                                           where (c.Service == "AWS" && c.Name == "SecretKey")
                                           select c.Value;

            BasicAWSCredentials creds = new BasicAWSCredentials(accesskey.ToString(), secretKey.ToString());
            
            string orgBucket = org.OrganizationId + "_" + org.Name.Trim().ToUpper();


            if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, orgBucket) == false)
            {
                var putBucketRequest = new PutBucketRequest()
                {
                    BucketName = orgBucket,
                    BucketRegion = "us-east-1"
                };

                var response = await _client.PutBucketAsync(putBucketRequest);
            }
        }
        
    }
}
