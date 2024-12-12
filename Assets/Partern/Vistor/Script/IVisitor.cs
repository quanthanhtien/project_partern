namespace Visitor
{
    public interface IVisitor
    {
        // void Visit(HealthComponent healthComponent);
        // void Visit(ManaComponent manaComponent);
        void Visit(object o);
    }
}
