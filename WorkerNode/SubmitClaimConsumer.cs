namespace WorkerNode
{
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit;


    public class SubmitClaimConsumer :
        IConsumer<SubmitClaim>
    {
        public Task Consume(ConsumeContext<SubmitClaim> context)
        {
            LogContext.Info?.Log("Submit Claim: {Index}/{Count} {ClaimId} {SourceAddress}", context.Message.Index, context.Message.Count,
                context.Message.ClaimId, context.SourceAddress);

            return context.RespondAsync(new ClaimSubmitted { ClaimId = context.Message.ClaimId });
        }
    }
}