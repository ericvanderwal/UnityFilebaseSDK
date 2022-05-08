using System;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;
using FileBase;


// amazon API documentation https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/S3/TS3Client.html
// filebase documentation https://docs.filebase.com/

public class UploadObjects : MonoBehaviour
{
    [SerializeField]
    private string bucketName;

    [SerializeField]
    private string keyName;

    async void Start()
    {
        // start by getting the client.
        // make sure proper credentials are included in FileBase.Paths
        AmazonS3Client client = FileBase.Utils.GetClient();

        await UploadObjectToBucket(client);
    }


    /// <summary>
    /// Upload an object by passing the bucket name, keyname and current path
    /// </summary>
    /// <param name="client"></param>
    private async Task UploadObjectToBucket(AmazonS3Client client)
    {
        try
        {
            var success =
                await FileBase.Transfer.UploadObjectToBucket(client, bucketName, keyName);

            Debug.Log(success ? "File has been uploaded" : "File upload failure");
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