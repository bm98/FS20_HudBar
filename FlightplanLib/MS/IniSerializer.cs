using MS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MS
{
  /// <summary>
  /// (De)Serializer for INI files
  /// </summary>
  internal class IniSerializer
  {

    // Deserialize a section
    private object DeSection( Type st, string section, MSiniFile iniFile, int level )
    {
      var svSection = (section == IniFileSection.MainSection) ? "" : section; // Main in the INI is an empty string
      var ret = Activator.CreateInstance( st );

      // Base Type scan
      PropertyInfo[] propertyInfo;
      Type myType = st;
      // Get the type and fields of FieldInfoClass.
      propertyInfo = myType.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly );

      // loop all fields 
      foreach (var prop in propertyInfo) {
        var cArgs = prop.GetCustomAttributes( false );
        string svKey = null, sectionAttr = null;
        // check if settable
        if (prop.GetSetMethod( true ) == null) { continue; } // cannot 
        // find the Ini attribute of this Property
        for (int a = 0; a < cArgs.Length; a++) {
          if (cArgs[a] is IniFileKey) {
            svKey = (cArgs[a] as IniFileKey).Name;
          }
          else if (cArgs[a] is IniFileSection) {
            sectionAttr = (cArgs[a] as IniFileSection).Name;
          }
          else if (cArgs[a] is IniFileIgnore) { continue; } // jump to next
          else {
            // other attribute - we don't care
          }
        }

        // check for Ini attribute available
        if (string.IsNullOrEmpty( svKey ) && string.IsNullOrEmpty( sectionAttr )) {
          // for now we ignore them
          //throw new ApplicationException( $"RegisterIniVars - missing Attribute for property: {prop.Name}" );
        }

        else {
          // now we should be able to load items..
          if (prop.PropertyType == typeof( string )) {
            var strValue = iniFile.ItemValue( svSection, svKey );
            prop.SetValue( ret, strValue );
          }
          else if (prop.PropertyType == typeof( float )) {
            var strValue = iniFile.ItemValue( svSection, svKey );
            if (float.TryParse( strValue, out float dValue )) { prop.SetValue( ret, dValue ); }
          }
          else if (prop.PropertyType == typeof( double )) {
            var strValue = iniFile.ItemValue( svSection, svKey );
            if (double.TryParse( strValue, out double dValue )) { prop.SetValue( ret, dValue ); }
          }
          else if (prop.PropertyType == typeof( int )) {
            var strValue = iniFile.ItemValue( svSection, svKey );
            if (int.TryParse( strValue, out int dValue )) { prop.SetValue( ret, dValue ); }
          }
          else if (prop.PropertyType == typeof( uint )) {
            var strValue = iniFile.ItemValue( svSection, svKey );
            if (uint.TryParse( strValue, out uint dValue )) { prop.SetValue( ret, dValue ); }
          }
          else if (prop.PropertyType == typeof( long )) {
            var strValue = iniFile.ItemValue( svSection, svKey );
            if (long.TryParse( strValue, out long dValue )) { prop.SetValue( ret, dValue ); }
          }
          else if (prop.PropertyType == typeof( ulong )) {
            var strValue = iniFile.ItemValue( svSection, svKey );
            if (ulong.TryParse( strValue, out ulong dValue )) { prop.SetValue( ret, dValue ); }
          }
          else if (prop.PropertyType == typeof( short )) {
            var strValue = iniFile.ItemValue( svSection, svKey );
            if (short.TryParse( strValue, out short dValue )) { prop.SetValue( ret, dValue ); }
          }
          else if (prop.PropertyType == typeof( ushort )) {
            var strValue = iniFile.ItemValue( svSection, svKey );
            if (ushort.TryParse( strValue, out ushort dValue )) { prop.SetValue( ret, dValue ); }
          }

          else if (prop.PropertyType == typeof( Dictionary<string, string> )) {
            // a sequence of svNameN items
            var dict = new Dictionary<string, string>( );
            int index = 0;
            var strValue = iniFile.ItemValue( svSection, $"{svKey}{index}" );
            while (!string.IsNullOrEmpty( strValue )) {
              dict.Add( $"{svKey}{index}", strValue );
              // next
              strValue = iniFile.ItemValue( svSection, $"{svKey}{++index}" );
            }
            prop.SetValue( ret, dict );
          }
          else {
            // custom type, represents a section can only be on level 0
            if ((level == 0) && !string.IsNullOrEmpty( sectionAttr )) {
              object item = DeSection( prop.PropertyType, sectionAttr, iniFile, level + 1 );
              prop.SetValue( ret, item );
            }
            else if (!string.IsNullOrEmpty( svKey )) {
              // decode the string content itself into an object
            }
            else {
              throw new ApplicationException( $"ERROR: Nested section of Type: {prop.PropertyType} of Property {prop.Name}" );
            }
          }
        }
      }
      return ret;
    }

    private Type _type;

    public IniSerializer( Type type )
    {
      _type = type;
    }

    /// <summary>
    /// Deserializes the INI document contained by the specified System.IO.Stream.
    /// </summary>
    /// <param name="stream">The System.IO.Stream that contains the INI document to deserialize.</param>
    /// <returns>The System.Object being deserialized.</returns>
    public object Deserialize( Stream stream )
    {
      var ret = Activator.CreateInstance( _type );

      MSiniFile iniFile = new MSiniFile( );
      iniFile.Load( stream );

      ret = DeSection( _type, IniFileSection.MainSection, iniFile, 0 );

      return ret;
    }
  }
}
