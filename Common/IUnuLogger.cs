using UnityEngine;

public interface IUnuLogger
{
    bool EnableAllLog { get; set; }

    bool EnableLogInfo { get; set; }

    bool EnableLogWarning { get; set; }

    bool EnableLogError { get; set; }

    void Log(object message);

    void Log(object message, Object context);

    void LogFormat(string message, params object[] args);

    void LogFormat(Object context, string message, params object[] args);

    void LogError(object message);

    void LogError(object message, Object context);

    void LogErrorFormat(string message, params object[] args);

    void LogErrorFormat(Object context, string message, params object[] args);

    void LogWarning(object message);

    void LogWarning(object message, Object context);

    void LogWarningFormat(string message, params object[] args);

    void LogWarningFormat(Object context, string message, params object[] args);

    void LogException(System.Exception exception);

    void LogException(System.Exception exception, Object context);
}