// Required for Native AOT compilation
Amazon.RuntimeDependencies.GlobalRuntimeDependencyRegistry.Instance.RegisterChecksumProvider(
  new AWSSDK.Extensions.CrtIntegration.CrtChecksums()
  );

// Check arguments
if (args.Count() != 3) {
    Console.WriteLine("Usage: s3-delta-download <BUCKETNAME> <KEYPREFIX> <LOCALDIRECTORY>");
    Environment.Exit(1);
}

var s3util = new S3Util(bucketName: args[0], keyPrefix: args[1]);
await foreach (string key in s3util.ListBucketContentsAsync()) {
    //Console.WriteLine(key);
    await s3util.DownloadFile(key, args[2]);
}
