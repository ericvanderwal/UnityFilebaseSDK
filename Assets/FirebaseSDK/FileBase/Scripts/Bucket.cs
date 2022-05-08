using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;
using System.Linq;

namespace FileBase
{
    public static class Bucket
    {
        /// <summary>
        /// Get a list of buckets
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<List<S3Bucket>> GetAllBuckets(AmazonS3Client client)
        {
            if (client == null)
            {
                Debug.LogError("Client name is required");
                return null;
            }

            ListBucketsResponse response = await client.ListBucketsAsync();
            return response.Buckets;
        }

        /// <summary>
        /// Get a list of all objects within a specific bucket by name. Return null for no results
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public static async Task<List<S3Object>> GetAllObjectsInBucket(AmazonS3Client client, string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName) || client == null)
            {
                Debug.LogError("Data is missing. Did you add client and bucket name?");
                return null;
            }

            if (await CheckBucketExists(client, bucketName)) return null;

            ListObjectsV2Request listObjectsV2Request = new ListObjectsV2Request();
            listObjectsV2Request.BucketName = bucketName;
            ListObjectsV2Response response = await client.ListObjectsV2Async(listObjectsV2Request);

            return response.S3Objects;
        }

        /// <summary>
        /// Delete bucket by name. Return true if success.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteBucket(AmazonS3Client client, string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName) || client == null)
            {
                Debug.LogError("Data is missing. Did you add client and bucket name?");
                return false;
            }

            if (await CheckBucketExists(client, bucketName)) return false;

            DeleteBucketResponse response = await client.DeleteBucketAsync(bucketName);
            if (response.HttpStatusCode == HttpStatusCode.NoContent) return true;
            return false;
        }

        /// <summary>
        /// Check if bucket exists by name. Returns true if exists.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public static async Task<bool> CheckBucketExists(AmazonS3Client client, string bucketName)
        {
            // check bucket exists to avoid throwing an error
            List<S3Bucket> buckets = await GetAllBuckets(client);
            S3Bucket name = buckets.FirstOrDefault(x => x.BucketName.Contains(bucketName));
            if (name == null)
            {
                Debug.LogError("No bucket with this name exists");
                return true;
            }

            return false;
        }
    }
}