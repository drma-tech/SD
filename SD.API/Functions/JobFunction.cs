namespace SD.API.Functions;

public class JobFunction(CosmosJobRepository repo)
{
    //[Function("Welcome")]
    //public async Task Welcome([HttpTrigger(AuthorizationLevel.Anonymous, Method.Post, Route = "job/welcome")] HttpRequestData req, CancellationToken cancellationToken)
    //{
    //    var jobs = await repo.Query<WelcomeModel>(job => job.RunAt <= DateTimeOffset.UtcNow, JobType.Welcome, cancellationToken);
    //    var zepto = new ZeptoMailClient(ApiStartup.Configurations.ZeptoMail!.WelcomeApiKey!);

    //    foreach (var job in jobs)
    //    {
    //        if (job.Email.NotEmpty()) await zepto.SendWelcomeEmail(job.Email, job.Id.Split(":")[1], cancellationToken);

    //        await repo.Delete(job, cancellationToken);
    //    }
    //}
}