# Serverless API usage metrics analyzer

Presentation: https://docs.google.com/presentation/d/1OMnQbf1gFVAMKgd7biBc2hrKIi7LFYdXwwF7SPM2ulk/edit#slide=id.g37a4323b0_2_0

In this challenge we're going to learn how to trigger an [AWS Lambda](https://aws.amazon.com/lambda/) function when data is put in [Amazon Kinesis Stream](https://aws.amazon.com/kinesis/). That [AWS Lambda](https://aws.amazon.com/lambda/) function will read the Kinesis stream, decode it, read the XML documents, transform them to be in a CSV format, and forward the data to a new Kinesis stream.

## Prerequisites

1. AWS Tools
    1. Sign-up for an [AWS account](https://aws.amazon.com)
    2. Install [AWS CLI](https://aws.amazon.com/cli/)
2. .NET Core
    1. Install [.NET Core 1.0](https://www.microsoft.com/net/core) **(NOTE: You MUST install 1.0. Do NOT install 1.1 or later!)**
    2. Install [Visual Studio Code](https://code.visualstudio.com/)
    3. Install [C# Extension for VS Code](https://code.visualstudio.com/Docs/languages/csharp)
3. AWS C# Lambda Tools
    1. [Install](https://aws.amazon.com/blogs/developer/creating-net-core-aws-lambda-projects-without-visual-studio/) the templates to the `dotnet` tool for AWS Lambda: `dotnet new -i Amazon.Lambda.Templates::*`
    
## Level 0: Use the client app in the repository, to submit data to your input Kinesis Stream
In the [repository](https://github.com/LambdaSharp/April2017-KinesisAnalyticsChallenge), find the client directory, modify it to write to your Kinesis Stream and start submitting XML data.

** ACCEPTANCE TEST:** You see traffic coming through in your Kinesis Stream

## Level 1: Hook up a Lambda function to your input Kinesis Stream, and transform the received XML data to CSV data
In the [repository](https://github.com/LambdaSharp/April2017-KinesisAnalyticsChallenge), find the `xml2csv` lambda function, and modify it to transform the data from XML to CSV.

** ACCEPTANCE TEST:** You see invocations of your Lambda function.

## Level 2: Push the csv records to a new Kinesis Stream that will be consumed by Kinesis Analytics
Since Kinesis Analytics works with CSV, push the transformed data to that new stream

** ACCEPTANCE TEST:** You see data coming through on your csv destination Kinesis Stream.

## Level 3: Create a Kinesis Analytics App with the CSV based Kinesis Stream as input
Go to the AWS Console in the Kinesis Analytics section and create an application, set its input to the Kinesis Stream that is carrying CSV data, and make the Kinesis Analytics App identify its schema.

** ACCEPTANCE TEST:** You create a Kinesis Analytics App, and your csv based Kinesis Stream is configured as its input.

## Level 4: Make your Kinesis Analytics App analyze its input Kinesis Stream such that you can find the Top 5 API calls in a 30 seconds window.
Write Kinesis Analytics SQL to find the 5 most popular API calls during 30 second windows.

** ACCEPTANCE TEST:** You can operate on the API calls metrics with SQL in your Kinesis Analytics app, and you create the right output streams and pumps to be able to compute the top 5 most called API methods.

## Boss Level: Detect abnormal API calls that take too long.
Modify your Kinesis Analytics App to be able to detect abnormally slow API calls.

** ACCEPTANCE TEST:** You can see the most anomalous calls in descending order.
