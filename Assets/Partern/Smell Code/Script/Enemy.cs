using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Smell
{
    public interface IEntity { }

    public class Enemy : MonoBehaviour, IEntity
    {
        public EnemyConfig Config { get; private set; }
        public Queue<ICommand<IEntity>> commandQueue = new();

        public void Initialize(EnemyConfig config) => Config = config;

        public void QueueCommand(ICommand<IEntity> command) => commandQueue.Enqueue(command);

        public void ExcuteCommand()
        {
            if (commandQueue.Count > 0)
            {
                commandQueue.Dequeue().Execute();
            }
        }

        [Button]
        void Update()
        {
            ExcuteCommand();

            var newCommand = new BattleCommand.Builder(new List<IEntity> { this })
                .WithAction(_ => Debug.Log($"{Config.type} do something"))
                .WithLogger()
                .Build();
            QueueCommand(newCommand);
        }
    }
}
