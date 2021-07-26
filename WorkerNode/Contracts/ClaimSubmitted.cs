namespace Contracts
{
    using System;


    public record ClaimSubmitted
    {
        public Guid ClaimId { get; init; }
    }
}
