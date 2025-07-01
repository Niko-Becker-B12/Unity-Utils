using Hextant;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using NUnit.Framework.Internal;
using System;
using UnityEngine.Android;
using UnityEngine.Localization.Settings;

#if UNITY_EDITOR
using Hextant.Editor;
using UnityEditor;
#endif

[Settings(SettingsUsage.RuntimeProject, "B12/Game Settings")]
public sealed class RuntimeSettings : Settings<RuntimeSettings>
{

#if UNITY_EDITOR
        [SettingsProvider]
        public static SettingsProvider GetSettingsProvider() => instance.GetSettingsProvider();
#endif


    [SerializeField]
    private bool usePlayerPrefs = false;

    public bool UsePlayerPrefs
    {
        get => usePlayerPrefs;
        set => usePlayerPrefs = value;
    }
    
    public List<RuntimeSettingsPropertyBase> properties = new List<RuntimeSettingsPropertyBase>();

    /*
    [OdinSerialize, PolymorphicDrawerSettings(ShowBaseType = false)]
    [ShowInInspector]
    [SerializeField]
    public Dictionary<string, RuntimeSettingsPropertyBase> properties = new Dictionary<string, RuntimeSettingsPropertyBase>();


    public string GetPropertyValue(string propertyName)
    {

        if (propertyName == null || string.IsNullOrWhiteSpace(propertyName))
            return string.Empty;

        RuntimeSettingsPropertyBase foundProperty = GetProperty(propertyName);

        if (foundProperty == null)
            return string.Empty;

        return foundProperty.value.ToString();

    }

    public RuntimeSettingsPropertyBase GetProperty(string propertyName)
    {

        if (propertyName == null || string.IsNullOrWhiteSpace(propertyName))
            return null;

        RuntimeSettingsPropertyBase foundProperty;
        
        if(properties.TryGetValue(propertyName, out foundProperty))
            return foundProperty;
        else
            return null;

    }

    public void SetPropertyValue(string propertyName, string propertyValue)
    {

        if (propertyName == null || string.IsNullOrWhiteSpace(propertyName))
            return;

        RuntimeSettingsPropertyBase foundProperty = GetProperty(propertyName);

        if (foundProperty == null)
            return;

        foundProperty.value = propertyValue;

        SaveSettingsToJson();

    }

    public void SetPropertyValue(string propertyName, RuntimeSettingsPropertyBase newProperty)
    {

        if (propertyName == null || string.IsNullOrWhiteSpace(propertyName))
            return;

        RuntimeSettingsPropertyBase foundProperty = GetProperty(propertyName);

        if (foundProperty == null)
            return;

        foundProperty.value = newProperty.value;

        SaveSettingsToJson();

    }

    private void OnEnable()
    {

#if UNITY_EDITOR
        SaveSettingsToJson();
#else
        if (File.Exists($"{Application.persistentDataPath}/B12-Settings.json"))
        {

            string jsonString = File.ReadAllText($"{Application.persistentDataPath}/B12-Settings.json");

            JsonUtility.FromJsonOverwrite(jsonString, RuntimeSettings.instance);

        }
        else
        {

            SaveSettingsToJson();

        }
#endif

    }

    */

    private void Awake()
    {
        
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
#endif
        
    }


    public string GetPropertyValue(string propertyName)
    {

        if (propertyName == null || string.IsNullOrWhiteSpace(propertyName))
            return string.Empty;

        RuntimeSettingsPropertyBase foundProperty = GetProperty(propertyName);

        if (foundProperty == null)
            return string.Empty;

        return foundProperty.Value.ToString();

    }

    public RuntimeSettingsPropertyBase GetProperty(string propertyName)
    {

        if (propertyName == null || string.IsNullOrWhiteSpace(propertyName))
            return null;

        RuntimeSettingsPropertyBase foundProperty = properties.Find((x) => x.name == propertyName);

        return foundProperty;

    }

    public void SetPropertyValue(string propertyName, string propertyValue)
    {

        if (propertyName == null || string.IsNullOrWhiteSpace(propertyName))
        {
            
            Debug.LogError($"There is no property named '{propertyName}' in settings");
            
            return;
            
        }

        RuntimeSettingsPropertyBase foundProperty = GetProperty(propertyName);

        if (foundProperty == null)
        {
            
            Debug.LogError($"Could not find property '{propertyName}' in settings");
            
            return;
            
        }

        foundProperty.Value = propertyValue;

        HandleSaving();

    }

    public void SetPropertyValue(string propertyName, RuntimeSettingsPropertyBase newProperty)
    {

        if (propertyName == null || string.IsNullOrWhiteSpace(propertyName))
            return;

        RuntimeSettingsPropertyBase foundProperty = GetProperty(propertyName);

        if (foundProperty == null)
            return;

        foundProperty.Value = newProperty.Value;

        HandleSaving();

    }

    private void OnEnable()
    {

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
#endif
        

#if UNITY_ANDROID

        if (PlayerPrefs.HasKey("Language"))
        {
            for (int i = 0; i < properties.Count; i++)
            {
            
                properties[i] = new RuntimeSettingsPropertyBase()
                {
                    name = properties[i].name,
                    Value = PlayerPrefs.GetString(properties[i].name)
                };
            
            }
        }
        else
        {
            HandleSaving();
        }

#else

        RuntimeSettingsPropertyBase languageProperty = GetProperty("Language");
        
        if (languageProperty != null)
        {
            
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find((x) => x.Identifier.Code == languageProperty.Value.ToString());
            
        }

        if (File.Exists($"{Application.persistentDataPath}/B12-Settings.json"))
        {

            string jsonString = File.ReadAllText($"{Application.persistentDataPath}/B12-Settings.json");

            JsonUtility.FromJsonOverwrite(jsonString, RuntimeSettings.instance);

        }
        else
        {

            HandleSaving();

        }
#endif
        


    }

    [Button("Save Settings")]
    public void HandleSaving()
    {
        
        Debug.Log("Saving Settings");

        if (usePlayerPrefs)
        {
            SaveSettingsToPlayerPrefs();
        }
        else
        {
            SaveSettingsToJson();
        }
        
    }
    
    public static void SaveSettingsToJson()
    {

        Debug.Log("Saving Settings to Json");

        string jsonString = JsonUtility.ToJson(RuntimeSettings.instance, true);

        File.WriteAllText($"{Application.persistentDataPath}/B12-Settings.json", jsonString);

    }
    
    public static void SaveSettingsToPlayerPrefs()
    {

        Debug.Log("Saving Settings to PlayerPrefs");
        
        PlayerPrefs.DeleteAll();

        for (int i = 0; i < RuntimeSettings.instance.properties.Count; i++)
        {
            
            PlayerPrefs.SetString(RuntimeSettings.instance.properties[i].name, RuntimeSettings.instance.properties[i].Value.ToString());
            
        }

        PlayerPrefs.Save();

    }

    [Button("Update Settings")]
    public void UpdateSettings()
    {

#if !UNITY_EDITOR
        for(int i = 0; i < properties.Count; i++)
        {

            properties[i].Value = properties[i].Value;
            
            if(properties[i].name == "Language")
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find((x)=>x.Identifier.Code == properties[i].Value.ToString());

        }
#endif

    }

}

[System.Serializable]
public class RuntimeSettingsPropertyBase
{

    public string name;

    [SerializeField]
    private string value;

    public string Value
    {

        get => value; 
        set 
        { 
            this.value = value; 
            OnPropertyChanged?.Invoke(value); 
        }
    

    }

    public event Action<string> OnPropertyChanged;

}

public interface ISettingsProperty<T>
{

    public T Value { get; set; }

}