using UnityEngine;

public static class UnuLogger
{
    public static IUnuLogger Logger
    {
        get { return _logger ?? _default; }
    }

    public static bool EnableAllLog
    {
        get { return Logger.EnableAllLog; }
        set { Logger.EnableAllLog = value; }
    }

    public static bool EnableLogInfo
    {
        get { return Logger.EnableLogInfo; }
        set { Logger.EnableLogInfo = value; }
    }

    public static bool EnableLogWarning
    {
        get { return Logger.EnableLogWarning; }
        set { Logger.EnableLogWarning = value; }
    }

    public static bool EnableLogError
    {
        get { return Logger.EnableLogError; }
        set { Logger.EnableLogError = value; }
    }

    private static readonly DefaultLogger _default = new DefaultLogger();
    private static IUnuLogger _logger;

    public static void Register(IUnuLogger logger)
    {
        _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    public static void UseDefault()
    {
        _logger = null;
    }

    public static void Log(object message)
    {
        Logger.Log(message);
    }

    public static void Log(object message, Object context)
    {
        Logger.Log(message, context);
    }

    public static void LogFormat(string message, params object[] args)
    {
        Logger.LogFormat(message, args);
    }

    public static void LogFormat(Object context, string message, params object[] args)
    {
        Logger.LogFormat(context, message, args);
    }

    public static void LogError(object message)
    {
        Logger.LogError(message);
    }

    public static void LogError(object message, Object context)
    {
        Logger.LogError(message, context);
    }

    public static void LogErrorFormat(string message, params object[] args)
    {
        Logger.LogErrorFormat(message, args);
    }

    public static void LogErrorFormat(Object context, string message, params object[] args)
    {
        Logger.LogErrorFormat(context, message, args);
    }

    public static void LogWarning(object message)
    {
        Logger.LogWarning(message);
    }

    public static void LogWarning(object message, Object context)
    {
        Logger.LogWarning(message, context);
    }

    public static void LogWarningFormat(string message, params object[] args)
    {
        Logger.LogWarningFormat(message, args);
    }

    public static void LogWarningFormat(Object context, string message, params object[] args)
    {
        Logger.LogWarningFormat(context, message, args);
    }

    public static void LogException(System.Exception exception)
    {
        Logger.LogException(exception);
    }

    public static void LogException(System.Exception exception, Object context)
    {
        Logger.LogException(exception, context);
    }

    private sealed class DefaultLogger : IUnuLogger
    {
        public bool EnableAllLog { get; set; } = true;

        public bool EnableLogInfo { get; set; } = true;

        public bool EnableLogWarning { get; set; } = true;

        public bool EnableLogError { get; set; } = true;

        public void Log(object message)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogInfo)
                Debug.Log(message);
#endif
        }

        public void Log(object message, Object context)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogInfo)
                Debug.Log(message, context);
#endif
        }

        public void LogFormat(string message, params object[] args)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogInfo)
                Debug.LogFormat(message, args);
#endif
        }

        public void LogFormat(Object context, string message, params object[] args)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogInfo)
                Debug.LogFormat(context, message, args);
#endif
        }

        public void LogError(object message)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogError)
                Debug.LogError(message);
#endif
        }

        public void LogError(object message, Object context)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogError)
                Debug.LogError(message, context);
#endif
        }

        public void LogErrorFormat(string message, params object[] args)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogError)
                Debug.LogErrorFormat(message, args);
#endif
        }

        public void LogErrorFormat(Object context, string message, params object[] args)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogError)
                Debug.LogErrorFormat(context, message, args);
#endif
        }

        public void LogWarning(object message)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogWarning)
                Debug.LogWarning(message);
#endif
        }

        public void LogWarning(object message, Object context)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogWarning)
                Debug.LogWarning(message, context);
#endif
        }

        public void LogWarningFormat(string message, params object[] args)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogWarning)
                Debug.LogWarningFormat(message, args);
#endif
        }

        public void LogWarningFormat(Object context, string message, params object[] args)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogWarning)
                Debug.LogWarningFormat(context, message, args);
#endif
        }

        public void LogException(System.Exception exception)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogError)
                Debug.LogException(exception);
#endif
        }

        public void LogException(System.Exception exception, Object context)
        {
#if UNITY_DEBUG || UNITY_EDITOR
            if (this.EnableAllLog || this.EnableLogError)
                Debug.LogException(exception, context);
#endif
        }
    }
}