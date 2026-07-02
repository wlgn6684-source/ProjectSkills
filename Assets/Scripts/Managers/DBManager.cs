using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DBManager : ManagerBase
{
    FirebaseAuth authentication;
    FirebaseUser user;
    DatabaseReference rootDB;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(InitializeFireBase);
        yield return null;
    }

    protected override void OnDisconnected()
    {

    }

    void InitializeFireBase(Task<DependencyStatus> task)
    {
        if (task.Result == DependencyStatus.Available)
        {
            authentication = FirebaseAuth.DefaultInstance;

            user = authentication.CurrentUser;

            rootDB = FirebaseDatabase.DefaultInstance.RootReference;

            GuestLogin();

            Debug.Log("Firebase Initialized");

        }

        else
        {
            Debug.LogError($"Fail to Intialize FireBase : {task.Exception}");
        }
    }

    public async void GuestLogin()
    {
        if (user is not null)
        {
            Debug.Log($"Login Success ({user.UserId})");

            UserData resultData =
    await ReadDataAsync<UserData>(
        "users",
        "userData",
        user.UserId);

            if (resultData != null)
            {
                Debug.Log("===== Firebase 데이터 읽기 성공 =====");
                Debug.Log($"닉네임 : {resultData.nickname}");
                Debug.Log($"레벨 : {resultData.userLevel}");
                Debug.Log($"골드 : {resultData.money}");
                Debug.Log($"출석 횟수 : {resultData.attendTime}");
            }

            return;
            //if (authentication is null) return;
            //if (user is not null)
            //{
            //    Debug.LogError($"Login Failed : Already Has Login Data ({user.IsValid()},{user.UserId})");
            //    UserData resultData = await ReadDataAsync<UserData>("users", "userData", user.UserId);
            //    if (resultData is not null)
            //    {
            //        Debug.Log(resultData.nickname);
            //    }
            //    else
            //    {
            //        WriteData(MakeNewUserData("NoNamed"), "users", "userData", user.UserId);
            //    }
            //    return;
            //}
            //
            //await authentication.SignInAnonymouslyAsync().ContinueWithOnMainThread(OnLoginResult);
        }
    }

    private async void OnLoginResult(Task<AuthResult> task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError($"Fail to Sign in : {task.Exception}");
            return;
        }

        user = task.Result.User;

        WriteData(
            MakeNewUserData("천마"),
            "users",
            "userData",
            user.UserId);

        Debug.Log($"Sign in Succeed: {user.UserId}");

        await Task.Delay(1000);

        await AttendReward();
    }

    [Serializable]
    public class UserData
    {
        public string nickname;
        public string assignDate;
        public int userLevel;
        public int money;
        public int attendTime;
        public string lastAttendDate;   

    }

    public TMPro.TMP_InputField emailInput;
    public TMPro.TMP_InputField passwordInput;
    
    public TMPro.TMP_InputField nickNameInput;

    public void MakeUserData()
    {
        WriteData(MakeNewUserData(nickNameInput.text), "users", "userData", user.UserId);
    }

    UserData MakeNewUserData(string wantNickname) => new()
    {
        nickname = wantNickname,
        assignDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        userLevel = 0,
        money = 3000,
        attendTime = 0,
        lastAttendDate = ""
    };

    public DatabaseReference GetFinalDirectory(DatabaseReference root, params string[] directory)
    {

        if (directory is null || directory.Length == 0) return root;
        DatabaseReference currentReference = root;
        foreach (string currentChild in directory)
        {
            currentReference = currentReference.Child(currentChild);
        }
        return currentReference;
    }

    public void WriteData(object wantData, params string[] directory)
    {

        if (rootDB is null || wantData is null) return;

        string jsonData = JsonUtility.ToJson(wantData);
        GetFinalDirectory(rootDB, directory).SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(OnTaskResult);
    }

    public void WriteData(Dictionary<string, object> changes, params string[] directory)
    {
        if (rootDB is null || changes is null) return;
        GetFinalDirectory(rootDB, directory).UpdateChildrenAsync(changes).ContinueWithOnMainThread(OnTaskResult);

    }

    public async Task WriteDataAsync(object wantData,params string[] directory)
    {
        if (rootDB == null || wantData == null)
            return;

        string jsonData = JsonUtility.ToJson(wantData);

        await GetFinalDirectory(rootDB, directory).SetRawJsonValueAsync(jsonData);
    }

    public void ReadData(Action<Task<DataSnapshot>> OnReadData, params string[] directory)
    {
        GetFinalDirectory(rootDB, directory).GetValueAsync().ContinueWithOnMainThread(OnReadData);

    }
    public IEnumerator ReadDataCoroutine(Action<Task<DataSnapshot>> OnReadData, params string[] directory)
    {
        Task<DataSnapshot> readTask = GetFinalDirectory(rootDB, directory).GetValueAsync();
        yield return readTask.WaitforTask();
        OnReadData.Invoke(readTask);
    }

    public async Task<T> ReadDataAsync<T>(params string[] directory)
    {
        DataSnapshot currentTask = await GetFinalDirectory(rootDB, directory).GetValueAsync();
        if (currentTask is null) return default;
        if (!currentTask.Exists) return default;
        try
        {
            if (currentTask.HasChildren)
            {
              return JsonUtility.FromJson<T>(currentTask.GetRawJsonValue());
            }
            return (T)System.Convert.ChangeType(currentTask.Value, typeof(T));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return default;
        }
    }

        void OnTaskResult(Task task)
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError(task.Exception);
            }
        }
    public async Task<bool> AttendReward()
    {
        Debug.Log("AttendReward 호출됨");
        Debug.Log($"Current User : {user?.UserId}");
        Debug.Log("========== 출석 시스템 시작 ==========");

        UserData userData =
            await ReadDataAsync<UserData>(
                "users",
                "userData",
                user.UserId);

        if (userData == null)
        {
            Debug.LogError("유저 데이터를 찾을 수 없습니다.");
            return false;
        }

        Debug.Log($"닉네임 : {userData.nickname}");
        Debug.Log($"현재 골드 : {userData.money}");
        Debug.Log($"누적 출석 횟수 : {userData.attendTime}");
        Debug.Log($"마지막 출석 날짜 : {userData.lastAttendDate}");

        string today = DateTime.Now.ToString("yyyyMMdd");

        if (userData.lastAttendDate == today)
        {
            Debug.Log("오늘 이미 출석 보상을 지급받았습니다.");
            return false;
        }

        // 롤백용 백업
        UserData backupData =
            JsonUtility.FromJson<UserData>(
                JsonUtility.ToJson(userData));

        try
        {
            Debug.Log("출석 보상 지급 시작");

            userData.money += 500;
            userData.attendTime++;
            userData.lastAttendDate = today;

            Debug.Log($"지급 골드 : +500");
            Debug.Log($"변경된 골드 : {backupData.money} -> {userData.money}");
            Debug.Log($"출석 횟수 : {backupData.attendTime} -> {userData.attendTime}");
            Debug.Log($"출석 날짜 갱신 : {userData.lastAttendDate}");

            await WriteDataAsync(userData,"users","userData", user.UserId);

            Debug.Log("Firebase 저장 완료");


            Debug.Log("Firebase 저장 요청 완료");
            Debug.Log("========== 출석 처리 완료 ==========");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"출석 처리 실패 : {e.Message}");

            WriteData(
                backupData,
                "users",
                "userData",
                user.UserId);

            Debug.Log("롤백 수행");
            Debug.Log($"골드 복구 : {backupData.money}");
            Debug.Log($"출석 횟수 복구 : {backupData.attendTime}");
            Debug.Log("========== 롤백 완료 ==========");

            return false;
        }
    }

    public async void TestAttendReward()
    {
        bool result = await AttendReward();

        if (result)
        {
            Debug.Log("출석 보상 테스트 성공");
        }
        else
        {
            Debug.Log("출석 보상 테스트 실패");
        }
    }

    public async void Register()
    {
        if (authentication == null)
            return;

        string email = emailInput.text;
        string password = passwordInput.text;
        string nickname = nickNameInput.text;

        try
        {
            AuthResult result =
                await authentication
                    .CreateUserWithEmailAndPasswordAsync(
                        email,
                        password);

            user = result.User;

            UserData newUser = MakeNewUserData(nickname);

            WriteData(
                newUser,
                "users",
                "userData",
                user.UserId);

            Debug.Log("회원가입 성공");
        }
        catch (Exception e)
        {
            Debug.LogError($"회원가입 실패 : {e}");
        }
    }

    public async void Login()
    {
        if (authentication == null)
            return;

        try
        {
            AuthResult result =
                await authentication
                    .SignInWithEmailAndPasswordAsync(
                        emailInput.text,
                        passwordInput.text);

            user = result.User;

            UserData data =
                await ReadDataAsync<UserData>(
                    "users",
                    "userData",
                    user.UserId);

            Debug.Log($"로그인 성공 : {data.nickname}");
        }
        catch (Exception e)
        {
            Debug.LogError($"로그인 실패 : {e}");
        }

    }
    public void Logout()
    {
        authentication.SignOut();

        user = null;

        Debug.Log("로그아웃 완료");
    }
}
