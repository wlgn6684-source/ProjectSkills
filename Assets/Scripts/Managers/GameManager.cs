using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance => _instance;

    UIManager _ui;
    public UIManager UI =>_ui;

    DataManager _data;
    public DataManager Data => _data;

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
        StopCoroutine(initializing);
        DeleteManagers(); //하위 매니저들도 없어지게
    }
    //로딩을 하기 위한 기다림 함수
    //coroutine => 함께      루틴
    //            화면출력   로딩 을 동시에
    //IEnumerator => Start
    //WaitForSeconds을 통해서 시간을 기다릴 수 있게끔
    IEnumerator InitializeManagers()
    {
        yield return CreateManager(ref _ui).Connect(this);
        
        yield return CreateManager(ref _data).Connect(this);
       
        yield return CreateManager(ref _save).Connect(this);
        
        yield return CreateManager(ref _setting).Connect(this);
        
        yield return CreateManager(ref _language).Connect(this);
        
        yield return CreateManager(ref _camera).Connect(this);
        
        yield return CreateManager(ref _audio).Connect(this);
         
        yield return CreateManager(ref _input).Connect(this);
        
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

    void Update()
    {
        
    }


}
