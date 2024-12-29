using System.Reflection;

namespace ES.Yoomoney.Api;

internal static class CurrentAssembly
{
    public static readonly Assembly Reference = Assembly.GetExecutingAssembly();
}