using Amazon.S3;
using Amazon.S3.Model;

public class S3Util
{
    private readonly AmazonS3Client _client = new();
    private readonly string _bucketName;
    private readonly string _keyPrefix;

    public S3Util(string bucketName, string keyPrefix) {
        _bucketName = bucketName;
        _keyPrefix = keyPrefix;
    }

    // Call The ListObjectsV2 API and iterate over each key string
    public async IAsyncEnumerable<string> ListBucketContentsAsync() {
        var request = new ListObjectsV2Request {
            BucketName = _bucketName,
            Prefix = _keyPrefix,
            //MaxKeys = 5,
        };

        ListObjectsV2Response response;
        do {
            response = await _client.ListObjectsV2Async(request);

            foreach (var obj in response.S3Objects) {
                yield return obj.Key;
            }

            request.ContinuationToken = response.NextContinuationToken;
        } while (response.IsTruncated);
    }

    // Download file if it doesn't already exist
    public async Task DownloadFile(string key, string localDir) {
        string tempPath = GetTempFilePath(localDir);
        string finalPath = GetLocalFilePath(localDir, key);
        if (File.Exists(finalPath)) {
            return;
        }

        Console.WriteLine("Downloading: " + key);
        //Console.WriteLine("Temp File: " + tempPath);

        var request = new GetObjectRequest {
            BucketName = _bucketName,
            Key = key,
        };

        using GetObjectResponse response = await _client.GetObjectAsync(request);
        await response.WriteResponseStreamToFileAsync(tempPath, true, CancellationToken.None);
        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK) {
            throw new Exception("bad HttpStatusCode");
        }
        File.Move(tempPath, finalPath, true);
    }

    private static string GetTempFilePath(string localDir) {
        // Use a guid so multiple concurrent processes are safe
        return Path.Join(localDir,  ".s3-syncer.tmp." + Guid.NewGuid().ToString());
    }

    private static string GetLocalFilePath(string localDir, string key) {
        // TODO: If you want to replicate a directory structure in the localDir,
        // this code will need to change
        string[] parts = key.Split("/");
        return Path.Join(localDir, parts.Last());
    }
}
