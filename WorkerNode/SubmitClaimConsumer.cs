namespace WorkerNode
{
    using System.Threading.Tasks;
    using Contracts;
    using MassTransit;
    using MassTransit.Context;


    public class SubmitClaimConsumer :
        IConsumer<SubmitClaim>
    {
        public Task Consume(ConsumeContext<SubmitClaim> context)
        {
            LogContext.Info?.Log("Submit Claim: {ClaimId} {SourceAddress}", context.Message.ClaimId, context.SourceAddress);

            return context.RespondAsync(new ClaimSubmitted {ClaimId = context.Message.ClaimId});
        }
    }
}
