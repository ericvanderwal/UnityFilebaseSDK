using System;

namespace FileBase
{
    [Serializable]
    public class FileBaseObject
    {
        public string Cid;
        public string LocalPath;
        public string Url;
        public string BucketName;
        public string ContentType;
        public string Key;
        public string ETag;
        public DateTime LastModified;
    }
}