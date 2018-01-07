namespace SavingFor.AndroidClient.Constants
{
    class AppConstant
    {
        public static string GoalCreated => "com.savingfor.goal_created";
        public static string SelectedGoals => "com.savingfor.selected_goals";
        public static string SelectedGoalsBundle => "com.savingfor.selected_goals_bundle";

        public class Goal
        {
            public static string Id => "no.livemoney_goal_id";
            public static string AccountId => "no.livemoney_goal_account_id";
            public static string Name => "no.livemoney_goal_name";
            public static string Amount => "no.livemoney_goal_amount";
            public static string Start => "no.livemoney_goal_start";
            public static string End => "no.livemoney_goal_end";
        }

        public class RequestUri
        {
            public static string GoalUri => "no.livemoney_goal_request";
        }
    }
}