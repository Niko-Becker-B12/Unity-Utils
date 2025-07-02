using System;

namespace RedsUtils
{
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

        public bool cursorLocked = true;
        public bool cursorInputForLook = true;
    

        public static event Action OnTryingToQuit;
        public static event Action OnQuitConfirm;


        public override void Awake()
        {
            base.Awake();

#if UNITY_ANDROID
        Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(90f);
#endif

        }

        private void Start()
        {



        }
        
        public void Quit()
        {

            Application.Quit();

        }
    
        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        public void SetCursorState(bool isLocked)
        {
        
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            cursorLocked = isLocked;

        }
        
        static bool WantsToQuit()
        {
            Debug.Log("Player prevented from quitting.");

            //ActivateQuitUI
            OnTryingToQuit?.Invoke();

            return false;
        }

        [RuntimeInitializeOnLoadMethod]
        static void RunOnStart()
        {
            Application.wantsToQuit += WantsToQuit;
        }

        public void TryingToQuit()
        {

            OnTryingToQuit?.Invoke();

        }

        public void QuitConfirmButton()
        {

            // When pressing Confirmation-Button, unsubscribe from Event and the Quit Application
            OnQuitConfirm?.Invoke();
            Application.wantsToQuit -= WantsToQuit;
            Application.Quit();

        }

    }
}