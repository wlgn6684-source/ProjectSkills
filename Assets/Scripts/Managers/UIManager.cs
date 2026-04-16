using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public enum UIType
{ 
    None, Loading, Title, _Length, Movable, Menu, Lobby, Map, GameQuit, Profile, Ranking, Passive, Quest, Inventory, Equipment, Market, Customize
}

public enum ScreenChangeType
{ 

    None,
    ScreenChanger,
    _Length
}

public delegate void PopUpEvent(string title, string context, string confirm);

public class UIManager : ManagerBase
{
    public static event PopUpEvent OnPopUp;

    Canvas _mainCanvas;
    public Canvas MainCanvas => _mainCanvas;

    UIBase _movableScreen;
    RectTransform switcherTransform;
    RectTransform createdTransform;
    RectTransform changerTransform;

    GraphicRaycaster _raycaster;
    public GraphicRaycaster Raycaster => _raycaster;

    Dictionary<UIType, UIBase> uiDictionary = new();

    Dictionary<ScreenChangeType, UI_ScreenChanger> screenChangerDictionary = new();

    Rect _uiBoundary;
    public static Rect UIBoundary => GameManager.Instance?.UI?._uiBoundary ?? Rect.zero;

    UIType _currentScreenType = UIType.None;
    public static UIType CurrentScreen => GameManager.Instance?.UI?._currentScreenType ?? UIType.None;

    UI_ScreenChanger currentScreenChanger;

    float _uiScale = 1.0f;
    public static float UIScale => GameManager.Instance?.UI?._uiScale ?? 1.0f;
    public IEnumerator Initialize(GameManager newManager)
    {
        //GameObject.FindGameObjectsWithTag("MainCanvas");
        SetMainCanvas(GetComponentInChildren<Canvas>());
        SetUI(UIType.Loading, GetComponentInChildren<UI_LoadingScreen>());
        yield return null;
    }

    public RectTransform CreateFullScreen(string wantName)
    {

        GameObject Instance = new GameObject(wantName);
        RectTransform result = Instance.AddComponent<RectTransform>();
        result.SetParent(MainCanvas.transform);
        result.SetAsFirstSibling();
        result.anchorMin = Vector3.zero;
        result.anchorMax = Vector3.one;
        result.offsetMin = Vector3.zero;
        result.offsetMax = Vector3.zero;
        result.localScale = Vector3.one;
        return result;


    }

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        createdTransform = CreateFullScreen("CreatedUI");
        _movableScreen = CreateUI(UIType.Movable, "MovableScreen", MainCanvas?.transform);
        switcherTransform = CreateFullScreen("ScreenSwitcher");
        CreateUI(UIType.Title, "TitleScreen", switcherTransform);
        CreateUI(UIType.Lobby, "LobbyScreen", switcherTransform);
        CreateUI(UIType.Map, "MapScreen", switcherTransform);


        foreach (Transform currentTransform in switcherTransform)
        {
            currentTransform.gameObject.SetActive(false);
        }

        changerTransform = CreateFullScreen("ScreenChanger");
        changerTransform.SetAsLastSibling();

        for (ScreenChangeType currentChanger = (ScreenChangeType)1;
            currentChanger < ScreenChangeType._Length;
            currentChanger++)
        {
            GameObject instance = ObjectManager.CreateObject(currentChanger.ToString(), changerTransform);
            if (instance?.TryGetComponent(out UI_ScreenChanger asChanger) ?? false)
            {
                screenChangerDictionary.Add(currentChanger, asChanger);
            }

            instance?.SetActive(false);
        }



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


    protected UIBase CreateUI(UIType wantType, string wantName, Transform parent)
    {
        GameObject instance = ObjectManager.CreateObject(wantName, parent);
        UIBase result = instance?.GetComponent<UIBase>();

        return SetUI(wantType, result);

    }

    protected UIBase CreateUI(UIType wantType, string wantName)
    {
        UIBase result = CreateUI(wantType, wantName, createdTransform ?? MainCanvas?.transform);
        if (result?.GetComponentInChildren<UI_DraggableWindow>())
        {
            _movableScreen?.SetChild(result.gameObject);
        }
        return result;
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
        if (!wantUI) return;

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
        if (uiDictionary.TryGetValue(wantType, out UIBase origin)) return origin;
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
        if (result) EventSystem.current.SetSelectedGameObject(result.gameObject);
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

    protected UIBase OpenScreen(UIType wantType)
    {
        CloseUI(CurrentScreen);
        _currentScreenType = wantType;
        return OpenUI(wantType);
    }
    public static UIBase ClaimOpenScreen(UIType wantType) => GameManager.Instance?.UI?.OpenScreen(wantType);
    public static void ClaimOpenScreen(UIType wantScreen, ScreenChangeType changeType)
    {
        GameManager.Instance?.UI?.OpenScreen(wantScreen, changeType);
    }

    public void OpenScreen(UIType wantScreen, ScreenChangeType changeType)
    {
        ClaimScreenChangeEffect(changeType, () => OpenScreen(wantScreen));
    }

    protected void ScreenChangeEffectStart(ScreenChangeType wantType, System.Action endFunction = null)
    {
        if(currentScreenChanger) return;
        if (screenChangerDictionary.TryGetValue(wantType, out UI_ScreenChanger result ))
        {
            if (!result)
            { 
                endFunction?.Invoke();   
                return; 
            }
            result.gameObject.SetActive(true);
            result.ChangeStart(endFunction);
            currentScreenChanger = result;
        }
        
        else
        {
            endFunction?.Invoke();
        }
    }
    public static void ClaimScreenChangeEffectStart(ScreenChangeType wantType, System.Action endFunction = null) 
        => GameManager.Instance?.UI?.ScreenChangeEffectStart(wantType, endFunction);

    public static void ClaimScreenChangeEffect(ScreenChangeType wantType, System.Action endFunction = null)
       => GameManager.Instance?.UI?.ScreenChangeEffectStart(wantType, endFunction + ClaimScreenChangeEffectEnd);

    protected void ScreenChangeEffectEnd()
    {
        if (currentScreenChanger == null) return;
        GameObject targetObject = currentScreenChanger.gameObject;
        currentScreenChanger.ChangeEnd(() => targetObject.SetActive(false));
        currentScreenChanger = null;
    }
   
    public static void ClaimScreenChangeEffectEnd() => GameManager.Instance?.UI?.ScreenChangeEffectEnd();
    

    public static void ClaimPopup(string title, string context, string confirm)
    { 
        OnPopUp?.Invoke(title, context, confirm);
    }

    public static void ClaimErrorMessage(string context)
    {
        OnPopUp?.Invoke("Error", context, "confirm");
    }
}

