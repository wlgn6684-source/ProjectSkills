using UnityEngine;

public class CharacterModule : MonoBehaviour
{
    public virtual System.Type RegistrationType => typeof(CharacterModule);
    CharacterBase _owner;
    public CharacterBase Owner => _owner;

    public virtual void OnRegistration(CharacterBase newOwner)
    { 
        _owner = newOwner;
    }

    public virtual void OnUnregistration(CharacterBase oldOwner)
    { 
        _owner = null;
    }

    public virtual void Initialize(CharacterBase owner)
    {
        _owner = owner;
    }
}


