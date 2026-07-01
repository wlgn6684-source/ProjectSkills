using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
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
        if (authentication is null) return;
        if (user is not null)
        {
            Debug.LogError($"Login Failed : Already Has Login Data ({user.IsValid()},{user.UserId})");
            UserData resultData = await ReadDataAsync<UserData>("users", "userData", user.UserId);
            if (resultData is not null)
            {
                Debug.Log(resultData.nickname);
            }
            else
            { 
                WriteData(MakeNewUserData("NoNamed"),"users", "userData", user.UserId);
            }
                return;
        }

        await authentication.SignInAnonymouslyAsync().ContinueWithOnMainThread(OnLoginResult);
    }

    private void OnLoginResult(Task<AuthResult> task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError($"Fail to Sign in : {task.Exception}");
            return;
        }

        user = task.Result.User;
        WriteData(MakeNewUserData("천마"), "users", "userData");
        Debug.Log($"Sign in Succeed: {user.UserId}");
    }

    [Serializable]
    public class UserData
    {
        public string nickname;
        public DateTime assignData;
        public int userLevel;
        public int money;
        public int attendtime;
    }

    public TMPro.TMP_InputField nickNameInput;

    public void MakeUserData()
    {
        WriteData(MakeNewUserData(nickNameInput.text), "user", "userData", user.UserId);
    }

    UserData MakeNewUserData(string wantNickname) => new()
    {
        nickname = wantNickname,
        assignData = DateTime.Now,
        userLevel = 0,
        money = 3000,
        attendtime = 0
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
}
