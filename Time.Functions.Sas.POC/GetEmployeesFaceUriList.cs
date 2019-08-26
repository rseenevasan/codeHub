using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paycor.Time.Functions.Contract.MobileKiosk;
using Paycor.Time.Functions.Common;

namespace Time.Functions.Sas.POC
{
    public static class Function1
    {
        private static BlobHelper<EmployeeFaceData> blobHelper;
        [FunctionName("GetEmployeesFaceUriList")]
        public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "mobileKiosk/getEmployeesFaceUriList/{serialNumber}/{clientId}/{employeeUid}")] HttpRequest req,
           string serialNumber, string clientId, string employeeUid,
          ILogger logger)
        {
            try
            {
                var employeeId = Convert.ToInt64(employeeUid);
                string azureStorage = Environment.GetEnvironmentVariable(Constants.StorageConnectionStringVariable, EnvironmentVariableTarget.Process);
                blobHelper = new BlobHelper<EmployeeFaceData>(azureStorage);
                var blobCount = 1;
                var containerName = Constants.FaceServiceContainer + clientId;
                var blobContainer = await blobHelper.GetBlobContainer(containerName, logger);

                var result =await FaceServiceHelper.GetBlobUrisByContainer(blobContainer, clientId, employeeId, logger);

                return (ActionResult)new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult("An error occured " + ex.Message);
            }
        }
    }
}
