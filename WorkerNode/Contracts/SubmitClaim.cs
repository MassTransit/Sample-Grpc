namespace Contracts
{
    using System;


    public record SubmitClaim
    {
        public Guid ClaimId { get; init; }
    }
}
