namespace SavingFor.AndroidClient.Interfaces
{
    public interface IHandleGoal
    {
        void HandleGoalGroupCreated(string groupName);
        void HandleGroupLinkRemoved();
    }
}