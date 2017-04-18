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
            var streamName = "YOUR STREAM NAME HERE";

            foreach (var record in kinesisEvent.Records) {
                context.Logger.LogLine($"Event ID: {record.EventId}");
                context.Logger.LogLine($"Event Name: {record.EventName}");

                string recordData = GetRecordContents(record.Kinesis);
                context.Logger.LogLine($"Record Data:");
                context.Logger.LogLine(recordData);
            }
        }

        private string GetRecordContents(KinesisEvent.Record streamRecord) {
            using (var reader = new StreamReader(streamRecord.Data, Encoding.ASCII)) {
                return reader.ReadToEnd();
            }
        }
    }
}