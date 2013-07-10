using System;

namespace Projectors {
  public static class VivitekResponses {
    public static string AckResponse = "\r";
  }
  public static class VivitekCommands {
    private static string BaseCommand = "~{0}" + Environment.NewLine;
    public static string PowerOnCommand = string.Format(BaseCommand, "PN");
    public static string PowerOffCommand = string.Format(BaseCommand, "PF");
    public static string AutoImageCommand = string.Format(BaseCommand, "AI");
    public static string InputPcCommand = string.Format(BaseCommand, "SR");
    public static string InputDviHdmi1Command = string.Format(BaseCommand, "SD");
    public static string InputHdmi2Command = string.Format(BaseCommand, "SH");
    public static string InputVideoCommand = string.Format(BaseCommand, "SV");
    public static string InputSVideoCommand = string.Format(BaseCommand, "SS");
    public static string InputComponentCommand = string.Format(BaseCommand, "SY");

    public static string ArrowUpCommand = string.Format(BaseCommand, "rU");
    public static string ArrowDownCommand = string.Format(BaseCommand, "rD");
    public static string ArrowLeftCommand = string.Format(BaseCommand, "rL");
    public static string ArrowRightCommand = string.Format(BaseCommand, "rR");
    public static string PowerCommand = string.Format(BaseCommand, "rP");
    public static string ExitCommand = string.Format(BaseCommand, "rE");
    public static string InputCommand = string.Format(BaseCommand, "rI");
    public static string AutoCommand = string.Format(BaseCommand, "rA");
    public static string KeystonePlusCommand = string.Format(BaseCommand, "rK");
    public static string KeystoneMinusCommand = string.Format(BaseCommand, "rJ");
    public static string MenuCommand = string.Format(BaseCommand, "rM");
    public static string StatusCommand = string.Format(BaseCommand, "rS");
    public static string MuteCommand = string.Format(BaseCommand, "rT");
    public static string ZoomInCommand = string.Format(BaseCommand, "rZ");
    public static string ZoomOutCommand = string.Format(BaseCommand, "rY");
    public static string BlankCommand = string.Format(BaseCommand, "rB");
    public static string FreezeCommand = string.Format(BaseCommand, "rF");
    public static string VolumeUpCommand = string.Format(BaseCommand, "rV");
    public static string VolumeDownCommand = string.Format(BaseCommand, "rW");
    public static string EnterCommand = string.Format(BaseCommand, "rN");

    /// <summary>
    /// Set Data Range: 0-100
    /// Source: ALL
    /// </summary>
    public static string SetBrightnessCommand = string.Format(BaseCommand, "sB{0}");
    /// <summary>
    /// Set Data Range: 0-100
    /// Source: ALL
    /// </summary>
    public static string SetContrastCommand = string.Format(BaseCommand, "sC{0}");
    /// <summary>
    /// Set Data Range: 0-100
    /// Source: Video/S-Video/Component
    /// </summary>
    public static string SetColorCommand = string.Format(BaseCommand, "sR{0}");
    /// <summary>
    /// Set Data Range: 0-100
    /// Source: Video/S-Video/Component
    /// </summary>
    public static string SetTintCommand = string.Format(BaseCommand, "sN{0}");
    /// <summary>
    /// Set Data Range: (0) auto (1) 16:9 (2) 4:3 (3,4) LetterBox (5) Real
    /// Source: ALL
    /// </summary>
    public static string SetAspectCommand = string.Format(BaseCommand, "sA{0}");
    /// <summary>
    /// Set Data Range: (0) LAMP Native (1) Warm (2) Normal (3) Cool (4) Cooler (5) High Cool
    /// Source: ALL
    /// </summary>
    public static string SetColorTempCommand = string.Format(BaseCommand, "sT{0}");
    /// <summary>
    /// Set Data Range: (0) Front Table (1) Front (2) Ceiling (3) Rear Table (4) Rear+ Ceiling
    /// Source: ALL
    /// </summary>
    public static string SetProjectionModeCommand = string.Format(BaseCommand, "sJ{0}");
    /// <summary>
    /// Set Data Range: 0 - 15
    /// Source: ALL
    /// </summary>
    public static string SetSharpnessCommand = string.Format(BaseCommand, "sH{0}");

    public static string QuerySoftwareVersionCommand = string.Format(BaseCommand, "qV");
    public static string QueryPowerStateCommand = string.Format(BaseCommand, "qP");
    public static string QueryInputSelectCommand = string.Format(BaseCommand, "qS");
    public static string QueryLampHoursCommand = string.Format(BaseCommand, "qL");
    public static string QueryBrightnessCommand = string.Format(BaseCommand, "qB");
    public static string QueryContrastCommand = string.Format(BaseCommand, "qC");
    public static string QueryColorCommand = string.Format(BaseCommand, "qR");
    public static string QueryTintCommand = string.Format(BaseCommand, "qN");
    public static string QueryAspectCommand = string.Format(BaseCommand, "qA");
    public static string QueryColorTemperatureCommand = string.Format(BaseCommand, "qT");
    public static string QueryProjectionModeCommand = string.Format(BaseCommand, "qJ");
    public static string QuerySharpnessCommand = string.Format(BaseCommand, "qH");
  }
}
