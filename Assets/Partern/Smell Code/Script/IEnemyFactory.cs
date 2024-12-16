namespace Smell
{
    public interface IEnemyFactory
    {
        Enemy Create(EnemyConfig config);
    }
}
