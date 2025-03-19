# s3-delta-download

## Overview

Since the AWS CLI 's3 sync' command
[still doesn't support](https://github.com/aws/aws-cli/issues/4240) using a non-directory
key prefix, I created this CLI tool to quickly fetch my latest cloudfront logs like this:


    AWS_PROFILE=myprofile AWS_REGION=us-east-2 ./s3-delta-download \
        my-cloudfront-logs-bucket web/CF34I1N71LBO8.2025-03 /tmp/s3logs
    Downloading: web/CF34I1N71LBO8.2025-03-17-21.b3ff36e3.gz
    Downloading: web/CF34I1N71LBO8.2025-03-17-21.cf1a42c7.gz
    Downloading: web/CF34I1N71LBO8.2025-03-17-22.05e8f2b2.gz
    ...

The above command will fetch all keys in the bucket with a prefix of
`web/CF34I1N71LBO8/2025-03`, meaning all files in March 2025.

The tool will only download files that don't exist in the local directory. In
the above example, I already had files from March 1 to 16 downloaded, so they
are skipped.

The tool does atomic renames of files after a complete download, so this
existence check is safe, assuming the files in S3 are immutable.

## Installation

1) This needs the .NET 8 SDK (or later) to build
2) If you want to compile to native code for fast startup, ensure you have the [native toolchain prerequistes](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=windows%2Cnet8#prerequisites) installed
3) If you don't want to compile to native code, remove the `<PublishAot>true</PublishAot>` from the .csproj file
4) Run `dotnet publish` to compile a release build.
5) Run the binary from here or copy it: `./bin/Release/net8.0/linux-x64/publish/s3-delta-download`

## Possible Enhancements

* The [ListObjectsV2](https://docs.aws.amazon.com/AmazonS3/latest/API/API_ListObjectsV2.html)
`StartAfter` parameter could be used instead of a KeyPrefix.  Instead of only
downloading logs in March 2025, this would download all logs >= March 2025.

* Create a directory structure when downloading, instead of assuming all files
  go directly into localDir.
