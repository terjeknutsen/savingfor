using System;

namespace SavingFor.AndroidClient.Adapters.Apis
{
    internal interface IModeable
    {
        bool IsEditMode { get; }
        event EventHandler<bool> ModeChanged;
        event EventHandler Refresh;
    }
}