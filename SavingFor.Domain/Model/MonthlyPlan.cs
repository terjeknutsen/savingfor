namespace SavingFor.Domain.Model
{
    public sealed class MonthlyPlan
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
    }
}
