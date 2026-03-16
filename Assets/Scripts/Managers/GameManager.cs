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
        }
        //게임에 단 하나만 유지하도록 하는 패턴으로 싱글턴 패턴이라고 함
    }

    void InitializeManagers()
    {
        CreateManager(ref _ui);
        
        CreateManager(ref _data);
       
        CreateManager(ref _save);
        
        CreateManager(ref _setting);
       
        CreateManager(ref _language);
       
        CreateManager(ref _camera);
      
        CreateManager(ref _audio);
        
        CreateManager(ref _input);
        
    }
    //
    //_input에다가 값을 넣고 싶다면
    //다른데에서는 _audio에다가 값을 넣는다
    //대상이 되는 변수를 가져와야함
    //원본 값을 바꿔야함
    //                                    원본 값을 참조한다
    //                                    원본 값을 변수로 만들어주기
    //                                    Reference => ref
    ManagerType CreateManager<ManagerType>(ref ManagerType targetVarirable) where ManagerType : ManagerBase
    {
        if (targetVarirable == null)
        {
            targetVarirable = gameObject.AddComponent<ManagerType>();
            targetVarirable.Connect(this);
        } 
        return targetVarirable;
    }

    void Update()
    {
        
    }


}
