using System.Collections.Generic;
using UnityEngine;

namespace COR
{
    public class QuestDemo : MonoBehaviour
    {
        [SerializeField] QuestManager questManager;

        void Start()
        {
            SerializableGUID questId1 = new("Quest1");
            Debug.Log($"questId1: {questId1}");
            questManager.RegisterQuest(new Quest { Id = questId1, Name = "Find the treasure" });
            questManager.UpdateQuest(new StartQuestMessage { QuestId = questId1 });
            questManager.UpdateQuest(new CompleteQuestMessage { QuestId = questId1 });
        }
    }
    
}
