using System;
using System.Security;

namespace SavingFor.Domain.Model
{
    public sealed class Goal
        {
            public Guid Id { get; set; }
            public Guid AccountId { get; set; }
            public string Name { get; set; }
         
            public decimal Amount { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string Image { get; set; }
            public decimal ProgressPerMinute { get; set; }

            public string ImageMedium => $"{Image}_medium";

            public string ImageSmall => $"{Image}_small";
        }
   
}
