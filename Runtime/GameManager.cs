using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : SingletonPersistent<GameManager>
{
    
    public static event Action<string> OnPlayerNewCustomID;

    public int currentAppState;

    public List<AppState> appStates = new List<AppState>();

    [SerializeField] private string playerId;
    
    public string PlayerId
    {
        get { return playerId; }
        set
        {

            playerId = value;

            OnPlayerNewCustomID?.Invoke(playerId);

        }
    }

    public static event Action OnSceneChangeStarted;
    public static event Action OnSceneChangeFinalized;


    public override void Awake()
    {
        base.Awake();

#if UNITY_ANDROID
        Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(90f);
#endif

    }

    private void Start()
    {

        SwitchAppState(0);

    }

    public void SwitchAppState(int index)
    {

        currentAppState = index;

        for (int i = 0; i < appStates.Count; i++)
        {

            if(i == index)
                appStates[index].SwitchToState();
            else
                appStates[i].OnReset();

        }

    }

    [Button]
    public void SwitchScene(string scene)
    {
        
        OnSceneChangeStarted?.Invoke();
        
        HandleSceneChange(scene);

    }

    async void HandleSceneChange(string scene)
    {
        
        string currentScene = SceneManager.GetActiveScene().name;
        
        Debug.Log($"Started loading scene {scene}");
        
        await Task.Delay(500);
        
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);

        Debug.Log($"started async loading scene {scene}");
        
        await asyncOperation;
        
        await Task.Delay(2000);
        
        
        Debug.Log($"finished loading scene {scene}");
        
        await Task.Delay(1000);
        
        
        OnSceneChangeFinalized?.Invoke();

    }

}

[System.Serializable]
public class AppState
{


    [FoldoutGroup("$stateName")]
    public string stateName;

    [FoldoutGroup("$stateName")]
    [TextArea(5, 10)]
    public string stateDescription;

    [FoldoutGroup("$stateName")]
    public List<Function> onSetActiveFunctions = new List<Function>();

    [FoldoutGroup("$stateName")]
    public List<Function> onResetFunctions = new List<Function>();


    public void SwitchToState()
    {

        Debug.Log("Switching State");

        for(int i = 0; i < onSetActiveFunctions.Count; i++)
            Function.InvokeEvent(onSetActiveFunctions[i], GameManager.Instance);

    }

    public void OnReset()
    {

        Debug.Log($"Resetting State {stateName}");

        for (int i = 0; i < onResetFunctions.Count; i++)
            Function.InvokeEvent(onResetFunctions[i], GameManager.Instance);

    }


    

}