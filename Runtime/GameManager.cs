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

        public static event Action<string> OnPlayerNewCustomID;

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

    }
}