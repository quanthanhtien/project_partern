using System.Collections.Generic;
using UnityEngine;

namespace COR
{
    public class QuestDemo : MonoBehaviour
    {
        [SerializeField]
        QuestManager questManager;

        void Start()
        {
            SerializableGUID questId1 = new("Quest1");
            SerializableGUID questId2 = new();
            Debug.Log($"questId1: {questId1}");
            Debug.Log($"questId2: {questId2}");
            questManager.RegisterQuest(new Quest { Id = questId1, Name = "Find the treasure" });
            questManager.RegisterQuest(new Quest { Id = questId2, Name = "kill dragon" });
            questManager.UpdateQuest(new StartQuestMessage { QuestId = questId1 });
            questManager.UpdateQuest(new CompleteQuestMessage { QuestId = questId1 });
            questManager.UpdateQuest(new StartQuestMessage { QuestId = questId2 });
            questManager.UpdateQuest(new CompleteQuestMessage { QuestId = questId2 });
            questManager.UpdateQuest(new FailQuestMessage { QuestId = questId2 });
        }
    }
}
