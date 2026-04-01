using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{ 
    None, Loading, Title, _Length, Movable
}

public delegate void PopUpEvent(string title, string context, string confirm);

public class UIManager : ManagerBase
{   
    public static event PopUpEvent OnPopUp;

    Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    Dictionary<UIType, UIBase> uiDictionary = new();
    public IEnumerator Initialize(GameManager newManager)
    {
        _mainCanvas = GetComponentInChildren<Canvas>();
        //GameObject.FindGameObjectsWithTag("MainCanvas");
        SetUI(UIType.Loading, GetComponentInChildren<UI_LoadingScreen>());
        yield return null;
    }

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        UIBase movableUI = CreateUI(UIType.Movable, "MovableScreen");
        yield return null;
        movableUI.SetChild(ObjectManager.CreateObject("PopUp"));
        yield return null;
    }
    protected override void OnDisconnected()
    {

    }

    protected UIBase CreateUI(UIType wantType, string wantName)
    {
        GameObject instance = ObjectManager.CreateObject(wantName, _mainCanvas.transform);
        UIBase result = instance?.GetComponent<UIBase>();
       
        return SetUI(wantType, result);
        
    }

    protected UIBase SetUI(UIType wantType, UIBase wantUI)
    {
        //InventoryType, InventoryInstance
        if (wantUI == null) return null;
        if(uiDictionary.TryGetValue(wantType, out UIBase origin)) return origin;
        uiDictionary.Add(wantType, wantUI);
        return wantUI;
    }
    public static UIBase ClaimSetUI(UIType wantType, UIBase wantUI) => GameManager.Instance?.UI?.SetUI(wantType, wantUI);

    protected UIBase GetUI(UIType wantType)
    { 
       if (uiDictionary.TryGetValue(wantType, out UIBase result)) return result;//������ result��ȯ
       else return null;
    }
    public static UIBase ClaimGetUI(UIType wantType) => GameManager.Instance?.UI?.GetUI(wantType);
    protected UIBase OpenUI(UIType wantType)
    {   
         //result가 몰라도 IOpenabla이면 열게해준다, 세부 요소는 몰라도 상위요소만으로 실행가능
        //리스코프 반환 원칙
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Open();

        //IOpenable opener = result as IOpenable;
        //if (opener != null) opener.Open();
        

        return result;
    }
    public static UIBase ClaimOpenUI(UIType wantType) => GameManager.Instance?.UI?.OpenUI(wantType);

    protected UIBase CloseUI(UIType wantType) 
    {
        UIBase result = GetUI(wantType);
        
        if (result is IOpenable asOpenable) asOpenable.Close();
        return result;
    }
    public static UIBase ClaimCloseUI(UIType wantType) => GameManager.Instance?.UI?.CloseUI(wantType);

    protected UIBase ToggleUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        //result?.SetActive(result.activeInHierarchy);
        if (result is IOpenable asOpenable) asOpenable.Toggle();
        return result;
    }
    public static UIBase ClaimToggleUI(UIType wantType) => GameManager.Instance?.UI?.ToggleUI(wantType);

    public static void ClaimPopup(string title, string context, string confirm)
    { 
        OnPopUp?.Invoke(title, context, confirm);
    }

    public static void ClaimErrorMessage(string title, string context, string confirm)
    {
        OnPopUp?.Invoke("Error", context, "confirm");
    }
}
