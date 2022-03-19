namespace Contracts
{
    using System;


    public record SubmitClaim
    {
        public Guid ClaimId { get; init; }
        public int Index { get; init; }
        public int Count { get; init; }
    }
}