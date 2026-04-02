using UnityEngine;

public class UI_MovableScreen : UIBase
{   
    Vector3 popupPosition = Vector3.zero;
    Vector3 popupShift = new(20.0f, -20.0f);

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        UIManager.OnPopUp -= PopUp;
        UIManager.OnPopUp += PopUp;
    }

    public override void UnRegistration(UIManager manager)
    {
        base.UnRegistration(manager);
        UIManager.OnPopUp -= PopUp;
    }
    protected override GameObject OnSetChild(GameObject newChild)
    {

        UIManager.ClaimSetUI(newChild);
        
        return base.OnSetChild(newChild);
    }
    protected override void OnUnsetChild(GameObject oldChild)
    {
        UIManager.ClaimUnsetUI(oldChild);
        base.OnUnsetChild(oldChild);
    }

    void PopUp(string title, string context, string confirm)
    {
        GameObject newChild = SetChild(ObjectManager.CreateObject("PopUp",transform));
        if (newChild)
        {
            if(newChild.TryGetComponent(out ISystemMessagePossible target))
                { 
                    target.SetSystemMessage(title, context, confirm);
                }
            if(newChild.TryGetComponent(out IConfirmable confirmTarget))
                {
                confirmTarget.SetConfirmAction(() =>
                    {
                        UnsetChild(newChild);
                        ObjectManager.DestroyObject(newChild);
                    });
                }
            newChild.transform.localPosition = popupPosition;
            popupPosition += popupShift;
        }
        
    }
}
