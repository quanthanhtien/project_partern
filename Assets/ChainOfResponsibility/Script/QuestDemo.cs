using UnityEngine;

namespace COR
{
    public class QuestDemo : MonoBehaviour
    {
        [SerializeField] QuestManager questManager;

        void Start()
        {
            SerializableGUID questId = new ();
            questManager.RegisterQuest(new Quest{Id = questId, Name = "Find the treasure"});
            
            questManager.UpdateQuest(new StartQuestMessage{QuestId = questId});
            questManager.UpdateQuest(new CompleteQuestMessage{QuestId = questId});
            questManager.UpdateQuest(new FailQuestMessage{QuestId = questId});
        }
    }
}