using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// Alternative to YVision's reflection. Pseudo singleton manager, except doesn't require other components to change.
/// </summary>
public class AppServicesProvider : MonoBehaviour
{

    private Dictionary<Type, Component> Services = new Dictionary<Type, Component>();
    private Dictionary<string, System.Object> GlobalObjects = new Dictionary<string, System.Object>();
    private static AppServicesProvider prov;
    public Component[] PreloadedServices;
    public bool DebugMode;
    void Awake()
    {
        if (prov != null)
        {
            Destroy(this);
            return;
        }
        prov = this;
        foreach (var serv in PreloadedServices)
        {
            Services.Add(serv.GetType(), serv);
            //WorldObjectRoot.Instance.AddService(serv.GetType(), serv);
        }
        var currentScene = SceneManager.GetActiveScene();
        var root = currentScene.GetRootGameObjects();
        var providers = root.SelectMany(v => v.GetInterfacesInChildren<IAppService>());
        foreach (var p in providers)
        {
            p.Serv();
        }
        var clients = root.SelectMany(v => v.GetInterfacesInChildren<IAppServiceClient>());
        foreach (var c in clients)
            c.FetchServices();
    }
    //[Show]
    //public void gettest(string key)
    //{
    //    Debug.LogError(GetObject<GameObject>(key).name);
    //}
    public static T GetService<T>() where T : Component
    {
        Component serv;
        var t = typeof(T);
        if (prov.Services.TryGetValue(t, out serv))
        {
            if (prov.DebugMode) Debug.LogError($"Type {t} is dictionary, returning it from {serv.name}");
            return serv as T;
        }
        serv = (Component) FindObjectOfType(t);
        RegisterService<T>(serv as T);
        return serv as T;
    }

    public static T GetObject<T>(string key)
    {
        object value = null;
        if (prov.GlobalObjects.TryGetValue(key, out value))
        {
            return (T)value;
        }
        Debug.LogError("Requested object could not be found");
        return default(T);
    }
    public static void RegisterGlobalObject(string key, object obj, UnityEngine.Component provider)
    {
        RegisterGlobalObject(key, obj);
        provider.OnDestroyAsObservable().Subscribe(_ => prov.GlobalObjects.Remove(key));
    }

    public static void RegisterGlobalObject(string key, object obj)
    {
        prov.GlobalObjects.Add(key, obj);
    }

    public static void RegisterService<T>(T obj) where T : Component
    {
        if (prov.DebugMode) Debug.LogError("Registering " + obj + " as " + typeof(T));
        prov.Services.Add(typeof(T), obj as T);
    }
    //[Show]
    //public void Test()
    //{
    //    //WorldObjectRoot.Instance.serv
    //    //Debug.LogError($"elements in dictionary = {Services.Count} at start");
    //    //var b = Get<MapViewer>();
    //    //Debug.LogError(b +" <<Got mapviewer");
    //    //var c = Get<Tour>();
    //    //Debug.LogError(c + " <<Got Tour");
    //    //var d = Get<Tour>();
    //    //Debug.LogError(d + " <<Got Tour again");
    //    //Debug.LogError($"elements in dictionary = {Services.Count} at end" );
    //}

}
public interface IAppServiceClient
{
    /// <summary>
    /// Use this to fetch your required Services
    /// </summary>
    void FetchServices();
}
public interface IAppService
{
    /// <summary>
    /// Returns service registered
    /// </summary>
    /// <returns>The service the provider wishes registered</returns>
    Component Serv();
}