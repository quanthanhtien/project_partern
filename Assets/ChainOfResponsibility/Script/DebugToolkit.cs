using UnityEngine;

namespace COR
{
    public class DebugToolkit : MonoBehaviour
    {
        [SerializeField] string LogFilePath = "debug_log.txt";

        private IDebugProcessor chain;

        void Awake()
        {
            chain = new NullCheckProcessor();
            chain.SetNext(new ConsoleLogProcessor())
                .SetNext(new FileLogProcessor(LogFilePath))
                .SetNext(new StateSaveProcessor());
        }
        
        public void Log(DebugMessageBase message) => chain.Process(message);
    }
}