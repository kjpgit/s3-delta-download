foreach (string arg in args) {
    Console.WriteLine(arg);
}

//AWSSDK.Extensions.CrtIntegration.CrtChecksums
Amazon.RuntimeDependencies.GlobalRuntimeDependencyRegistry.Instance.RegisterChecksumProvider(
  new AWSSDK.Extensions.CrtIntegration.CrtChecksums()
        );

var s3util = new S3Util(args[0], args[1]);
await foreach (string key in s3util.ListBucketContentsAsync()) {
    Console.WriteLine(key);
    await s3util.DownloadFile(key, args[2]);
}
