using System;
using UnityEngine;

public enum StatType
{
    MaxHp,

    AttackPower,

    MoveSpeed,

    ReloadSpeed,

    ProjectileSpeed,

    ProjectileRange,

    AttackSpeed,

    CriticalChance,

    CriticalMultiplier,

    LifeSteal,

    Length
}



[Serializable]
public struct StatValue
{
    public float BaseValue;
    public float AdditiveValue;
    public float Multiplier;

    public float Value =>
        (BaseValue + AdditiveValue) * Multiplier;

    public void ResetModifier()
    {
        AdditiveValue = 0;
        Multiplier = 1f;
    }
}
public class StatModule : CharacterModule
{
    public sealed override Type RegistrationType
        => typeof(StatModule);

    [Header("Combat")]
    [SerializeField] private StatValue attackPower;
    [SerializeField] private StatValue attackSpeed;
    [SerializeField] private StatValue criticalChance;
    [SerializeField] private StatValue criticalMultiplier;

    [Header("Defense")]
    [SerializeField] private StatValue maxHp;

    [Header("Movement")]
    [SerializeField] private StatValue moveSpeed;

    [Header("Weapon")]
    [SerializeField] private StatValue reloadSpeed;
    [SerializeField] private StatValue projectileSpeed;
    [SerializeField] private StatValue projectileRange;

    [Header("Utility")]
    [SerializeField] private StatValue lifeSteal;

    public float MaxHp => maxHp.Value;
    public float AttackPower => attackPower.Value;
    public float AttackSpeed => attackSpeed.Value;
    public float CriticalChance => criticalChance.Value;
    public float CriticalMultiplier => criticalMultiplier.Value;
    public float MoveSpeed => moveSpeed.Value;
    public float ReloadSpeed => reloadSpeed.Value;
    public float ProjectileSpeed => projectileSpeed.Value;
    public float ProjectileRange => projectileRange.Value;
    public float LifeSteal => lifeSteal.Value;


}
