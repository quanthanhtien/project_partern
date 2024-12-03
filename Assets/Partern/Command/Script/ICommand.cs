using System.Security.Principal;
using UnityEngine;
using System.Threading.Tasks;
using static System.Activator;
public interface ICommand
{
    Task Execute();
}

public abstract class HeroCommand : ICommand
{
    protected readonly IEntity hero;
    public abstract Task Execute();

    protected HeroCommand(IEntity hero)
    {
        this.hero = hero;
    }
    
    public static T create<T>(IEntity hero) where T : HeroCommand
    {
        return (T) System.Activator.CreateInstance(typeof(T), hero);
    }
    
    public class AttackCommand : HeroCommand
    {
        public AttackCommand(IEntity hero) : base(hero)
        {
        }

        public override async Task Execute()
        {
            hero.Attack();
        }
    }
    public class SpinCommand : HeroCommand
    {
        public SpinCommand(IEntity hero) : base(hero)
        {
        }

        public override async Task Execute()
        {
            hero.Spin();
        }
    }
    public class JumpCommand : HeroCommand
    {
        public JumpCommand(IEntity hero) : base(hero)
        {
        }

        public override async Task Execute()
        {
            hero.Jump();
        }
    }
}