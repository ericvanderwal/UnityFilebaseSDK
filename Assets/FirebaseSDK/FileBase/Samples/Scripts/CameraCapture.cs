using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    [SerializeField]
    private string bucketName;

    [SerializeField]
    private Button button;

    [SerializeField]
    private TMP_InputField inputField;


    [Header("Debug Only")]
    [SerializeField]
    private string objectKey;

    [SerializeField]
    private string savePath;

    private AmazonS3Client _client;

    void Start()
    {
        // build client using api key
        _client = FileBase.Utils.GetClient();

        // listen for click button
        button.onClick.AddListener(TakePicture);
    }

    private void TakePicture()
    {
        TakePictureAsync();
    }


    /// <summary>
    /// Save a picture from the main camera
    /// </summary>
    private async Task TakePictureAsync()
    {
        // set a name for the object key
        objectKey = "Obj_" + System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";

        // generate a path using a bucket name and object key (filename) to find it easily later
        savePath = FileBase.Utils.GeneratePath(bucketName, objectKey);

        // capture the screen
        ScreenCapture.CaptureScreenshot(savePath, 2);


        // await to allow a frame to pass and image to be save before moving forward
        await Task.Delay(1);

        SetText("Object internally saved to: " + savePath);


        await UploadObjectToBucket();
    }

    /// <summary>
    /// Upload object to filebase
    /// </summary>
    private async Task UploadObjectToBucket()
    {
        // disable the button to prevent multiple clicks and spamming
        button.interactable = false;

        SetText("Trying to upload to Filebase");


        // use a try catch statement in case of failure, such as inability to log in to filebase
        try
        {
            // upload file to filebase bucket
            var success = await FileBase.Transfer.UploadObjectToBucket(_client, bucketName, objectKey);

            SetText(success ? "File has been uploaded" : "File upload failure");


            var publicPath = await FileBase.Utils.GetBaseUrl(_client, bucketName, objectKey);
            SetText("Object can be found at: " + publicPath);
        }
        catch (Exception e)
        {
            SetText("Error: " + e);
        }

        await Task.Delay(10000);
        button.interactable = true;
    }

    private void SetText(string text)
    {
        inputField.text = text;
        // Debug.Log(text);
    }
}