using System;
using Realms;
using SavingFor.LPI;

namespace SavingFor.RealmDomain.Model
{
        public sealed class Goal : RealmObject,IEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Amount { get; set; }
            public DateTimeOffset Start { get; set; }
            public DateTimeOffset End { get; set; }
            public string Image { get; set; }
            public double ProgressPerMinute { get; set; }

            public string ImageMedium => $"{Image}_medium";

            public string ImageSmall => $"{Image}_small";
    }
   
}
