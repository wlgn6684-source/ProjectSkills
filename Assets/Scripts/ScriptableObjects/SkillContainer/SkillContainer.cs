using UnityEngine;

public enum SkillType
{
    Active, Passive, Ultimate, Length
}
public enum SkillTier
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum SkillTarget
{
    Self,
    Ally,
    Enemy,
    Area
}

public enum SkillCastType
{
    Instant,
    Projectile,
    Channeling,
    Buff
}

[CreateAssetMenu(fileName = "SkillContainer", menuName = "Skill/SkillBase")]
public class SkillContainer : InfoSkill
{
    public SkillType Type;
    public SkillTier Tier;
    public SkillTarget Target;
    public SkillCastType CastType;
    

    public float Cooldown;
    public float Damage;
    public float Cost;
    public float Duration;
    public float Range;

    public int MaxLevel;


    public GameObject SkillPrefab;
}
