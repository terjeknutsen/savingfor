using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Adapters.Holders
{
    internal sealed class PlanHolder : RecyclerView.ViewHolder
    {
        private TextView month;
        private TextView year;
        private TextView amount;

        public PlanHolder(View itemView) : base(itemView)
        {
            BindView(itemView);
        }

        private void BindView(View itemView)
        {
            month = itemView.FindViewById<TextView>(Resource.Id.text_month);
            year = itemView.FindViewById<TextView>(Resource.Id.text_year);
            amount = itemView.FindViewById<TextView>(Resource.Id.text_amount);
        }

        public void SetMonthlyPlan(MonthlyPlan plan)
        {
            month.Text = plan.Month;
            year.Text = @"" + plan.Year;
            amount.Text = plan.Amount.ToString("C");
        }
    }
}