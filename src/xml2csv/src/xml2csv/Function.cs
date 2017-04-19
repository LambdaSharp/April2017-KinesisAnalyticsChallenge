using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using System.Xml.Linq;

using System.Threading;
using System.Threading.Tasks;

using Amazon.Kinesis;
using Amazon.Kinesis.Model;

using Amazon.Lambda.Core;
using Amazon.Lambda.KinesisEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace xml2csv {
    public class Function {
        //--- Fields ---
        private static IAmazonKinesis _klient = new AmazonKinesisClient();
        private static Random _random = new Random((int)DateTime.UtcNow.Ticks);

        //--- Methods ---
        public void FunctionHandler(KinesisEvent kinesisEvent, ILambdaContext context) {
            context.Logger.LogLine($"Beginning to process {kinesisEvent.Records.Count} records...");
            var streamName = "YOUR STREAM HERE";
            var rows = new List<string>();

            foreach (var record in kinesisEvent.Records) {
                string recordData = GetRecordContents(record.Kinesis);
                var doc = XElement.Parse(recordData);
                var row = string.Join(",", new [] {
                    doc.Element("method-name")?.Value,
                    doc.Element("elapsed-ms")?.Value,
                    doc.Element("timestamp")?.Value,
                    doc.Element("customer-id")?.Value
                });
                rows.Add(row);
            }
            var skip = 0;
            var CHUNK_SIZE = 25;
            while(true) {
                var chunk = rows.Skip(skip).Take(CHUNK_SIZE).ToArray();
                if(!chunk.Any()) {
                    break;
                }
                var t = _klient.PutRecordsAsync(new PutRecordsRequest {
                    Records = chunk.Select(row => {
                        using(var m = new MemoryStream(Encoding.UTF8.GetBytes(row + "|"))) {
                            return new PutRecordsRequestEntry {
                                Data = m,
                                PartitionKey = Guid.NewGuid().ToString()
                            };
                        }
                    }).ToList(),
                    StreamName = streamName
                }, CancellationToken.None);
                Task.WaitAll(new [] { t });
                skip += CHUNK_SIZE;
                var response = t.Result;
                if(response.FailedRecordCount > 0) {
                    foreach(var record in response.Records) {
                        if(!string.IsNullOrEmpty(record.ErrorCode) || !string.IsNullOrEmpty(record.ErrorMessage)) {
                            context.Logger.LogLine($"errorCode={record.ErrorCode}, errorMessage={record.ErrorMessage}");
                        }
                    }
                } else {
                    context.Logger.LogLine($"awsRequestId={context.AwsRequestId}, successfully flushed all the messages");
                }
                Thread.Sleep(1000);
            }
            context.Logger.LogLine("Stream processing complete.");
        }

        private string GetRecordContents(KinesisEvent.Record streamRecord) {
            using (var reader = new StreamReader(streamRecord.Data, Encoding.ASCII)) {
                return reader.ReadToEnd();
            }
        }
    }
}
