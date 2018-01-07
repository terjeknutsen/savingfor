using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using SavingFor.AndroidClient.Adapters.Holders;
using SavingFor.AndroidLib.Apis;
using SavingFor.Domain.Model;

namespace SavingFor.AndroidClient.Adapters
{
    internal sealed class GoalFinishedAdapter : RecyclerView.Adapter
    {
        private readonly Bitmap placeholder;
        private readonly ContentResolver resolver;
        private readonly Resources resources;
        private readonly List<Goal> goals = new List<Goal>();
        private bool isEditMode;
        private readonly HashSet<Guid> checkedItems = new HashSet<Guid>();
        private readonly IImageService imageService;

        public GoalFinishedAdapter(Bitmap placeholder, ContentResolver resolver, Resources resources, IImageService imageService)
        {
            this.placeholder = placeholder;
            this.resolver = resolver;
            this.resources = resources;
            this.imageService = imageService;
        }

        public void Clear()
        {
            var count = ItemCount;
            goals.Clear();
            checkedItems.Clear();
            NotifyItemRangeRemoved(0, count);
        }

        public void SetItems(IEnumerable<Goal> items)
        {
            goals.AddRange(items);
            NotifyItemRangeInserted(0, goals.Count);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((GoalFinishedHolder)holder).SetItem(goals[position], isEditMode, checkedItems.Contains(goals[position].Id));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.list_item_finished_goal, parent, false);
            var viewHolder = new GoalFinishedHolder(layout, OnItemClick, OnItemLongClick, imageService)
            {
                Placeholder = placeholder,
                Resolver = resolver,
                Resources = resources
            };
            return viewHolder;
        }

        public override int ItemCount => goals.Count;

        public IEnumerable<Goal> SelectedGoals
        {
            get {
                    return checkedItems.Select(checkedItem => goals.First(g => g.Id == checkedItem));
                }
        }

        public event EventHandler<int> ItemCheckChanged;

        public event EventHandler<Guid> ItemClick;
        public event EventHandler<Guid> ItemLongClick;
        public event EventHandler EditModeEnded;

        private void OnItemClick(Guid itemId)
        {
            if (isEditMode)
            {
                AddOrRemoveCheckedItem(itemId);
                VerifyEditMode();
                return;
            }
            ItemClick?.Invoke(this, itemId);
        }

        private void VerifyEditMode()
        {
            if (checkedItems.Any()) return;
            isEditMode = false;
            OnEditModeEnded();
            NotifyItemRangeChanged(0, ItemCount);
        }

        private void OnItemLongClick(Guid itemId)
        {
            if (isEditMode)
            {
                return;
            }

            isEditMode = true;
            AddOrRemoveCheckedItem(itemId);

            NotifyItemRangeChanged(0, ItemCount);
            ItemLongClick?.Invoke(this, itemId);
        }

        private void AddOrRemoveCheckedItem(Guid itemId)
        {
            if (checkedItems.Contains(itemId))
            {
                checkedItems.Remove(itemId);
            }
            else
            {
                checkedItems.Add(itemId);
            }
            OnItemCheckChanged(checkedItems.Count);
            NotifyItemRangeChanged(0, ItemCount);
        }

        public void CancelEditMode()
        {
            isEditMode = false;
            NotifyItemRangeChanged(0, ItemCount);
        }

        private void OnEditModeEnded()
        {
            EditModeEnded?.Invoke(this, EventArgs.Empty);
        }

        private void OnItemCheckChanged(int e)
        {
            ItemCheckChanged?.Invoke(this, e);
        }
    }
}