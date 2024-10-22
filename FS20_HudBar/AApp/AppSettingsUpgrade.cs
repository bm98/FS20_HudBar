using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar
{
  /// <summary>
  /// Methods to upgrade from Old to New Settings
  ///  Must be called at the very start of the HudBar App
  /// </summary>
  internal static class AppSettingsUpgrade
  {

    /// <summary>
    /// The Main Method to Upgrade to V2 AppSettings
    /// </summary>
    public static void UpgradeSettings( )
    {
      if (AppSettingsV2.Instance.V2InUse) return; // already Upgraded
      if (!Program.AppSettingsV1Available) return; // V1 is not available, bail out

      // should/MUST never fail
      try {
        // start upgrading from V1 (we do this in an Alphabetic order - easier to check if complete)
        AppSettingsV2.Instance.Altitude_Metric = AppSettings.Instance.Altitude_Metric;
        AppSettingsV2.Instance.Appearance = AppSettings.Instance.Appearance;
        AppSettingsV2.Instance.Distance_Metric = AppSettings.Instance.Distance_Metric;
        AppSettingsV2.Instance.FlowBreak_1 = AppSettings.Instance.FlowBreak_1;
        AppSettingsV2.Instance.FlowBreak_10 = AppSettings.Instance.FlowBreak_10;
        AppSettingsV2.Instance.FlowBreak_2 = AppSettings.Instance.FlowBreak_2;
        AppSettingsV2.Instance.FlowBreak_3 = AppSettings.Instance.FlowBreak_3;
        AppSettingsV2.Instance.FlowBreak_4 = AppSettings.Instance.FlowBreak_4;
        AppSettingsV2.Instance.FlowBreak_5 = AppSettings.Instance.FlowBreak_5;
        AppSettingsV2.Instance.FlowBreak_6 = AppSettings.Instance.FlowBreak_6;
        AppSettingsV2.Instance.FlowBreak_7 = AppSettings.Instance.FlowBreak_7;
        AppSettingsV2.Instance.FlowBreak_8 = AppSettings.Instance.FlowBreak_8;
        AppSettingsV2.Instance.FlowBreak_9 = AppSettings.Instance.FlowBreak_9;
        AppSettingsV2.Instance.FltAutoSaveATC = AppSettings.Instance.FltAutoSaveATC; // restricted to 0,1 in AppSettingsV2
        AppSettingsV2.Instance.FormLocation = AppSettings.Instance.FormLocation;
        AppSettingsV2.Instance.FRecorder = AppSettings.Instance.FRecorder;
        AppSettingsV2.Instance.FreeText = AppSettings.Instance.FreeText;
        AppSettingsV2.Instance.HKCamera = AppSettings.Instance.HKCamera;
        AppSettingsV2.Instance.HKChecklistBox = AppSettings.Instance.HKChecklistBox;
        AppSettingsV2.Instance.HKProfile1 = AppSettings.Instance.HKProfile1;
        AppSettingsV2.Instance.HKProfile10 = AppSettings.Instance.HKProfile10;
        AppSettingsV2.Instance.HKProfile2 = AppSettings.Instance.HKProfile2;
        AppSettingsV2.Instance.HKProfile3 = AppSettings.Instance.HKProfile3;
        AppSettingsV2.Instance.HKProfile4 = AppSettings.Instance.HKProfile4;
        AppSettingsV2.Instance.HKProfile5 = AppSettings.Instance.HKProfile5;
        AppSettingsV2.Instance.HKProfile6 = AppSettings.Instance.HKProfile6;
        AppSettingsV2.Instance.HKProfile7 = AppSettings.Instance.HKProfile7;
        AppSettingsV2.Instance.HKProfile8 = AppSettings.Instance.HKProfile8;
        AppSettingsV2.Instance.HKProfile9 = AppSettings.Instance.HKProfile9;
        AppSettingsV2.Instance.HKShelf = AppSettings.Instance.HKShelf;
        AppSettingsV2.Instance.HKShowHide = AppSettings.Instance.HKShowHide;
        AppSettingsV2.Instance.InGameHook = AppSettings.Instance.InGameHook;
        AppSettingsV2.Instance.KeyboardHook = AppSettings.Instance.KeyboardHook;
        AppSettingsV2.Instance.Profile_1 = AppSettings.Instance.Profile_1;
        AppSettingsV2.Instance.Profile_10 = AppSettings.Instance.Profile_10;
        AppSettingsV2.Instance.Profile_10_Condensed = AppSettings.Instance.Profile_10_Condensed;
        AppSettingsV2.Instance.Profile_10_FontSize = AppSettings.Instance.Profile_10_FontSize;
        AppSettingsV2.Instance.Profile_10_Kind = AppSettings.Instance.Profile_10_Kind;
        AppSettingsV2.Instance.Profile_10_Location = AppSettings.Instance.Profile_10_Location;
        AppSettingsV2.Instance.Profile_10_Name = AppSettings.Instance.Profile_10_Name;
        AppSettingsV2.Instance.Profile_10_Placement = AppSettings.Instance.Profile_10_Placement;
        AppSettingsV2.Instance.Profile_10_Trans = AppSettings.Instance.Profile_10_Trans;
        AppSettingsV2.Instance.Profile_1_Condensed = AppSettings.Instance.Profile_1_Condensed;
        AppSettingsV2.Instance.Profile_1_FontSize = AppSettings.Instance.Profile_1_FontSize;
        AppSettingsV2.Instance.Profile_1_Kind = AppSettings.Instance.Profile_1_Kind;
        AppSettingsV2.Instance.Profile_1_Location = AppSettings.Instance.Profile_1_Location;
        AppSettingsV2.Instance.Profile_1_Name = AppSettings.Instance.Profile_1_Name;
        AppSettingsV2.Instance.Profile_1_Placement = AppSettings.Instance.Profile_1_Placement;
        AppSettingsV2.Instance.Profile_1_Trans = AppSettings.Instance.Profile_1_Trans;
        AppSettingsV2.Instance.Profile_2 = AppSettings.Instance.Profile_2;
        AppSettingsV2.Instance.Profile_2_Condensed = AppSettings.Instance.Profile_2_Condensed;
        AppSettingsV2.Instance.Profile_2_FontSize = AppSettings.Instance.Profile_2_FontSize;
        AppSettingsV2.Instance.Profile_2_Kind = AppSettings.Instance.Profile_2_Kind;
        AppSettingsV2.Instance.Profile_2_Location = AppSettings.Instance.Profile_2_Location;
        AppSettingsV2.Instance.Profile_2_Name = AppSettings.Instance.Profile_2_Name;
        AppSettingsV2.Instance.Profile_2_Placement = AppSettings.Instance.Profile_2_Placement;
        AppSettingsV2.Instance.Profile_2_Trans = AppSettings.Instance.Profile_2_Trans;

        AppSettingsV2.Instance.Profile_3 = AppSettings.Instance.Profile_3;
        AppSettingsV2.Instance.Profile_3_Condensed = AppSettings.Instance.Profile_3_Condensed;
        AppSettingsV2.Instance.Profile_3_FontSize = AppSettings.Instance.Profile_3_FontSize;
        AppSettingsV2.Instance.Profile_3_Kind = AppSettings.Instance.Profile_3_Kind;
        AppSettingsV2.Instance.Profile_3_Location = AppSettings.Instance.Profile_3_Location;
        AppSettingsV2.Instance.Profile_3_Name = AppSettings.Instance.Profile_3_Name;
        AppSettingsV2.Instance.Profile_3_Placement = AppSettings.Instance.Profile_3_Placement;
        AppSettingsV2.Instance.Profile_3_Trans = AppSettings.Instance.Profile_3_Trans;

        AppSettingsV2.Instance.Profile_4 = AppSettings.Instance.Profile_4;
        AppSettingsV2.Instance.Profile_4_Condensed = AppSettings.Instance.Profile_4_Condensed;
        AppSettingsV2.Instance.Profile_4_FontSize = AppSettings.Instance.Profile_4_FontSize;
        AppSettingsV2.Instance.Profile_4_Kind = AppSettings.Instance.Profile_4_Kind;
        AppSettingsV2.Instance.Profile_4_Location = AppSettings.Instance.Profile_4_Location;
        AppSettingsV2.Instance.Profile_4_Name = AppSettings.Instance.Profile_4_Name;
        AppSettingsV2.Instance.Profile_4_Placement = AppSettings.Instance.Profile_4_Placement;
        AppSettingsV2.Instance.Profile_4_Trans = AppSettings.Instance.Profile_4_Trans;

        AppSettingsV2.Instance.Profile_5 = AppSettings.Instance.Profile_5;
        AppSettingsV2.Instance.Profile_5_Condensed = AppSettings.Instance.Profile_5_Condensed;
        AppSettingsV2.Instance.Profile_5_FontSize = AppSettings.Instance.Profile_5_FontSize;
        AppSettingsV2.Instance.Profile_5_Kind = AppSettings.Instance.Profile_5_Kind;
        AppSettingsV2.Instance.Profile_5_Location = AppSettings.Instance.Profile_5_Location;
        AppSettingsV2.Instance.Profile_5_Name = AppSettings.Instance.Profile_5_Name;
        AppSettingsV2.Instance.Profile_5_Placement = AppSettings.Instance.Profile_5_Placement;
        AppSettingsV2.Instance.Profile_5_Trans = AppSettings.Instance.Profile_5_Trans;

        AppSettingsV2.Instance.Profile_6 = AppSettings.Instance.Profile_6;
        AppSettingsV2.Instance.Profile_6_Condensed = AppSettings.Instance.Profile_6_Condensed;
        AppSettingsV2.Instance.Profile_6_FontSize = AppSettings.Instance.Profile_6_FontSize;
        AppSettingsV2.Instance.Profile_6_Kind = AppSettings.Instance.Profile_6_Kind;
        AppSettingsV2.Instance.Profile_6_Location = AppSettings.Instance.Profile_6_Location;
        AppSettingsV2.Instance.Profile_6_Name = AppSettings.Instance.Profile_6_Name;
        AppSettingsV2.Instance.Profile_6_Placement = AppSettings.Instance.Profile_6_Placement;
        AppSettingsV2.Instance.Profile_6_Trans = AppSettings.Instance.Profile_6_Trans;

        AppSettingsV2.Instance.Profile_7 = AppSettings.Instance.Profile_7;
        AppSettingsV2.Instance.Profile_7_Condensed = AppSettings.Instance.Profile_7_Condensed;
        AppSettingsV2.Instance.Profile_7_FontSize = AppSettings.Instance.Profile_7_FontSize;
        AppSettingsV2.Instance.Profile_7_Kind = AppSettings.Instance.Profile_7_Kind;
        AppSettingsV2.Instance.Profile_7_Location = AppSettings.Instance.Profile_7_Location;
        AppSettingsV2.Instance.Profile_7_Name = AppSettings.Instance.Profile_7_Name;
        AppSettingsV2.Instance.Profile_7_Placement = AppSettings.Instance.Profile_7_Placement;
        AppSettingsV2.Instance.Profile_7_Trans = AppSettings.Instance.Profile_7_Trans;

        AppSettingsV2.Instance.Profile_8 = AppSettings.Instance.Profile_8;
        AppSettingsV2.Instance.Profile_8_Condensed = AppSettings.Instance.Profile_8_Condensed;
        AppSettingsV2.Instance.Profile_8_FontSize = AppSettings.Instance.Profile_8_FontSize;
        AppSettingsV2.Instance.Profile_8_Kind = AppSettings.Instance.Profile_8_Kind;
        AppSettingsV2.Instance.Profile_8_Location = AppSettings.Instance.Profile_8_Location;
        AppSettingsV2.Instance.Profile_8_Name = AppSettings.Instance.Profile_8_Name;
        AppSettingsV2.Instance.Profile_8_Placement = AppSettings.Instance.Profile_8_Placement;
        AppSettingsV2.Instance.Profile_8_Trans = AppSettings.Instance.Profile_8_Trans;

        AppSettingsV2.Instance.Profile_9 = AppSettings.Instance.Profile_9;
        AppSettingsV2.Instance.Profile_9_Condensed = AppSettings.Instance.Profile_9_Condensed;
        AppSettingsV2.Instance.Profile_9_FontSize = AppSettings.Instance.Profile_9_FontSize;
        AppSettingsV2.Instance.Profile_9_Kind = AppSettings.Instance.Profile_9_Kind;
        AppSettingsV2.Instance.Profile_9_Location = AppSettings.Instance.Profile_9_Location;
        AppSettingsV2.Instance.Profile_9_Name = AppSettings.Instance.Profile_9_Name;
        AppSettingsV2.Instance.Profile_9_Placement = AppSettings.Instance.Profile_9_Placement;
        AppSettingsV2.Instance.Profile_9_Trans = AppSettings.Instance.Profile_9_Trans;

        AppSettingsV2.Instance.ScreenNumber = AppSettings.Instance.ScreenNumber;
        AppSettingsV2.Instance.SelProfile = AppSettings.Instance.SelProfile;
        AppSettingsV2.Instance.Sequence_1 = AppSettings.Instance.Sequence_1;
        AppSettingsV2.Instance.Sequence_10 = AppSettings.Instance.Sequence_10;
        AppSettingsV2.Instance.Sequence_2 = AppSettings.Instance.Sequence_2;
        AppSettingsV2.Instance.Sequence_3 = AppSettings.Instance.Sequence_3;
        AppSettingsV2.Instance.Sequence_4 = AppSettings.Instance.Sequence_4;
        AppSettingsV2.Instance.Sequence_5 = AppSettings.Instance.Sequence_5;
        AppSettingsV2.Instance.Sequence_6 = AppSettings.Instance.Sequence_6;
        AppSettingsV2.Instance.Sequence_7 = AppSettings.Instance.Sequence_7;
        AppSettingsV2.Instance.Sequence_8 = AppSettings.Instance.Sequence_8;
        AppSettingsV2.Instance.Sequence_9 = AppSettings.Instance.Sequence_9;
        AppSettingsV2.Instance.ShowUnits = AppSettings.Instance.ShowUnits;
        AppSettingsV2.Instance.UserColorsDim = AppSettings.Instance.UserColorsDim;
        AppSettingsV2.Instance.UserColorsInv = AppSettings.Instance.UserColorsInv;
        AppSettingsV2.Instance.UserColorsReg = AppSettings.Instance.UserColorsReg;
        AppSettingsV2.Instance.UserFonts = AppSettings.Instance.UserFonts;
        AppSettingsV2.Instance.VoiceCalloutProfile = AppSettings.Instance.VoiceCalloutProfile;
        AppSettingsV2.Instance.VoiceName = AppSettings.Instance.VoiceName;
      }
      catch {
        ; // DEBUG STOP
      }
      finally {
        AppSettingsV2.Instance.V2InUse = true;
        AppSettingsV2.Instance.Save( );
      }
    }

  }
}
