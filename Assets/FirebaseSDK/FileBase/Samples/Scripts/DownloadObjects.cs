using System;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;
using FileBase;


// amazon API documentation https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/S3/TS3Client.html
// filebase documentation https://docs.filebase.com/

public class DownloadObjects : MonoBehaviour
{
    [SerializeField]
    private string bucketName;

    [SerializeField]
    private string objectKey;

    async void Start()
    {
        // start by getting the client.
        // make sure proper credentials are included in FileBase.Paths
        AmazonS3Client client = FileBase.Utils.GetClient();

        await DownloadObjectFromBucket(client);
    }


    /// <summary>
    /// Download object from bucket by object key name and bucket name
    /// </summary>
    /// <param name="client"></param>
    private async Task DownloadObjectFromBucket(AmazonS3Client client)
    {
        try
        {
            FileBaseObject fileBaseObject =
                await FileBase.Transfer.DownloadObjectFromBucket(client, bucketName, objectKey, DisplayProgress);

            if (fileBaseObject != null)
            {
                Debug.Log("File has been downloaded to: " + fileBaseObject.LocalPath);
            }
            else
            {
                Debug.Log("No object was downloaded");
            }
        }

        // always remember to wrap calls in try catch statements in case an uncaught exception occurs
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// Display the progress of the download
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void DisplayProgress(object sender, WriteObjectProgressArgs e)
    {
        Debug.Log("Progress : " + e.PercentDone);
    }
}