using System;
using System.IO;
using System.Text;
using System.Linq;

using System.Threading;
using System.Threading.Tasks;

using System.Xml.Linq;

using Amazon.Kinesis;
using Amazon.Kinesis.Model;

namespace LambdaSharp.Client {
    class Program {

        //---- Methods ---
        static void Main(string[] args) {
            if(args.Length < 1) {
                Console.WriteLine(@"Usage: dotnet run {STREAM_NAME}");
                return;
            }
            var streamName = args[0];
            var count = Environment.ProcessorCount;
            Console.WriteLine($"processorCount={Environment.ProcessorCount}, streamName={streamName}, producersCount={count}");
            var random = new Random((int)DateTime.UtcNow.Ticks);
            var randomTime = new Random((int)DateTime.UtcNow.Ticks);
            var methodNames = new [] {
                "GetUserRepos",
                "GetOrgRepos",
                "GetRepositories",
                "CreateRepos",
                "GetRepo",
                "EditRepo",
                "GetRepoContributors",
                "GetRepoLanguages",
                "GetRepoTeams",
                "GetRepoTags",
                "SearchRepositories",
                "SearchCommits",
                "SearchCode",
                "SearchIssues",
                "SearchUsers",
                "GetRepoReadMe",
                "GetContents",
                "CreateFile",
                "UpdateFile",
                "DeleteFile",
                "GetArchiveLink",
                "ListPullRequests",
                "GetPullRequest",
                "CreatePullRequest",
                "UpdatePullRequest"
            };
            var methodsCount = methodNames.Length - 1;
            var client = new AmazonKinesisClient();
            
            var tasks = new Task[count];
            for(var i = 0; i < count; i++) {
                var ii = i;
                tasks[ii] = Task.Factory.StartNew(async x => {
                    // Console.WriteLine($"Creating task {ii}");
                    var randomMethod = new Random((int)DateTime.UtcNow.Ticks + random.Next(1, 500000));
                    var customerIdGenerator = new Random((int)DateTime.UtcNow.Ticks);
                    while(true) {
                        // Console.WriteLine($"Task {ii}: Begin PutRecords");
                        try {
                            var response = await client.PutRecordsAsync(new PutRecordsRequest {
                                Records = Enumerable.Range(1, random.Next(500)).Select(_ => {
                                    var doc = new XElement("profiling",
                                        new XElement("method-name", methodNames[randomMethod.Next(0, methodsCount)]),
                                        new XElement("elapsed-ms", randomTime.Next(1, 1000)),
                                        new XElement("timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                                        new XElement("customer-id", customerIdGenerator.Next(1, Int32.MaxValue))
                                    );
                                    using(var m = new MemoryStream(Encoding.UTF8.GetBytes(doc.ToString()))) {
                                        return new PutRecordsRequestEntry {
                                            Data = m,
                                            PartitionKey = Guid.NewGuid().ToString()
                                        };
                                    }
                                }).ToList(),
                                StreamName = streamName
                            }, CancellationToken.None);
                            Thread.Sleep(customerIdGenerator.Next(1, 1000 * count));
                            // Console.WriteLine($"Task {ii}: End PutRecords, status={response.HttpStatusCode}");
                        }catch(Exception e) {
                            Console.WriteLine(e.Message);
                        }
                    }
                }, ii).Unwrap();
                Thread.Sleep(2000);
            }

            Task.WaitAll(tasks);
        }
    }
}
