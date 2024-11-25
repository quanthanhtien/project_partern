using UnityEngine;

[CreateAssetMenu(fileName = "ShieldFactory", menuName = "Weapon Factory/Shield")]
public class GenericShieldFactory : ShieldFactory
{
    public override IShield CreateShield()
    {
        return new Shield();
    }
}