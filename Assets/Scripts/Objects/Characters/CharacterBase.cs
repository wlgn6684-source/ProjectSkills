using System.Collections.Generic;
using UnityEngine;

public delegate void MovementEvent(Vector3 move);
public delegate void LookAtEvent(Vector3 direction);
public delegate void DamageEvent(in DamageStruct info);
public delegate void RestoreEvent(in RestoreStruct info);

public class CharacterBase : MonoBehaviour
{
    public event MovementEvent OnMovement;
    public void MovementNotify(Vector3 move) => OnMovement?.Invoke(move);
    
    public event LookAtEvent OnLookAt;
    public void LookAtNotify(Vector3 direction) => OnLookAt?.Invoke(direction);
    public event DamageEvent OnDamage;
    public void DamageNotify(in DamageStruct info) => OnDamage?.Invoke(info);
    public RestoreEvent OnRestore;
    public void RestoreNotify(in RestoreStruct info) => OnRestore?.Invoke(info);


    ControllerBase _controller;
    public ControllerBase Controller => _controller;

    public Vector3 _lookRotation;
    public Vector3 LookRotation => _lookRotation;

    public virtual string Displaying => "character";

    // 모듈을 저장
    //
    Dictionary<System.Type, CharacterModule> moduleDictionary = new();

    public void AddModule(System.Type wantType, CharacterModule wantModule)
    {
        if (moduleDictionary.TryAdd(wantType, wantModule))
        {
            wantModule.OnRegistration(this);
        }
    }

    public void AddAllModuleFromObject(GameObject target)
    {
        foreach (CharacterModule currentModule in target.GetComponentsInChildren<CharacterModule>())
        {
            AddModule(currentModule.RegistrationType, currentModule);
        }
    }

    public void RemoveModule(System.Type wantType)
    {
        if (moduleDictionary.ContainsKey(wantType)) 
        {
            moduleDictionary[wantType]?.OnUnregistration(this);
            moduleDictionary.Remove(wantType); 
        }
        
    }

    public void RemoveAllModule()
    {
        foreach (CharacterModule currentModule in moduleDictionary.Values)
        { 
            currentModule.OnUnregistration(this);
        }
        moduleDictionary.Clear();
    }

    public T GetModule<T>() where T : CharacterModule
    {
        moduleDictionary.TryGetValue(typeof(T), out CharacterModule result);
        return result as T;
    }

    public ControllerBase Possessed(ControllerBase from)
    {
        if (Controller) Unpossessed();
        _controller = from;
        AddAllModuleFromObject(gameObject);
        OnPossessed(Controller);
        return Controller;
    }


    protected virtual void OnPossessed(ControllerBase newController){}

    protected virtual void OnUnpossessed(ControllerBase oldController){}
    public void Unpossessed()
    {
        if (Controller) OnUnpossessed(Controller);
        RemoveAllModule();
        _controller = null;
    }

    public bool Unpossessed(ControllerBase oldController)
    { 
        if (Controller != oldController) return false;
        Unpossessed();
        return true;
    }

}
