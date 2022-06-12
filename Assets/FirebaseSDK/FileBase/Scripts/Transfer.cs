using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;

namespace FileBase
{
    public static class Transfer
    {
        //todo add ability to overwrite object
        /// <summary>
        /// Download an object from filebase by passing the bucket and object key (name).
        /// Returns a filebase object and stores the file at filebase.Localpath
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName"></param>
        /// <param name="objectKeyName"></param>
        /// <param name="displayProgress"></param>
        /// <returns></returns>
        public static async Task<FileBaseObject> DownloadObjectFromBucket(AmazonS3Client client, string bucketName,
            string objectKeyName, EventHandler<WriteObjectProgressArgs> displayProgress = null)
        {
            if (client == null)
            {
                Debug.LogError("Client is required to download an object");
                return null;
            }

            if (string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(objectKeyName))
            {
                Debug.LogError("Bucket name and object key name is required to download an object");
                return null;
            }

            // set paths. Return null if fail.
            var pathFile = FileBase.Utils.GeneratePath(bucketName, objectKeyName);
            if (string.IsNullOrEmpty(pathFile)) return null;

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

                // download object and update display progress
                if (displayProgress != null)
                {
                    response.WriteObjectProgressEvent += displayProgress;
                }

                // do the actual file downloading
                await response.WriteResponseStreamToFileAsync(pathFile, false, System.Threading.CancellationToken.None);

                FileBaseObject fileBaseObject = new FileBaseObject();
                fileBaseObject.Cid = response.Metadata["x-amz-meta-cid"];
                fileBaseObject.Url = Paths.FileBase + fileBaseObject.Cid;
                fileBaseObject.LocalPath = pathFile;
                fileBaseObject.BucketName = response.BucketName;
                fileBaseObject.Key = response.Key;
                fileBaseObject.ETag = response.ETag;
                fileBaseObject.LastModified = response.LastModified;

                return fileBaseObject;
            }

            // catch any errors that occur
            catch (AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    Debug.LogError(innerException.Message);
                }

                return null;
            }
        }


        /// <summary>
        /// Upload object to bucket by objectKeyName and bucket.
        /// Object should be stored at Application.persistentDataPath + "/" + bucketName + "/" + object key name
        /// Alternatively use UploadObjectToBucketByPath to define a custom path
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName"></param>
        /// <param name="objectKeyName"></param>
        /// <returns></returns>
        public static async Task<bool> UploadObjectToBucket(AmazonS3Client client, string bucketName,
            string objectKeyName)
        {
            var path = Application.persistentDataPath + "/" + bucketName;
            var pathFile = path + "/" + objectKeyName;

            // check directory and path exist before trying to upload
            if (!Directory.Exists(path))
            {
                Debug.LogError("Directory not found at: " + path);
                return false;
            }

            if (!File.Exists(pathFile))
            {
                Debug.LogError("File does not exists. File: " + pathFile);
                return false;
            }

            var success = await UploadObjectToBucketByPath(client, bucketName, objectKeyName, pathFile);
            return success;
        }


        /// <summary>
        /// Upload an object by passing a custom path. A destination object key name and bucket are required.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bucketName"></param>
        /// <param name="objectKeyName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<bool> UploadObjectToBucketByPath(AmazonS3Client client, string bucketName,
            string objectKeyName, string path)
        {
            if (client == null)
            {
                Debug.LogError("A valid client is required to upload a file. Are your credentials correct?");
            }

            // check if file exists before read
            if (!File.Exists(path))
            {
                Debug.LogError("File does not exists. File name at : " + path);
                return false;
            }

            try
            {
                // Create a GetObject request
                PutObjectRequest putObjectRequest = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    FilePath = path,
                };

                PutObjectResponse response = await client.PutObjectAsync(putObjectRequest);
                
                return true;
            }

            catch (AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.InnerExceptions)
                {
                    Debug.LogError(innerException.Message);
                }

                return false;
            }
        }
    }
}