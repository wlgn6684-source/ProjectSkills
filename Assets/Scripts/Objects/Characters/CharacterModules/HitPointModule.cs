using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

public struct DamageStruct
{
    public GameObject from;
    public ControllerBase instigator;

    public int damageAmount;

    public bool critical;

    public bool ignoreInvincible;
}

public struct RestoreStruct
{
    public GameObject from;
    public ControllerBase instigator;

    public int restoreAmount;
}

public abstract class HitPointModule : CharacterModule
{
    protected FillValue fill = new FillValue(100, 100, 0);

    public sealed override Type RegistrationType
        => typeof(HitPointModule);

    private StatModule stat;

    protected void Initialize()
    {
        stat = Owner.GetModule<StatModule>();

        fill.SetMax((int)stat.MaxHp);
        fill.SetCurrent(fill.Max);
    }

    public int Max => fill.Max;
    public int Cur => fill.Current;

    public bool IsFullHealth => fill.IsMax;
    public bool IsEmpty => fill.IsEmpty;

    //------------------------------------
    // 
    //------------------------------------

    protected float lastDamagedTime;

    [SerializeField]
    protected float autoHealDelay = 3f;

    [SerializeField]
    protected float autoHealPercentPerSecond = 0.13f;

    //------------------------------------
    // 이벤트
    //------------------------------------

    public event Action<int, int> OnHpChanged;
    public event Action<DamageStruct> OnDamaged;
    public event Action<RestoreStruct> OnRestored;
    public event Action OnDead;

    //------------------------------------
    // Damage
    //------------------------------------

    public virtual int TakeDamage(in DamageStruct damageInfo)
    {
        if (IsEmpty)
            return 0;

        int before = Cur;

        fill.DecreaseCurrent(damageInfo.damageAmount);

        int actualDamage = before - Cur;

        lastDamagedTime = Time.time;

        OnDamaged?.Invoke(damageInfo);
        OnHpChanged?.Invoke(Cur, Max);

        if (IsEmpty)
        {
            OnDead?.Invoke();
        }

        return actualDamage;
    }

    //------------------------------------
    // Heal
    //------------------------------------

    public virtual int TakeRestore(
        in RestoreStruct restoreInfo)
    {
        if (IsEmpty)
            return 0;

        int before = Cur;

        fill.IncreaseCurrent(
            restoreInfo.restoreAmount);

        int actualRestore = Cur - before;

        if (actualRestore > 0)
        {
            OnRestored?.Invoke(restoreInfo);
            OnHpChanged?.Invoke(Cur, Max);
        }

        return actualRestore;
    }

    //------------------------------------
    // Auto Heal
    //------------------------------------

    public virtual void Tick()
    {
        if (IsEmpty)
            return;

        if (IsFullHealth)
            return;

        if (Time.time - lastDamagedTime
            < autoHealDelay)
            return;

        int healAmount =
            Mathf.CeilToInt(
                Max *
                autoHealPercentPerSecond *
                Time.deltaTime);

        if (healAmount <= 0)
            healAmount = 1;

        RestoreStruct restoreInfo =
            new RestoreStruct()
            {
                restoreAmount = healAmount
            };

        TakeRestore(restoreInfo);
    }
}