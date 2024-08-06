using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_VProfile
{
  /// <summary>
  /// String tag supporting various display kinds
  /// </summary>
  public enum ActiveState
  {
    /// <summary>
    /// The item is not shown
    /// </summary>
    Off=0,
    /// <summary>
    /// Item shown as Armed
    /// </summary>
    Armed,
    /// <summary>
    /// Item shown as Engaged
    /// </summary>
    Engaged,
    /// <summary>
    /// Item show as Warning
    /// </summary>
    Warn,
    /// <summary>
    /// Item shown as Alerting
    /// </summary>
    Alert,
  }


  /// <summary>
  /// Utility class supporting tagged strings
  ///  get/set ActiveStates
  /// </summary>
  public class TaggedString
  {
    /// <summary>
    /// Alert Tag
    /// </summary>
    public const string CAlert = "!";
    /// <summary>
    /// Warn Tag
    /// </summary>
    public const string CWarn = "^";
    /// <summary>
    /// Arm Tag
    /// </summary>
    public const string CArm = "~";

    // tagger
    private string[] m_stateChars = new string[]{ "", CArm, "", CWarn, CAlert};

    /// <summary>
    /// The ActiveState
    /// </summary>
    public ActiveState State { get; set; } = ActiveState.Off;

    /// <summary>
    /// The Active string
    /// prepended with one of the Tag chars
    /// </summary>
    public string ActiveString {
      get => m_stateChars[(int)State] + m_string; // decorate
      set {
        if ( string.IsNullOrEmpty( value ) ) {
          State = ActiveState.Off;
          m_string = "";
        }
        else {
          // undecorate
          string tag = value.Substring(0, 1); // 1st char
          m_string = "";
          if ( value.Length>1) {
            m_string = value.Substring( 1 );  // rest
          }
          // set state from tag
          if ( tag == CAlert ) State = ActiveState.Alert;
          else if ( tag == CWarn ) State = ActiveState.Warn;
          else if ( tag == CArm ) State = ActiveState.Armed;
          else {
            State = ActiveState.Engaged;
            m_string = value; // no tag 
          }
        }
      }
    }

    /// <summary>
    /// The undecorated display Text
    /// </summary>
    public string Text {
      get => m_string;
      set {
        m_string = value;
      }
    }
    private string m_string;


  }
}
