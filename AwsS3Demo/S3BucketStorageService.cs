using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AwsS3Demo
{
    public class S3BucketStorageService : IFileStorageService
    {
        private readonly string _bucketName;
        private readonly string _regionName;
        private readonly IAmazonS3 _client;

        public S3BucketStorageService(IAmazonS3 client, string bucketName, string regionName)
        {
            _bucketName = bucketName;
            _regionName = regionName;
            _client = client;
        }

        public async Task CreateBucketAsync(string bucketName)
        {
            if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName) == false)
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    BucketRegionName = _regionName,
                };

                var response = await _client.PutBucketAsync(putBucketRequest);

                var t = new
                {
                    Message = response.ResponseMetadata.RequestId,
                    Status = response.HttpStatusCode
                };
            }
        }

        public async Task<string> StoreFileAsync(FileInfo file)
        {
            // Create Bucket if not created already
            await CreateBucketAsync(_bucketName);

            var fileTransferUtility = new TransferUtility(_client);

            // Upload file with the name it has (NOT RECOMMENDED)
            await fileTransferUtility.UploadAsync(file.FullName, _bucketName);

            // Upload file with our custom name
            var newFileName = Guid.NewGuid().ToString();
            newFileName = $"{newFileName}{file.Extension}";
            await fileTransferUtility.UploadAsync(file.FullName, _bucketName, newFileName);

            var imageUrl = $"https://{_bucketName}.s3.{_regionName}.amazonaws.com/{newFileName}";

            return imageUrl;
        }

        public Task DeleteFileAsync(string key)
        {
            return Task.CompletedTask;
        }


        public Task GetFileAsync(string key, DirectoryInfo destinationFolder)
        {
            return Task.CompletedTask;

        }

        public void Dispose()
        {
            _client.Dispose();
        }

    }
}
