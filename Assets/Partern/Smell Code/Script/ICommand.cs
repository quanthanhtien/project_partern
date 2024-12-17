using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Smell
{
    public interface ICommand<T>
    {
        void Execute();
    }
    
    public class CommandLogger : ICommand<IEntity>
    {
        readonly ICommand<IEntity> command;

        public CommandLogger(ICommand<IEntity> command)
        {
            this.command = command;
        }
        
        public void Execute()
        {
            Console.WriteLine($"Command Excute: {command.GetType().Name}");
            command.Execute();
        }

        
    }
        
    public class BattleCommand : ICommand<IEntity>
    {
        List<IEntity> targets;
        Action<IEntity> action = delegate {};

        public BattleCommand(){}
        public void Execute()
        {
            foreach (var t in targets)
            {
                action.Invoke(t);
            }
        }
        public class Builder
        {
            readonly BattleCommand command = new BattleCommand();
            private bool isLoggerEnable;
            public Builder(List<IEntity> targets = default)
            {
                command.targets = targets ?? new List<IEntity>();
            }
            
            public Builder WithAction(Action<IEntity> action)
            {
                command.action = action;
                return this;
            }
            public Builder WithLogger()
            {
                isLoggerEnable = true;
                return this;
            }
            public ICommand<IEntity> Build() => isLoggerEnable ? new CommandLogger(command) : command;
        }
    }

    
}
