using UnityEngine;
using System;
using System.Collections.Generic;
namespace COR
{
    public interface IQuestProcessor
    {
        IQuestProcessor SetNext(IQuestProcessor processor);
        void Process(QuestMessageBase message, Dictionary<SerializableGUID, Quest> quests);
    }
    
    public abstract class QuestProcessorBase : IQuestProcessor
    {
        private IQuestProcessor next;
        public IQuestProcessor SetNext(IQuestProcessor processor) => next = processor;
        public virtual void Process(QuestMessageBase message, Dictionary<SerializableGUID, Quest> quests)
        {
            next?.Process(message, quests);
        }
    }
    
    public class GenericQuestProcessor<TMessage> : QuestProcessorBase where TMessage : QuestMessageBase
    {
        
    }
    
    public class FailQuestProcessor : QuestProcessorBase
    {
        public override void Process(QuestMessageBase message, Dictionary<SerializableGUID, Quest> quests)
        {
            Debug.Log($"{GetType().Name}: Processing message of type{message.GetType().Name}");

            if (message is FailQuestMessage failMessage &&
                quests.TryGetValue(failMessage.QuestId, out var quest))
            {
                if (quest.State == QuestState.InProgress)
                {
                    quest.State = QuestState.Failed;
                    Debug.Log($"Quest {quest.Name} is failed");
                }
                return;
            }
            
            base.Process(message, quests);
        }
    }
    
    public class CompleteQuestProcessor : QuestProcessorBase
    {
        public override void Process(QuestMessageBase message, Dictionary<SerializableGUID, Quest> quests)
        {
            Debug.Log($"{GetType().Name}: Processing message of type{GetType().Name}");

            if (message is CompleteQuestMessage completeMessage &&
                quests.TryGetValue(completeMessage.QuestId, out var quest))
            {
                if (quest.State == QuestState.InProgress)
                {
                    quest.State = QuestState.Completed;
                    Debug.Log($"Quest {quest.Name} is completed");
                }
                return;
            }
            base.Process(message, quests);
        }
    }
    
    public class StartQuestProcessor : QuestProcessorBase
    {
        public override void Process(QuestMessageBase message, Dictionary<SerializableGUID, Quest> quests)
        {
            Debug.Log($"{GetType().Name}: Processing message of type{message.GetType().Name}");

            if (message is StartQuestMessage startMessage &&
                quests.TryGetValue(startMessage.QuestId, out var quest))
            {
                if (quest.State == QuestState.NotStarted)
                {
                    quest.State = QuestState.InProgress;
                    Debug.Log($"Quest {quest.Name} is started");
                }
                return;
            }
            base.Process(message, quests);
        }
    }
    
    public abstract class QuestMessageBase
    {
        public SerializableGUID QuestId;
    }
    
    public class StartQuestMessage : QuestMessageBase
    {
    }
    public class CompleteQuestMessage : QuestMessageBase
    {
    }
    public class FailQuestMessage : QuestMessageBase
    {
    }
}
[Serializable]
public struct SerializableGUID
{
    public string guidString;

    public SerializableGUID(string guidString)
    {
        this.guidString = guidString;
    }

    public override string ToString()
    {
        return guidString;
    }
}