using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Security.Authentication;
using System.Threading.Tasks;
using UnityEngine;

public class DBManager : ManagerBase
{
     FirebaseAuth authentication;
     FirebaseUser user;
     DatabaseReference DBReference;

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(InitializeFireBase);
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

            DBReference = FirebaseDatabase.DefaultInstance.RootReference;

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
            Debug.LogError("Login Failed : Already Has Login Data");
        }

        authentication.SignInAnonymouslyAsync().ContinueWith(OnLoginResult);
    }

    private void OnLoginResult(Task<AuthResult> task)
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            Debug.LogError($"Fail to Sign in : {task.Exception}");
            return;
        }

        user = task.Result.User;
        Debug.Log($"Sign in Succeed: {user.UserId}");
    }
}
