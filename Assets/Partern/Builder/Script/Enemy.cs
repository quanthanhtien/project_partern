using UnityEngine;

public partial class Enemy : MonoBehaviour
{
    public string Name { get; set; }
    public int Health { get; set; }
    public float Speed { get; set; }
    public int Damage { get; set; }
    public bool IsBoss { get; set; }
}
