//C++을 하시는 분이다!
//#include다!
//C++은 #include를 해야 대상을 볼 수 있는데
//C#은 사실 모든게 다 보입니다!
//근데 앞에다가 이걸 원래 써야 해요!
//NameSpace기 때문에
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class DataManager : ManagerBase
{


    //프로퍼티는 변수모양이지만 함수
    //                    int GetLoadCount();
    public override int LoadCount
    {
        get
        {
            //Async => 비동기/ 남한테 시키고 제 할 일을 함
            //비동기가 아니라 동기로 만들어야 함
            var task = Addressables.LoadResourceLocationsAsync("Global");
            var result = task.WaitForCompletion();
            int count = result.Count;
            task.Release();
            return count;
        }
    }
    //Interface : 연결고리 => 무엇이 무엇을 사용할 수 있도록 열어주는 기능
    //            GUI : 그래픽 보여줌, 마우스 움직임, 누르 떼기, 클릭하기, 드래그
    //윈도우를 하다가, 맥으로 넘어간다! => 클릭하기 어려울까요?
    //이게 "클릭"이야 => GUI는 클릭이 가능하구나! => GUI이기만 하면 클릭을 지원하겠구나!
    //"어떤 기능이 있을 거야"라는 [약속]이 바로 Interface
    //IOpenable => 열기, 닫기, 토글, 열렸는지 확인도 가능하다!

    //로딩 진행율 => 최대 몇 개인지, 현재 몇 개까지 했는지
    //              현재 / 최대      1 / 100 = 0.01
    //10개
    protected override IEnumerator OnConnected(GameManager newManager)
    {
        //나는 로딩 스크린이 어떻게 생겼는지 모른다.
        //하지만 로딩 스크린을 업데이트해주고싶다.
        UIBase loading = UIManager.ClaimGetUI(UIType.Loading);
        IProgress<int> progressUI = loading as IProgress<int>;
        IStatus<string> statusUI = loading as IStatus<string>;

        int loaded = 0;
        int total = LoadCount;

        //람다는 왜 존재하나? 왜 가르쳐주는 거임?
        //람다 : 이름이 없는 함수 anonymous function
        //함수 안에서 만들어지는 함수 => 변수로 저장할 수 있다
        System.Action ProgressOnLoad = () => 
        {
            loaded++;
            progressUI?.AddCurrent(1);
            statusUI?.SetCurrentStatus($"Load Data ({loaded}/{total})");
        };

        
       

        LoadAllFromAssetBundle<GameObject>("Global", ProgressOnLoad);
        
        
        yield return null;
    }

    protected override void OnDisconnected()
    {

    }

    //파일을 가지고 올 건데, "경로"로 가져오는 것이 중요한 이유!
    //Resources => 유니티에서 Resources폴더를 만들고 나면 사용할 수 있다!
    // Resources/Prefabs/Square
    //드래그 - 드롭으로 넣는게 아니라 파일 경로로 찾는 이유는 무엇일까요?
    //파일이 많으면 드래그하는 데에 한 세월 걸림
    //폴더 째로 로드가 가능하다
    //폴더 내부에 있는 파일을 다른 사람(프로그래머 외)이 수정해도 괜찮다.
    // => 원래 지정되었던 게 전부 풀리고 => 새로 들어온 건 그냥 멀뚱멀뚱 있음
    //기획 문서를 가지고 사람들이 무언가를 찾을 수 있습니다.
    //프로그래밍 팀 따로, 아트 팀 따로, 사운드 팀 따로 ..
    //프로그래밍 팀은 아트가 아직 안들어와도 진행해도 된다.
    //프로그래밍 팀이 그냥 "경로"를 설정해놓고 (예외처리만) 담날 왔습니다.
    //근데 원래 이미지가 없었는데 오늘 켜봤더니 이미지가 적용되어있다!
    bool TryGetFileFromResources<T>(string path, out T result) where T : Object
    {
        //Resources.LoadAll<T>(path);
        result = Resources.Load<T>(path);
        return result != null;
    }

    //1. 경로로 찾는 건 좋은 거라서
    //2. 경로로 찾을 수밖에 없어서
    //파일을.. 클라이언트가 모두 가지고 있을 수 있는가 여부
    //모바일 애플리케이션 => 플레이스토어에서 200mb까지
    //컨텐츠 추가 다운로드 중...
    //Asset Bundle => 경로 (제가 임의로 지정한 카테고리)
    //DLC => 특정 카테고리에 있는 요소를 다운로드 하게 할 것인가 말 것인가?
    //Addressable
    //async함수는 비동기 함수 => 다른 함수와 같이 돌아갈 수 있는 함수!


    //매개변수로  할 일을 넣는 방법
    //컴퓨터에서의 할 일은 기능이기 때문에 Function => 함수
    //함수를 매개변수에 넘겨줄 수 있다.

    
    

    public void SaveDataFile<T>(T target) where T : Object
    {
        if (target == null) return;
        Debug.Log(target);
    }

    // Action => 행동
    // 행동은 언제나 함수 => 반환값이 없는 함수
    // Action<> => void Function()
    // Action<int> => void Function(int a)
    // Action<float> => void Function(float a)
    // Action<int, float> => void Function(int a, float b)
    //최대 16개의 매개변수까지 등록할 수 있다.
    //Func => 함수
    //수식은 반환값이 있어야 하니까 => 맨 오른쪽에 반환 자료형
    //Func<float> => float Function()
    //Func<float, int> => float Function(float a)
    //Func<float, string, int> => float Function(float a, string b)

    public async void LoadAllFromAssetBundle<T>(string label, System.Action actionForEachLoad) where T : Object
    {   
        //람다 함수 (매개변수) => {내용}
        var finder = Addressables.LoadAssetsAsync<T>(label, (T loaded) =>
        { 
            SaveDataFile(loaded);
            actionForEachLoad();
        });
        await finder.Task;
    }

    public async void LoadFileFromAssetBundle<T>(string address) where T : Object
    {
        var finder = Addressables.LoadAssetAsync<T>(address);
        await finder.Task; //Start/Run에 해당하는 부분
        SaveDataFile(finder.Result);
        

        
        //A-는 뜻이 뭘까?
        //An-
        //"~이 아닌"
        //"반대되는" 접두사
        //Tan => ATan
        //동기화하지 않는다! => 비동기
        //프로세스가 동기화되지 않는다
        //=> 하나의 프로세스로 돌리는 것이 아니다
        //                    유니티
        //=> 멀티 스레드 <-> 싱글 스레드
        //       Thread
        //       줄, 실
        //한 번에 실행하는 기능의 개수
        //밥 먹으면서 게임하면서 유튜브보면서 음악틀면서
        //시간이 빠르게 완료될 수 있다
        //게임을 하는 동안에 밥을 먹고 있단 말이죠.
        //지금 한타하느라 스킬을 조준해야 하는데, 숟가락을 들고 있어서
        //근데.. 저희는 그 상황에서 "결정"을 하잖아요?
        //손을 어따 써야 할지? => 우선순위가 있어야 함!
        //컴퓨터 입장에서는.. 지금 할 일 스레드마다 하나씩
        //어차피 이거 안하고 다음으로 넘어갈 수가 없습니다.
        //데미지 주는 기능이다!
        //생명력 감소하려고 했는데.. 생명력을 누가 쓰고 있어서 못바꾼다!
        //생명력 감소 안하고 죽었는지 체크할 것인가?
        // => 데드락
        //원래 밥만 먹었을 때보다 밥 먹는 시간은 느려진다
        //다 같이 하는데 왜요?
        //밥먹는 애, 유튜브보는 애, 게임하는 애, 음악 듣는 애
        //   O           O            X            O
        //다른 애들이 전부 게임하는 애 기다렸다가 다음 작업을 해야해요!
        //게임하는 애가 뭔가 중요한 변화를 주고 끝낼 수도 있잖아요?
    }
}