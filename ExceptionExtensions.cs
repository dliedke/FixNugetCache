using System;
using System.Text;

namespace FixNugetCache
{
    public static class ExceptionExtensions
    {
        public static string GetErrorMsg(this Exception ex)
        {
            StringBuilder sb = new(ex.Message);
            Exception inner = ex.InnerException;
            while (inner != null)
            {
                sb.AppendFormat(" - {0}", inner.Message);
                inner = inner.InnerException;
            }
            return sb.ToString();
        }
    }
}