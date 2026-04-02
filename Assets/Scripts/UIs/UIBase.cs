using UnityEngine;

public class UIBase : MonoBehaviour
{
    public virtual void Registration(UIManager manager)
    { 
        
    }

    public virtual void UnRegistration(UIManager manager)
    { 
    
    }

    public GameObject SetChild(GameObject newChild)
    {
        if (!newChild) return null;
        //오브젝트의 자식을 추가하는 방법
        newChild.transform.SetParent(transform);

        return OnSetChild(newChild);
    }

    protected virtual GameObject OnSetChild(GameObject newChild)
    { 
        return newChild;
    }

    public void UnsetChild(GameObject oldChild)
    {
        if (!oldChild) return;
        if (oldChild.transform.parent == transform)
        { 
            oldChild.transform.SetParent(null); 
        }
        OnSetChild(oldChild);
    }
    protected virtual void OnUnsetChild(GameObject oldChild)
    { 
        
    }

}
