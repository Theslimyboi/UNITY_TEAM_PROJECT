using UnityEngine;

// 1.0 = normal damage, 2.0 = resistant (half damage), 0.5 = weak (double damage)
public class EnemyStats : MonoBehaviour
{
    public float fireResistance = 1f;
    public float iceResistance = 1f;
    public float lightningResistance = 1f;
    public float poisonResistance = 1f; // was missing before — Poison now respects resistance
}