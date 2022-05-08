using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;


// amazon API documentation https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/S3/TS3Client.html
// filebase documentation https://docs.filebase.com/

public class GetAllObjects : MonoBehaviour
{
    [SerializeField]
    private string bucketName;

    async void Start()
    {
        // start by getting the client.
        // make sure proper credentials are included in FileBase.Paths
        AmazonS3Client client = FileBase.Utils.GetClient();

        await GetAllObjectsAndPrint(client);
    }

    // private static void DisplayProgress(object sender, WriteObjectProgressArgs e)
    // {
    //     Debug.Log("Progress : " + e.PercentDone);
    // }


    /// <summary>
    /// Get all objects listed in bucket define by name. Debug log each object key to the unity debug console.
    /// </summary>
    /// <param name="client"></param>
    private async Task GetAllObjectsAndPrint(AmazonS3Client client)
    {
        try
        {
            List<S3Object> BucketObjects = await FileBase.Bucket.GetAllObjectsInBucket(client, bucketName);


            // make sure the bucket is not empty
            if (BucketObjects == null || BucketObjects.Count == 0)
            {
                Debug.Log("No objects were found in this bucket");
                return;
            }


            // debug log all the objects stored in this bucket by key
            for (var index = 0; index < BucketObjects.Count; index++)
            {
                Debug.Log("Object " + index + " : " + BucketObjects[index].Key);
            }
        }

        // always remember to wrap calls in try catch statements in case an uncaught exception occurs
        catch (AggregateException aggregateException)
        {
            foreach (var innerException in aggregateException.InnerExceptions)
            {
                Debug.LogError(innerException.Message);
            }
        }
    }
}