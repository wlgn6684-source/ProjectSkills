using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public enum UIType
{ 
    None, Loading, Title, _Length, Movable, Menu, Info
}

public delegate void PopUpEvent(string title, string context, string confirm);

public class UIManager : ManagerBase
{   
    public static event PopUpEvent OnPopUp;

    Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    GraphicRaycaster _raycaster;
    public GraphicRaycaster Raycaster => _raycaster;

    Dictionary<UIType, UIBase> uiDictionary = new();

    Rect _uiBoundary;
    public static Rect UIBoundary => GameManager.Instance?.UI?._uiBoundary ?? Rect.zero;

    float _uiScale = 1.0f;
    public static float UIScale => GameManager.Instance?.UI?._uiScale ?? 1.0f;
    public IEnumerator Initialize(GameManager newManager)
    {
        //GameObject.FindGameObjectsWithTag("MainCanvas");
        SetMainCanvas(GetComponentInChildren<Canvas>());
        SetUI(UIType.Loading, GetComponentInChildren<UI_LoadingScreen>());
        yield return null;
    }

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        UIBase movableUI = CreateUI(UIType.Movable, "MovableScreen");
        yield return null;
        
       
    }
    protected override void OnDisconnected()
    {
        UnSetAllUI();
    }

    protected void SetMainCanvas(Canvas newCanvas)
    {
        _mainCanvas = newCanvas;
        if (_mainCanvas)
        {
            _raycaster = _mainCanvas.GetComponent<GraphicRaycaster>();
            //RectTransform mainRect = MainCanvas.GetComponent<RectTransform>();
            if (_mainCanvas.transform is RectTransform mainRectTransform)
            {
                _uiScale = mainRectTransform.lossyScale.x;
                _uiBoundary = mainRectTransform.rect;
                //_uiBoundary.size *= _uiScale;
                //_uiBoundary.position *= _uiBoundary.size / 1.0f;
            }
           
        }
        else 
        {
            _raycaster = null;
        }
            
    }

    
    protected UIBase CreateUI(UIType wantType, string wantName)
    {
        GameObject instance = ObjectManager.CreateObject(wantName, _mainCanvas.transform);
        UIBase result = instance?.GetComponent<UIBase>();
       
        return SetUI(wantType, result);
        
    }

    public static UIBase ClaimCreateUI(UIType wantType, string wantName) => GameManager.Instance.UI?.CreateUI(wantType, wantName);
    protected void UnSetUI(UIType wantType)
    {
        if (uiDictionary.TryGetValue(wantType, out UIBase found))
        {
            UnSetUI(found);
            uiDictionary.Remove(wantType);
        }
    }
    protected void UnSetUI(UIBase wantUI)
    { 
        if(!wantUI) return;

        wantUI.UnRegistration(this);
    }

    public static void ClaimUnsetUI(UIBase wantUI) => GameManager.Instance?.UI?.UnSetUI(wantUI);
    public static void ClaimUnsetUI(GameObject wantObject) => ClaimUnsetUI(wantObject?.GetComponent<UIBase>());

    protected void UnSetAllUI()
    {
        foreach (UIBase ui in uiDictionary.Values)
        {
            UnSetUI(ui);
        }
        uiDictionary.Clear();
    }

    public static UIBase ClaimSetUI(UIBase wantUI) => GameManager.Instance?.UI?.SetUI(wantUI);
    public static UIBase ClaimSetUI(GameObject wantObject) => ClaimSetUI(wantObject?.GetComponent<UIBase>());
    protected UIBase SetUI(UIBase wantUI)
    {
        wantUI.Registration(this);
        return wantUI;
    }
    protected UIBase SetUI(UIType wantType, UIBase wantUI)
    {
        //InventoryType, InventoryInstance
        if (wantUI == null) return null;
        if(uiDictionary.TryGetValue(wantType, out UIBase origin)) return origin;
        uiDictionary.Add(wantType, wantUI);
        return SetUI(wantUI);
    }

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

    public static void ClaimErrorMessage(string context)
    {
        OnPopUp?.Invoke("Error", context, "confirm");
    }
}
