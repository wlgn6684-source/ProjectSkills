using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    ControllerBase _controller;
    public ControllerBase Controller => _controller;

    public virtual string Displaying => "character";
    public ControllerBase Possessed(ControllerBase from)
    {
        if (Controller) Unpossessed();
        _controller = from;
        OnPossessed(Controller);
        return Controller;
    }


    protected virtual void OnPossessed(ControllerBase newController){}

    protected virtual void OnUnpossessed(ControllerBase oldController){}
    public void Unpossessed()
    {
        if(Controller) OnUnpossessed(Controller);
        _controller = null;
    }

    public bool Unpossessed(ControllerBase oldController)
    { 
        if (Controller != oldController) return false;
        Unpossessed();
        return true;
    }

}
