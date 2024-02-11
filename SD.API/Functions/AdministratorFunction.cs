using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Core.Middleware;

namespace SD.API.Functions
{
    public class AdministratorFunction()
    {
        [Authorize("administrator")]
        [Function("AdministratorTest")]
        public string AdministratorTest([HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "bla/test")] HttpRequestData req)
        {
            try
            {
                return "protect value";
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}