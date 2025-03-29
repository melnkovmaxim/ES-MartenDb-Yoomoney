using System.Reflection;

namespace ES.Yoomoney.Application;

internal static class CurrentAssembly
{
    public static readonly Assembly Reference = Assembly.GetExecutingAssembly();
}