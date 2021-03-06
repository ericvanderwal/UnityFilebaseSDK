using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;

namespace FileBase
{
    public static class Utils
    {
        
        /// <summary>
        /// Get S3 client setup for filebase using information stored in Filebase.Paths
        /// </summary>
        /// <returns></returns>
        public static AmazonS3Client GetClient()
        {
            AmazonS3Config s3Config = new AmazonS3Config
            {
                ServiceURL = FileBase.Paths.ServiceURL,
            };

            AmazonS3Client s3Client = new AmazonS3Client(FileBase.Paths.AccessKey, FileBase.Paths.SecretKey, s3Config);
            return s3Client;
        }

        /// <summary>
        /// Get CID by bucket and object key name
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName"></param>
        /// <param name="objectKeyName"></param>
        /// <returns></returns>
        public static async Task<string> GetCid(AmazonS3Client client, string bucketName, string objectKeyName)
        {
            try
            {
                // Create a GetObject request
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKeyName,
                };

                // Issue request and remember to dispose of the response
                using GetObjectResponse response = await client.GetObjectAsync(request);

                //Get base URI
                string cid = response.Metadata["x-amz-meta-cid"];
                return cid;
            }

            // catch any errors that occur
            catch (AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    Debug.LogError(innerException.Message);
                }
            }

            return null;
        }

        /// <summary>
        /// Get base url of object by bucket name and object key name
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName"></param>
        /// <param name="objectKeyName"></param>
        /// <returns></returns>
        public static async Task<string> GetBaseUrl(AmazonS3Client client, string bucketName, string objectKeyName)
        {
            string cid = await GetCid(client, bucketName, objectKeyName);
            return string.IsNullOrEmpty(cid) ? null : Paths.FileBase + cid;
        }
        
        /// <summary>
        /// Generate a path by bucket name and object key name including the application persistant data path.
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKeyName"></param>
        /// <returns></returns>
        public static string GeneratePath(string bucketName, string objectKeyName)
        {
            var path = Application.persistentDataPath + "/" + bucketName;
            var pathFile = path + "/" + objectKeyName;
            
            // create a directory if it doesnt exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log("New directory created");
            }

            
            // check if file exists before write
            if (File.Exists(pathFile))
            {
                Debug.LogError("File already exists at " + pathFile);
                return null;
            }

            return pathFile;
        }
        
    }
}