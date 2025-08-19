using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RedsUtils.LoadingSystem
{

    public class SceneLoadingManager : MonoBehaviour
    {
        
        private static List<ILoadable> loadables = new List<ILoadable>();

        public static async void SwitchScene(string sceneName)
        {

            await SceneManager.LoadSceneAsync("SCN_LoadingScreen");

            await LoadSceneAsync(sceneName);

        }
        
        private static bool AllLoadablesComplete()
        {
            
            foreach (var l in loadables)
                if (!l.IsDone) return false;
            return true;
            
        }
        
        private static void FinishLoading()
        {
            
            LoadingEvents.RaiseProgressChanged(1f);
            LoadingEvents.RaiseAllLoadablesComplete();
            
        }


        static async Task LoadSceneAsync(string sceneName)
        {

            //await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("SCN_LoadingScreen"));

            await Task.Delay(2000);

            LoadingEvents.RaiseSceneLoadStarted();

            // --- Step 1: Load Scene ---
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            op.allowSceneActivation = true;

            while (!op.isDone)
            {
                float sceneProgress = Mathf.Clamp01(op.progress / 0.9f);
                float blended = sceneProgress * 0.5f; // 0–50%
                LoadingEvents.RaiseProgressChanged(blended);

                await Task.Yield();
            }

            // --- Step 2: Collect loadables ---
            loadables = FindObjectsOfType<MonoBehaviour>(true)
                .OfType<ILoadable>()
                .ToList();

            foreach (var loadable in loadables)
                loadable.BeginLoad();

            // --- Step 3: Wait until loadables complete ---
            while (!AllLoadablesComplete())
            {
                float avgProgress = loadables.Count > 0
                    ? loadables.Average(l => l.Progress)
                    : 1f;

                float blended = 0.5f + avgProgress * 0.5f; // 50–100%
                LoadingEvents.RaiseProgressChanged(blended);

                await Task.Yield();
            }
            
            await Task.Delay(2000);

            // --- Step 4: Done ---
            FinishLoading();

        }

    }
}