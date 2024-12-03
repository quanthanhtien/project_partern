using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
public class CommandManager :SerializedMonoBehaviour
{
    public IEntity Entity;
    public ICommand singleCommand;
    public List<ICommand> commands = new List<ICommand>();

    private void Start()
    {
        Entity = GetComponent<IEntity>();
        
        singleCommand = HeroCommand.create<HeroCommand.AttackCommand>(Entity);

        commands = new List<ICommand>
        {
            HeroCommand.create<HeroCommand.AttackCommand>(Entity),
            HeroCommand.create<HeroCommand.SpinCommand>(Entity),
            HeroCommand.create<HeroCommand.JumpCommand>(Entity)
        };
    }

    async Task ExecuteCommend(List<ICommand> commands)
    {
        await CommanIvoker.ExecuteCommend(commands);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ExecuteCommend(new List<ICommand>{singleCommand});
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            ExecuteCommend(commands);
        }
    }
}

public class CommanIvoker
{
    public static async Task ExecuteCommend(List<ICommand> commands)
    {
        foreach (var command in commands)
        {
            await command.Execute();
        }
    }
}
