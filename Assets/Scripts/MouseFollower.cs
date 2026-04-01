using UnityEngine;

public class MouseFollower : MonoBehaviour, IFunctionable
{
    void Start()
    {
        RegistrationFunctions();
    }

    void OnDestroy()
    {
        UnregistrationFunctions();
    }
    public void RegistrationFunctions()
    {
        InputManager.OnMouseLeftUp += CreateToMouse;
        InputManager.OnMouseRightDown += DestroyToMouse;
        
    }
    public void UnregistrationFunctions()
    {
        InputManager.OnMouseLeftUp -= CreateToMouse;
        InputManager.OnMouseRightDown -= DestroyToMouse;
        
    }

    void CreateToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        GameObject inst = ObjectManager.CreateObject("PopUp");
        inst.transform.position = worldPosition;
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    { 
        transform.position = worldPosition;
    }
    void DestroyToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        ObjectManager.DestroyObject( GameManager.Instance.Input.GetGameObjectUnderCursor());
    }

}
