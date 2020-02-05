using System;
using System.Reflection;

namespace BddPipe
{
    /// <summary>
    /// Defines a way to rethrow an existing exception instance and preserve its original stack trace.
    /// </summary>
    internal static class ExceptionRethrowExtensions
    {
        private static readonly MethodInfo InternalPreserveStackTraceMethodInfo;

        static ExceptionRethrowExtensions()
        {
            const string internalPreserveStackTraceMethodName = "InternalPreserveStackTrace";

            InternalPreserveStackTraceMethodInfo =
                typeof(Exception)
                    .GetMethod(
                        internalPreserveStackTraceMethodName,
                        BindingFlags.Instance | BindingFlags.NonPublic
                    );
        }

        /// <summary>
        /// Attempt to preserve an exception's original stacktrace when it is thrown.
        /// </summary>
        /// <param name="exception">The exception instance.</param>
        public static void TryPreserveStackTrace(this Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            if (InternalPreserveStackTraceMethodInfo != null)
            {
                InternalPreserveStackTraceMethodInfo.Invoke(exception, null);
            }
        }
    }
}
