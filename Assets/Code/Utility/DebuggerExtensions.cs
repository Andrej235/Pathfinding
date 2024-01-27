using System;
using UnityEngine;

namespace Assets.Code.Utility
{
    public static class DebuggerExtensions
    {
        public static void LogError(this Exception exception, bool includeStackTrace = true) => Debug.LogError(GetErrorMessage(exception, includeStackTrace));

        private static string GetErrorMessage(Exception ex, bool includeStackTrace = true)
        {
            return includeStackTrace
                ? ex.InnerException is null
                    ? $"---> Error occurred: {ex.Message}\n{ex.StackTrace}\n"
                    : $"---> Error occurred: {ex.Message}\n{ex.InnerException.Message}\n{ex.StackTrace}\n"
                : ex.InnerException is null
                    ? $"---> Error occurred: {ex.Message}"
                    : $"---> Error occurred: {ex.Message}\n{ex.InnerException.Message}";
        }
    }
}
