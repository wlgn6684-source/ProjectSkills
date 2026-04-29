using System;
using UnityEngine;

public struct DamageStruct
{
    public GameObject from;
    public ControllerBase instigator;
    public int damageAmount;
    public bool critical;
    
}

public struct RestoreStruct
{
    public GameObject from;
    public ControllerBase instigator;
    public int restoreAmount;
}

public abstract class HitPointModule : CharacterModule
{
    protected FillValue fill = new FillValue(100,100,0);
    public sealed override System.Type RegistrationType => typeof(HitPointModule);

    public int Max => fill.Max;
    public int Cur => fill.Current;
    public bool IsFullHealth => fill.IsMax;
    public bool IsEmpty      => fill.IsEmpty;


    public int TakeDamage(in DamageStruct damageInfo)
    {
        fill.DecreaseCurrent(damageInfo.damageAmount);
        return damageInfo.damageAmount;
    }
    public int TakeRestore(in RestoreStruct restoreInfo)
    {
        fill.IncreaseCurrent(restoreInfo.restoreAmount);
        return restoreInfo.restoreAmount;
    }


}
