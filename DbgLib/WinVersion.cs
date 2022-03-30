using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace DbgLib
{

  internal static class WinVersion
  {
    static string get( Func<string> func )
    {
      try { return func( ); }
      catch { return "(undefined)"; }
    }

    public static string WindowsVersion( )
    {
      string NL = Environment.NewLine;
      string HKLMWinNTCurrent = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";

      string osName = get(() => Registry.GetValue(HKLMWinNTCurrent, "productName", "").ToString());

      string osRelease = get(() => Registry.GetValue(HKLMWinNTCurrent, "ReleaseId", "").ToString());
      if ( !string.IsNullOrEmpty( osRelease ) ) osRelease = $" ({ osRelease})";

      string osVersion = Environment.OSVersion.Version.ToString();

      string osType = Environment.Is64BitOperatingSystem ? "64-bits" : "32-bits";

      string clr = Environment.Version.ToString();

      string dotnet = get(() =>
          {
            var attributes = Assembly.GetExecutingAssembly().CustomAttributes;
            var result = attributes.FirstOrDefault(a => a.AttributeType == typeof(TargetFrameworkAttribute));
            return result == null
                    ? ".NET Framework (unknown)"
                    : result.NamedArguments[0].TypedValue.Value.ToString();
          });

      string Platform = $"{osName} {osType} {osVersion}{osRelease}{NL}{dotnet}{NL}CLR {clr}";

      return $"{osName} {osRelease} {osVersion} {osType}\n    Runtime: {clr}\n    .Net: {dotnet}";
    }
  }
}