using System;
using System.Collections.Generic;
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

    private Dictionary<StatType, StatValue> statTable;

    public override void Initialize(CharacterBase owner)
    {
        base.Initialize(owner);

        statTable = new Dictionary<StatType, StatValue>()
    {
        { StatType.MaxHp, maxHp },
        { StatType.AttackPower, attackPower },
        { StatType.MoveSpeed, moveSpeed },
        { StatType.ReloadSpeed, reloadSpeed },
        { StatType.ProjectileSpeed, projectileSpeed },
        { StatType.ProjectileRange, projectileRange },
        { StatType.AttackSpeed, attackSpeed },
        { StatType.CriticalChance, criticalChance },
        { StatType.CriticalMultiplier, criticalMultiplier },
        { StatType.LifeSteal, lifeSteal }
    };
    }

    public float GetStat(StatType type)
    {
        if (statTable.TryGetValue(type, out var stat))
            return stat.Value;

        return 0f;
    }

    public void AddStat(StatType type, float value)
    {
        if (!statTable.TryGetValue(type, out var stat))
            return;

        stat.AdditiveValue += value;
    }

    public void MultiplyStat(StatType type, float value)
    {
        if (!statTable.TryGetValue(type, out var stat))
            return;

        stat.Multiplier *= value;
    }

    public void ResetStatModifier(StatType type)
    {
        if (!statTable.TryGetValue(type, out var stat))
            return;

        stat.ResetModifier();
    }

    // 기존 사용 방식 유지
    public float MaxHp => GetStat(StatType.MaxHp);
    public float AttackPower => GetStat(StatType.AttackPower);
    public float AttackSpeed => GetStat(StatType.AttackSpeed);
    public float CriticalChance => GetStat(StatType.CriticalChance);
    public float CriticalMultiplier => GetStat(StatType.CriticalMultiplier);
    public float MoveSpeed => GetStat(StatType.MoveSpeed);
    public float ReloadSpeed => GetStat(StatType.ReloadSpeed);
    public float ProjectileSpeed => GetStat(StatType.ProjectileSpeed);
    public float ProjectileRange => GetStat(StatType.ProjectileRange);
    public float LifeSteal => GetStat(StatType.LifeSteal);
}