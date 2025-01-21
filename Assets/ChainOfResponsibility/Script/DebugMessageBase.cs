using System;
using System.IO;
using UnityEngine;

namespace COR
{
    public interface IDebugProcessor
    {
        IDebugProcessor SetNext(IDebugProcessor processor);
        void Process(DebugMessageBase message);
    }

    public class NullCheckProcessor : DebugProcessorBase
    {
        public override void Process(DebugMessageBase message)
        {
            if (message == null || message.Message == null)
            {
                Debug.LogError("NullCheckProcessor: Null message detected!");
                return;
            }
            base.Process(message);
        }
    }
    public abstract class DebugProcessorBase : IDebugProcessor
    {
        IDebugProcessor next;

        public IDebugProcessor SetNext(IDebugProcessor processor)
        {
            return next = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public virtual void Process(DebugMessageBase message) => next?.Process(message);
    }
    
    public class ConsoleLogProcessor : DebugProcessorBase
    {
        public override void Process(DebugMessageBase message)
        {
            Debug.Log($"ConsoleLogProcessor: {message.Message}");
            base.Process(message);
        }
    }

    public class StateSaveProcessor : DebugProcessorBase
    {
        public override void Process(DebugMessageBase message)
        {
            if (message is StateSaveMessage stateMessage)
            {
                string filePath = $"{stateMessage.StateName}_state.json";
                try
                {
                    string json = JsonUtility.ToJson(stateMessage.StateData);
                    File.WriteAllText(filePath, json);
                    Debug.Log($"StateSaveProcessor: State '{stateMessage.StateName}' saved to {filePath}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"StateSaveProcessor: Failed to save state '{stateMessage.StateName}'. Error: {e.Message}");
                }
                
                base.Process(message);
            }
        }
    }
    
    public class FileLogProcessor : DebugProcessorBase
    {
        private string logFilePath;
        
        public FileLogProcessor(string logFilePath) => this.logFilePath = logFilePath;

        public override void Process(DebugMessageBase message)
        {
            try
            {
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message.Message}\n");
            }
            catch (Exception e)
            {
                Debug.LogError($"FileLogProcessor: Failed to write to log file. Error: {e.Message}");
            }
            base.Process(message);
        }
    }
    
    public abstract class DebugMessageBase
    {
        public string Message { get; }
        
        protected DebugMessageBase(string message) => Message = message;
    }

    public class GeneralDebugMessage : DebugMessageBase
    {
        public GeneralDebugMessage(string message) : base(message) { }
    }
    
    public class StateSaveMessage : DebugMessageBase
    {
        public string StateName { get; }
        public object StateData { get; }

        public StateSaveMessage(string stateName, object stateData) : base($"Save Sate: {stateName}")
        {
            StateName =  stateName;
            StateData = stateData;
        }
    }
}