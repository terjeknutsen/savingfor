namespace SavingFor.RealmDomain.Model
{
    public sealed class MonthlyPlan
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public double Amount { get; set; }
    }
}
