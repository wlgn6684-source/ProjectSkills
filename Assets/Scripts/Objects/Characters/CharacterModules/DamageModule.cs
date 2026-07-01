using System;
using UnityEngine;

public class DamageModule : CharacterModule
{
    public sealed override Type RegistrationType
        => typeof(DamageModule);

    private StatModule stat;

    public override void OnRegistration(CharacterBase newOwner)
    {
        base.OnRegistration(newOwner);

        stat = Owner.GetModule<StatModule>();
    }

    public virtual DamageStruct CreateDamage()
    {
        DamageStruct damage = new DamageStruct();

        damage.from = gameObject;
        damage.instigator = Owner.Controller;

        damage.damageAmount =
            Mathf.RoundToInt(
                stat.AttackPower);

        damage.critical = false;
        damage.ignoreInvincible = false;

        return damage;
    }
}
