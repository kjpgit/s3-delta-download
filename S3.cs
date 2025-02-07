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

    public async Task DownloadFile(string key, string localDir) {
        string tempPath = GetTempFile(localDir);
        string finalPath = GetLocalFile(localDir, key);
        if (File.Exists(finalPath)) {
            return;
        }


        Console.WriteLine("Downloading: " + key);

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

    private static string GetTempFile(string localDir) {
        return localDir + "/.s3-syncer.tmp";
    }

    private static string GetLocalFile(string localDir, string key) {
        string[] parts = key.Split("/");
        return localDir + "/" + parts.Last();
    }

}
