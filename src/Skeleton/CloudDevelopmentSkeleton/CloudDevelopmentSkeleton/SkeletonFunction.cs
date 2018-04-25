using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Globalization;
using System;
using Stylelabs.M.Sdk.WebApiClient.ResourceExtensions;
using System.Threading.Tasks;


namespace CloudDevelopmentSkeleton
{
    public static class SkeletonFunction
    {
        [FunctionName("SkeletonFunction")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "HttpTriggerCSharp/name/{name}")]HttpRequestMessage req, string name, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // TODO: Put your code here
            // MConnector.Client ....

            // Fetching the name from the path parameter in the request URL
            return req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }
    }
}
