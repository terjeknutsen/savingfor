using System.Collections.Generic;
using Android.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using SavingFor.AndroidClient.Adapters.Holders;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Adapters
{
    internal sealed class MonthlyPlanAdapter : RecyclerView.Adapter
    {
        private readonly AssetManager assets;
        private readonly List<MonthlyPlan> plans = new List<MonthlyPlan>();

        public MonthlyPlanAdapter(AssetManager assets)
        {
            this.assets = assets;
        }

        private void Clear()
        {
            var count = ItemCount;
            plans.Clear();
            NotifyItemRangeRemoved(0, count);
        }
        public void SetPlans(IEnumerable<MonthlyPlan> items)
        {
            Clear();
            foreach (var monthlyPlan in items)
            {
                plans.Add(monthlyPlan);
                NotifyDataSetChanged();
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((PlanHolder)holder).SetMonthlyPlan(plans[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var planLayout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_plan, parent, false);
            var viewHolder = new PlanHolder(planLayout);
            return viewHolder;
        }

        public override int ItemCount => plans.Count;
    }
}