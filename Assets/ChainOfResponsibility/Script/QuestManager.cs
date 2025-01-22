using System.Collections.Generic;
using UnityEngine;
namespace COR
{
    public class QuestManager :MonoBehaviour
    {
        private Dictionary<SerializableGUID, Quest> quests = new();
        private IQuestProcessor chain;
        
        void Awake()
        {
            chain = new StartQuestProcessor();
            chain.SetNext(new CompleteQuestProcessor()).SetNext(new FailQuestProcessor());
        }

        public void RegisterQuest(Quest quest) => quests.Add(quest.Id, quest);
    
        public void UpdateQuest(QuestMessageBase message) => chain.Process(message, quests);
        
    }
}