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

        protected RuntimeSettingsPropertyBase UseQuitSystemProperty;
    

        public static event Action OnTryingToQuit;
        public static event Action OnQuitConfirm;


        public override void Awake()
        {
            base.Awake();

#if UNITY_ANDROID
        Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(90);
#endif

        }

        private void Start()
        {

            RuntimeSettings.GetSettingsProvider();

            UseQuitSystemProperty = RuntimeSettings.instance.GetProperty("UseQuitSystem");

            if (UseQuitSystemProperty == null)
            {

                UseQuitSystemProperty = new RuntimeSettingsPropertyBase()
                {
                    name = "UseQuitSystem",
                    Value = Boolean.TrueString,
                };
                
                RuntimeSettings.instance.AddProperty(UseQuitSystemProperty);
                
            }
            else
            {
                
                UseQuitSystemProperty.Value = RuntimeSettings.instance.GetPropertyValue("UseQuitSystem");
                
            }
            
        }
        
        static bool WantsToQuit()
        {

            if (GameManager.Instance.UseQuitSystemProperty.Value == Boolean.TrueString)
            {
                
                //ActivateQuitUI
                OnTryingToQuit?.Invoke();

                return false;
                
            }
            else
            {

                GameManager.Instance.QuitConfirm();
                
                return true;
                
            }


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

        public void QuitConfirm()
        {

            // When pressing Confirmation-Button, unsubscribe from Event and the Quit Application
            OnQuitConfirm?.Invoke();
            Application.wantsToQuit -= WantsToQuit;
            Application.Quit();

        }

    }
}