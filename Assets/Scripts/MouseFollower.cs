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
        InputManager.OnCancelButton += (value) => UIManager.ClaimPopup("인식 오류", "ㅇㅇ", "ㅇㅇ");
        
    }
    public void UnregistrationFunctions()
    {

        InputManager.OnCancelButton -= (value) => UIManager.ClaimPopup("인식 오류", "ㅇㅇ", "ㅇㅇ");

    }

    void CreateToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        GameObject inst = ObjectManager.CreateObject("Pss");
        if(inst) inst.transform.position = worldPosition;
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
