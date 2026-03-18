using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{ 
    None, Loading, Title, _Length
}

public class UIManager : ManagerBase
{
    Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    Dictionary<UIType, UIBase> uiDictionary = new();
    protected override IEnumerator OnConnected(GameManager newManager)
    {
        _mainCanvas = GetComponentInChildren<Canvas>();
        //GameObject.FindGameObjectsWithTag("MainCanvas");
        Debug.Log(MainCanvas);
        yield return null;
    }
    protected override void OnDisconnected()
    {

    }

    public UIBase SetUI(UIType wantType, UIBase wantUI)
    {
        //InventoryType, InventoryInstance
        if (wantUI == null) return null;
        if(uiDictionary.TryGetValue(wantType, out UIBase origin)) return origin;
        uiDictionary.Add(wantType, wantUI);
        return wantUI;
    }

    public UIBase GetUI(UIType wantType)
    { 
       if (uiDictionary.TryGetValue(wantType, out UIBase result)) return result;//있으면 result변환
       else return null;
    }
    public UIBase OpenUI(UIType wantType)
    {   
        //result가 몰라도 IOpenabla이면 열게해준다, 세부 요소는 몰라도 상위요소만으로 실행가능
        //리스코프 반환 원칙
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Open();

        //IOpenable opener = result as IOpenable;
        //if (opener != null) opener.Open();
        

        return result;
    }

    public UIBase CloseUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        if (result is IOpenable asOpenable) asOpenable.Close();
        return result;
    }

    public UIBase ToggleUI(UIType wantType)
    {
        UIBase result = GetUI(wantType);
        //result?.SetActive(result.activeInHierarchy);
        if (result is IOpenable asOpenable) asOpenable.Toggle();
        return result;
    }
}
