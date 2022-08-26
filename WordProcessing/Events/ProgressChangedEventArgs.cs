using System.ComponentModel;

namespace FiveWordFinder.WordProcessing.Events
{
    public class ProgressChangedMessageEventArgs : ProgressChangedEventArgs
    {
        public string ProgressMessage { get; private set; }
        public int? CurrentStep { get; private set; }
        public int? TotalSteps { get; private set; }
        public ProgressChangedMessageEventArgs(int progressPercentage,
                                               int? currentStep,
                                               int? totalSteps,
                                               string progressMessage,
                                               object? userState) : base(progressPercentage, userState)
        {
            CurrentStep = currentStep;
            TotalSteps = totalSteps;
            ProgressMessage = progressMessage;
        }
    }
}
