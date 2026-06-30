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

    public void GuestLogin()
    {
        if (authentication is null) return;
        if (user is not null)
        {
            Debug.LogError($"Login Failed : Already Has Login Data ({user.IsValid()},{user.UserId})");
            WriteData(MakeNewUserData("천마"), "users", "userData", user.UserId);
            return;
        }

        authentication.SignInAnonymouslyAsync().ContinueWithOnMainThread(OnLoginResult);
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

    public void WriteData(object wantData, params string[] directory)
    {

        if (rootDB is null || wantData is null) return;

        string jsonData = JsonUtility.ToJson(wantData);
        DatabaseReference currentReference = rootDB;
        foreach (string currentChild in directory)
        {
            currentReference = currentReference.Child(currentChild);
        }
        currentReference.SetRawJsonValueAsync(jsonData).ContinueWithOnMainThread(OnTaskResult);

        Dictionary<string, object> item = new()
        {
            {"name","돌"},{"weight", 0.3 },{ "price", 1}
        };
        rootDB.Child("Items").Child("Misc").Child("Nature").Child("Stone").UpdateChildrenAsync(item).ContinueWithOnMainThread(OnTaskResult);
        
    }

    void OnTaskResult(Task task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
    }
}
