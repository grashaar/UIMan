using UnityEngine;

public class UnuLogger
{
    private static bool _enableAllLog = true;

    public static bool EnableAllLog
    {
        get { return _enableAllLog; }
        set
        {
            _enableAllLog = value;
        }
    }

    private static bool _enableLogInfo = true;

    public static bool EnableLogInfo
    {
        get { return _enableLogInfo; }
        set
        {
            _enableLogInfo = value;
        }
    }

    private static bool _enableLogWarning = true;

    public static bool EnableLogWarning
    {
        get { return _enableLogWarning; }
        set
        {
            _enableLogWarning = value;
        }
    }

    private static bool _enableLogError = true;

    public static bool EnableLogError
    {
        get { return _enableLogError; }
        set
        {
            _enableLogError = value;
        }
    }

    public static void Log(object message)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogInfo)
            Debug.Log(message);
#endif
    }

    public static void Log(object message, Object context)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogInfo)
            Debug.Log(message, context);
#endif
    }

    public static void LogFormat(string message, params object[] args)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogInfo)
            Debug.LogFormat(message, args);
#endif
    }

    public static void LogFormat(Object context, string message, params object[] args)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogInfo)
            Debug.LogFormat(context, message, args);
#endif
    }

    public static void LogError(object message)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogError)
            Debug.LogError(message);
#endif
    }

    public static void LogErrorFormat(string message, params object[] args)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogError)
            Debug.LogErrorFormat(message, args);
#endif
    }

    public static void LogErrorFormat(Object context, string message, params object[] args)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogError)
            Debug.LogErrorFormat(context, message, args);
#endif
    }

    public static void LogWarning(object message)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogWarning)
            Debug.LogWarning(message);
#endif
    }

    public static void LogFormatWarning(string message, params object[] args)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogWarning)
            Debug.LogWarningFormat(message, args);
#endif
    }

    public static void LogFormatWarning(Object context, string message, params object[] args)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogWarning)
            Debug.LogWarningFormat(context, message, args);
#endif
    }

    public static void LogException(System.Exception exception)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogError)
            Debug.LogException(exception);
#endif
    }

    public static void LogException(System.Exception exception, Object context)
    {
#if UNITY_DEBUG || UNITY_EDITOR
        if (EnableAllLog || EnableLogError)
            Debug.LogException(exception, context);
#endif
    }
}