using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SavingFor.AndroidClient.Adapters.Apis;
using SavingFor.AndroidClient.Adapters.CommandObjects;
using SavingFor.AndroidClient.Adapters.Holders;
using SavingFor.AndroidClient.Settings;
using SavingFor.AndroidLib.Apis;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Adapters
{
    internal sealed class GoalItemAdapter : RecyclerView.Adapter, IModeable
    {
        internal class GoalItemState : IEquatable<GoalItemState>
        {
            public GoalItemState(Goal goal)
            {
                Goal = goal;
                Id = goal.Id;
                ProgressPerMinute = goal.ProgressPerMinute;
                Image = goal.ImageMedium;
                Name = goal.Name;
                End = goal.End;
                Amount = goal.Amount;
                GoalAmountRequired = new GoalAmountRequired(goal.ProgressPerMinute, goal.Start);
            }

            public GoalAmountRequired GoalAmountRequired { get; set; }

            public bool IsChecked { get; set; }
            public Guid Id { get; }
            public decimal ProgressPerMinute { get; }
            public string Image { get; }
            public string Name { get; }
            public DateTime End { get; }
            public decimal Amount { get; }
            public bool IsEditMode { get; set; }
            public int Position { get; set; }
            public Goal Goal { get; }

            public override bool Equals(object obj)
            {
                var g = obj as GoalItemState ?? new GoalItemState(new Goal());
                return Equals(g);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = IsChecked.GetHashCode();
                    hashCode = (hashCode * 397) ^ Id.GetHashCode();
                    hashCode = (hashCode * 397) ^ ProgressPerMinute.GetHashCode();
                    hashCode = (hashCode * 397) ^ (Image?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ End.GetHashCode();
                    hashCode = (hashCode * 397) ^ Amount.GetHashCode();
                    hashCode = (hashCode * 397) ^ IsEditMode.GetHashCode();
                    return hashCode;
                }
            }

            public bool Equals(GoalItemState other)
            {
                return Id == other.Id;
            }
        }

        private readonly List<Goal> goals = new List<Goal>();
        private readonly ContentResolver contentResolver;
        private readonly Typeface typeface;
        private readonly Bitmap placeholder;
        private readonly Resources resources;
        private readonly List<GoalItemState> goalStates = new List<GoalItemState>();
        private readonly IImageService imageService;

        public event EventHandler<GoalClickedEvent> ItemClick;
        public event EventHandler<int> ItemLongClick;
        public event EventHandler<ItemCheckChangedEvent> ItemCheckChanged;

        public GoalItemAdapter(ContentResolver contentResolver, Typeface typeface, Bitmap placeholder, Resources resources, IImageService imageService)
        {
            this.contentResolver = contentResolver;
            this.typeface = typeface;
            this.placeholder = placeholder;
            this.resources = resources;
            this.imageService = imageService;
        }

        public int CheckedItems => goalStates.Count(s => s.IsChecked);

        public IEnumerable<Goal> SelectedGoals
        {
            get
            {
                var selectedGoals = new List<Goal>();

                foreach (var checkedItem in goalStates.Where(s => s.IsChecked))
                {
                    selectedGoals.Add(checkedItem.Goal);
                }
                return selectedGoals;
            }
        }

        public void RemoveSelectedGoals()
        {
            var checkedItems = new List<Guid>(goalStates.Where(s => s.IsChecked).Select(g => g.Id)).OrderByDescending(a => a);

            foreach (var checkedItem in checkedItems)
            {
                RemoveGoal(checkedItem);
            }
        }

        private void AddGoal(Goal goal)
        {
            if (goalStates.Contains(new GoalItemState(goal))) return;
            var index = goals.FindIndex(g => g.End > goal.End);
            goals.Insert(index != -1 ? index : goals.Count - 1, goal);
            goalStates.Insert(index != -1 ? index : goals.Count - 1, new GoalItemState(goal));
            NotifyItemInserted(index != -1 ? index : goals.Count - 1);

        }

        private void RemoveGoal(Guid id)
        {
            var goal = goals.FirstOrDefault(g => g.Id == id);
            var goalState = goalStates.FirstOrDefault(gs => gs.Id == id);
            if (goal == null || goalState == null) return;

            var index = goalStates.IndexOf(goalState);
            goals.Remove(goal);
            goalStates.Remove(goalState);

            NotifyItemRemoved(index);
        }

        public void SetGoals(IEnumerable<Goal> items)
        {
            if (goals.Any())
            {

                foreach (var item in items)
                {
                    AddGoal(item);
                }
            }
            else
            {

                goals.AddRange(items.OrderBy(i => i.End));
                goalStates.AddRange(goals.Select(g => new GoalItemState(g)));

                NotifyItemRangeInserted(0, ItemCount);
            }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (!goalStates.Any()) return;

            if (position >=0 && position < goalStates.Count)
                ((GoalHolder)holder).SetGoal(goalStates[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemLayout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_goal, parent, false);
            var goalCommand = new GoalCommand
            {
                View = itemLayout,
                Resolver = contentResolver,
                Typeface = typeface,
                PlaceHolder = placeholder,
                Resources = resources,
                ItemClickListener = OnItemClick,
                ItemLongClickListener = OnItemLongClick,
                Mode = this,
                ItemSelected = OnItemCheckChanged,
                ImageService = imageService
            };
            var viewHolder = new GoalHolder(goalCommand) { Resources = resources };
            return viewHolder;
        }

        public override int ItemCount => goals.Count;

        private void OnItemClick(Guid e,ImageView imageView,string color)
        {
            var goal = goals.FirstOrDefault(g => g.Id == e);
            if (goal == null) return;
            Preferences.HeroImageGoalId = goal.Id.ToString();
            var goalClickEvent = new GoalClickedEvent
            {
                Id = e,
                ImageView = imageView,
                Color = color
            };
            ItemClick?.Invoke(this, goalClickEvent);
        }

        private void OnItemLongClick(int e)
        {
            IsEditMode = true;
            ItemLongClick?.Invoke(this, e);
            OnModeChanged(true);
            goalStates.ForEach(g => g.IsEditMode = true);
        }

        private void OnItemCheckChanged(ItemCheckChangedEvent e)
        {
            if (goalStates.TrueForAll(s => !s.IsChecked))
            {
                OnModeChanged(false);
            }
            ItemCheckChanged?.Invoke(this, e);
        }

        public bool IsEditMode { get; private set; }
        public IEnumerable<Goal> Items => goals;

        public event EventHandler<bool> ModeChanged;
        public event EventHandler Refresh;

        private void OnModeChanged(bool e)
        {
            ModeChanged?.Invoke(this, e);
        }

        public void OnRefresh()
        {
            Refresh?.Invoke(this, new EventArgs());
        }

        public void CancelEditMode()
        {
            OnModeChanged(false);
        }

        public void Clear()
        {
            var count = ItemCount;
            goals.Clear();
            goalStates.Clear();
            NotifyItemRangeRemoved(0, count);
        }

        public bool IsToBeDeleted(Guid index)
        {
            return SelectedGoals.Any(selectedGoal => goals.Contains(selectedGoal));
        }

        public void RefreshView()
        {
            NotifyDataSetChanged();
        }
    }

    internal class GoalClickedEvent
    {
        public Guid Id { get; set; }
        public ImageView ImageView { get; set; }
        public TextView TextViewTotal { get; set; }
        public TextView TextViewName { get; set; }
        public string Color { get; set; }
    }

    internal struct ItemCheckChangedEvent
    {
        public int Position { get; set; }
        public bool IsChecked { get; set; }
    }
}