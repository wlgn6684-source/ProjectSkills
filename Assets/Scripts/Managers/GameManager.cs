using System.Collections;
using UnityEngine;

public delegate void InitializeEvent();
public delegate void UpdateEvent(float deltaTime);
public delegate void DestroyEvent();



public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance => _instance;

    UIManager _ui;
    public UIManager UI =>_ui;

    DataManager _data;
    public DataManager Data => _data;

    ObjectManager _objectM;
    public ObjectManager ObjectM => _objectM;

    SaveManager _save;
    public SaveManager Save => _save;

    SettingManager _setting;
    public SettingManager Setting => _setting;

    LanguageManager _language;
    public LanguageManager Language => _language;

    AudioManager _audio;
    public AudioManager Audio => _audio;

    CameraManager _camera;
    public CameraManager Camera => _camera;

    InputManager _input;
    public InputManager Input => _input;

    IEnumerator initializing; //초기화 중 코루틴


    public static event InitializeEvent   OnInitializeManager;
    public static event InitializeEvent   OnInitializeController;
    public static event InitializeEvent   OnInitializeCharacter;
    public static event InitializeEvent   OnInitializeObject;
    public static event UpdateEvent       OnUpdateManager;
    public static event UpdateEvent       OnUpdateController;
    public static event UpdateEvent       OnUpdateCharater;
    public static event UpdateEvent       OnUpdateObject;

    public static event UpdateEvent       OnPhysicsCharacter;
    public static event UpdateEvent       OnPhysicsObject;

    public static event DestroyEvent      OnDestroyManager;
    public static event DestroyEvent      OnDestroyController;
    public static event DestroyEvent      OnDestroyCharater;
    public static event DestroyEvent      OnDestroyObject;

    [SerializeField] UIType startScreen;
    
    bool isPlaying = true;
   bool isLoading = true;

    //Awake : 이 친구가 시작할 때 (깨어남)
    //OnEnabled : 이 친구가 시작할 때
    //OnDisabled
    //Reset : 일을 시작하기 위해 초기화 /준비
    //Start : 이 친구가 시작할 때

    void Awake()
    {
        //게임 매니저가 일어나서 처음으로 할 일
        //진정한 게임 매니저를 가르는 사투
        //게임 매니저가 둘이다
        //두개는 존재 할 수 없다
        //먼저 온 매니저를 죽인다
        //먼저 온 매니저를 인정한다
        //하던 놈을 그대로 유지하는게 더 좋음
        if (_instance == null)
        {
            _instance = this;
        }

        else
        {
            Destroy(this);
            return;
        }
        //게임에 단 하나만 유지하도록 하는 패턴으로 싱글턴 패턴이라고 함
        //반환형식은 IEnimerator (반복자) 반복해서 함수가 실행됨 프레임 단위로 기다렸다가 실핼
        //실행하고 yield 양보 했다가 다음 프레임에 또 나와서 실행 및 반복
        //                          WaitForSeconds(10.0f)
        initializing = InitializeManagers();
        StartCoroutine(initializing);
        
    }

    void OnDestroy() //매니저가 없어지면
    {
        if(initializing != null) StopCoroutine(initializing);
        DeleteManagers(); //하위 매니저들도 없어지게
    }
    //로딩을 하기 위한 기다림 함수
    //coroutine => 함께      루틴
    //            화면출력   로딩 을 동시에
    //IEnumerator => Start
    //WaitForSeconds을 통해서 시간을 기다릴 수 있게끔
    IEnumerator InitializeManagers()
    {   
       int totalLoadCount = 0;
       totalLoadCount += CreateManager(ref _ui).LoadCount;
       totalLoadCount += CreateManager(ref _data).LoadCount;
       totalLoadCount += CreateManager(ref _objectM).LoadCount;
       totalLoadCount += CreateManager(ref _save).LoadCount;
       totalLoadCount += CreateManager(ref _setting).LoadCount;
       totalLoadCount += CreateManager(ref _language).LoadCount;
       totalLoadCount += CreateManager(ref _camera).LoadCount;
       totalLoadCount += CreateManager(ref _audio).LoadCount;
       totalLoadCount += CreateManager(ref _input).LoadCount;


        yield return UI.Initialize(this);
        UIBase loadingUI = UIManager.ClaimOpenScreen(UIType.Loading);
        IProgress<int> loadingProgress = loadingUI as IProgress<int>;

        loadingProgress?.Set(0, totalLoadCount);
        yield return Data.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return ObjectM.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return UI.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Save.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Setting.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Language.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Camera.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Audio.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return Input.Connect(this);
        loadingProgress?.AddCurrent(1);
        yield return null;

        Pause();
        
        InputManager.OnAnyKey -= UnPause;
        InputManager.OnAnyKey += UnPause;

        loadingProgress.SetComplete();

        yield return new WaitUntil(() => isPlaying);

        UIManager.ClaimOpenScreen(startScreen, ScreenChangeType.ScreenChanger);
        InputManager.OnAnyKey -= UnPause;
        isLoading = false;



    }
    //
    //_input에다가 값을 넣고 싶다면
    //다른데에서는 _audio에다가 값을 넣는다
    //대상이 되는 변수를 가져와야함
    //원본 값을 바꿔야함
    //                                    원본 값을 참조한다
    //                                    원본 값을 변수로 만들어주기
    //                                    Reference => ref

    void DeleteManagers()
    {
        //유저입력    InputManager
        Input?.Disconnected();
        //오브젝트    ObjectManager
        ObjectM?.Disconnected();
        //오디오      AudioManager
        Audio?.Disconnected();
        //언어        LanguageManager
        Language?.Disconnected();
        //세팅        SettingManager
        Setting?.Disconnected();
        //세이브      SaveManager
        Save?.Disconnected();
        //카메라      CameraManager
        Camera?.Disconnected();
        //UI         UIManager
        UI?.Disconnected();
        //데이터파일  DataManager
        Data?.Disconnected();
    }

    ManagerType CreateManager<ManagerType>(ref ManagerType targetVarirable) where ManagerType : ManagerBase
    {
        if (targetVarirable == null)
        {   
            //컴포넌트를 어떻게 추가해야 하나?

            targetVarirable = this.TryAddComponent<ManagerType>();
            //targetVarirable.Connect(this);
        } 
        return targetVarirable;
    }


    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif

    }

    public static void Pause()
    {
        Instance.isPlaying = false;
    }
    public static void UnPause()
    {
        Instance.isPlaying = true;
    }

    public void InvokeInitiallizeEvent(ref InitializeEvent OriginEvent)
    {
        if (OriginEvent != null)
        {
            InitializeEvent CurrentEvent = OriginEvent;
            OriginEvent = null;
            CurrentEvent.Invoke();
        }
    }

    public void InvokeDestroyEvent(ref DestroyEvent OriginEvent)
    {
        if (OriginEvent != null)
        { 
            DestroyEvent CurrentEvent = OriginEvent;
            OriginEvent = null;
            CurrentEvent.Invoke();
        }
    }
    void Update()
    {
        if (isLoading) return;

        //게임 진행을 할 수 있는지 여부를 조정할 수도 있다
        //초기화 해야하는지 하지 말아야 하는지
        //pause상태 => 업데이트를 하지 않는다.


        //매니저를 초기화
        InvokeInitiallizeEvent(ref OnInitializeManager);
        //OnInitializeManager?.Invoke();
        //OnInitializeManager = null;
        //캐릭터를 초기화
        InvokeInitiallizeEvent(ref OnInitializeCharacter);
        //if (OnInitializeCharacter != null)
        //{
        //    InitializeEvent currentInitializeCharater = OnInitializeCharacter;
        //    OnInitializeCharacter = null;
        //    currentInitializeCharater.Invoke();
        //}
        //OnInitializeCharater?.Invoke();
        //OnInitializeCharater = null;
        //컨트롤러를 초기화
        InvokeInitiallizeEvent(ref OnInitializeController);
        //OnInitializeController?.Invoke();
        //OnInitializeController = null;
        //오브젝트를 초기화
        InvokeInitiallizeEvent(ref OnInitializeObject);
        //OnInitializeObject?.Invoke();
        //OnInitializeObject = null;

        if (isPlaying)
        { 
            float deltaTime = Time.deltaTime;
            //매니저가 업데이트 하는 경우
            OnUpdateManager?.Invoke(deltaTime);
            //컨트롤러를 업데이트 한다
            OnUpdateController?.Invoke(deltaTime);
            //캐릭터를 업데이트 한다
            OnUpdateCharater?.Invoke(deltaTime);
            //오브젝트를 업데이트를 한다
            OnUpdateObject?.Invoke(deltaTime);
        }

        //오브젝트를 제거한다
        InvokeDestroyEvent(ref OnDestroyObject);
        //OnDestroyObject?.Invoke();
        //컨트롤러를 제거한다
        InvokeDestroyEvent(ref OnDestroyController);
        //OnDestroyController?.Invoke();
        //캐릭터를 제거한다
        InvokeDestroyEvent(ref OnDestroyCharater);
        //OnDestroyCharater?.Invoke();
        //매니저를 제거한다
        InvokeDestroyEvent(ref OnDestroyManager);
        //OnDestroyManager?.Invoke();


    }

    void FixedUpdate()
    {
        if (isLoading || !isPlaying) return;

        float deltaTime = Time.fixedDeltaTime;

        OnPhysicsCharacter?.Invoke(deltaTime);
        OnPhysicsObject?.Invoke(deltaTime);
    }
}
