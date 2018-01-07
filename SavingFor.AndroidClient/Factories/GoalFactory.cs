using System;
using System.Globalization;
using Android.Content;
using Android.OS;
using SavingFor.AndroidClient.Constants;
using SavingFor.Domain.Model;
using SavingFor.Domain.Service;

namespace SavingFor.AndroidClient.Factories
{
    class GoalFactory
    {
        public static Goal Create(decimal amount, DateTime dateTime, string imageUri, string name)
        {
            var goalService = new GoalProgressService();
            var period = new Period(DateTime.Now, dateTime);
            var progressPerMinute = goalService.ProgressPerminute(period, amount);
            return new Goal
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                End = dateTime,
                Image = imageUri,
                Name = name,
                Start = DateTime.Now,
                ProgressPerMinute = progressPerMinute
            };
        }

        public static Goal Copy(Goal existingGoal, Goal editedGoal)
        {
            var goalService = new GoalProgressService();
            var period = new Period(existingGoal.Start, editedGoal.End);
            var progressPerMinute = goalService.ProgressPerminute(period, editedGoal.Amount);

            editedGoal.Id = existingGoal.Id;
            editedGoal.ProgressPerMinute = progressPerMinute;
            editedGoal.Start = existingGoal.Start;

            return editedGoal;
        }

        public static Bundle Transform(Bundle bundle,Goal tmp)
        {
            bundle.PutString(AppConstant.Goal.Id, tmp.Id.ToString());
            bundle.PutString(AppConstant.Goal.AccountId, tmp.AccountId.ToString());
            bundle.PutString(AppConstant.Goal.Name, tmp.Name);
            bundle.PutString(AppConstant.Goal.Amount, tmp.Amount.ToString(NumberFormatInfo.InvariantInfo));
            bundle.PutString(AppConstant.Goal.Start, tmp.Start.ToString(CultureInfo.InvariantCulture));
            bundle.PutString(AppConstant.Goal.End, tmp.End.ToString(CultureInfo.InvariantCulture));
            return bundle;
        }
    }
}