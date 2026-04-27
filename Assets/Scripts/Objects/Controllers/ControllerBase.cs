using UnityEngine;

public class ControllerBase : MonoBehaviour, IFunctionable
{
    CharacterBase _character;
    public CharacterBase Character => _character;

   

    public virtual void RegistrationFunctions()
    {
        Possess(GetComponent<CharacterBase>());
    }

    public virtual void UnregistrationFunctions()
    {
        UnPossess();
    }

    protected virtual void OnPossess(CharacterBase newCharacter) { }
    public void Possess(CharacterBase target)
    {
        if (!target) return;
        ControllerBase result = target.Possessed(this);
        if (result == this)
        {
            _character = target;
            OnPossess(target);
        }
    }
    protected virtual void OnUnPossess(CharacterBase oldCharacter) { }
    public void UnPossess()
    {
        if (Character)
        {
            if (Character.Unpossessed(this))
            {
                OnUnPossess(Character);
            }
        }
        _character = null;
    }

    public void CommandMoveToDirection(Vector3 direction)
    {
        if (Character && Character.GetModule<MovementModule>() is IRunnable target) target.MoveToDirection(direction);
    }
    public void CommandMoveToDestination(Vector3 destination, float tolerance)
    {
        if (Character && Character.GetModule<MovementModule>() is IRunnable target) target.MoveToDestination(destination, tolerance);
    }
    public void CommandStop()
    { 
    if (Character && Character.GetModule<MovementModule>() is IRunnable target) target.StopMovement();
    }



}
