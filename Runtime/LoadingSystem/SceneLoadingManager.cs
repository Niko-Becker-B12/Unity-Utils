using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{

    public static async void SwitchScene(string sceneName)
    {
        
        await SceneManager.LoadSceneAsync("SCN_LoadingScreen");

        await LoadSceneAsync(sceneName);
        
        //await SceneManager.UnloadSceneAsync("SCN_LoadingScreen");

    }

    static async Task LoadSceneAsync(string sceneName)
    {
        
        //await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("SCN_LoadingScreen"));
        
        await Task.Delay(2000);
        
        await SceneManager.LoadSceneAsync(sceneName);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        
        await Task.Delay(2000);
        
    }
    
    
}