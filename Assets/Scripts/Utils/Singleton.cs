using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    /// <summary>
    /// Gets or sets an instance of the object.
    /// </summary>
    public static T Instance
    {
        get
        {
            return instance;
        }

        set
        {
            if (instance != null && value != null)
            {
                Debug.LogError(string.Format("There is another instance of {0} object in the scene.", typeof(T)));
            }

            instance = value;
        }
    }

    /// <summary>
    /// Handles a Unity Awake event.
    /// </summary>
    protected virtual void Awake()
    {
        Instance = (T)this;
    }

    /// <summary>
    /// Handles a Unity OnDestroy event.
    /// </summary>
    protected virtual void OnDestroy()
    {
        Instance = null;
    }
}