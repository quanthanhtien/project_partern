namespace COR
{
    public class Quest
    {
        public SerializableGUID Id;
        public string Name;
        public QuestState State = QuestState.NotStarted;
    }
    
    public enum QuestState
    {
        NotStarted,
        InProgress,
        Completed,
        Failed
    }
    
    public enum QuestEvent
    {
        Start,
        Complete,
        Fail
    }
}