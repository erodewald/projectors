using System.Collections.Generic;
using System.Linq;

namespace Projectors {
  public class InputCommandCollection : List<InputCommand> {
    public InputCommandCollection() {}

    public InputCommandCollection(IEnumerable<InputCommand> range) {
      foreach (var inputCommand in range) {
        Add(inputCommand);
      }
    }

    /// <summary>
    /// The remote commands simulate the code send from IR remote handset.
    /// </summary>
    /// <returns></returns>
    public static InputCommandCollection GetVivitekRemoteCommands() {
      return new InputCommandCollection(new List<InputCommand> {
              new InputCommand(VivitekCommands.ArrowDownCommand, "Down")
              , new InputCommand(VivitekCommands.ArrowLeftCommand, "Left")
              , new InputCommand(VivitekCommands.ArrowRightCommand, "Right")
              , new InputCommand(VivitekCommands.ArrowUpCommand, "Up")
              , new InputCommand(VivitekCommands.AutoCommand, "Auto")
              , new InputCommand(VivitekCommands.AutoImageCommand, "Auto Image")
              , new InputCommand(VivitekCommands.BlankCommand, "Blank")
              , new InputCommand(VivitekCommands.EnterCommand, "Enter")
              , new InputCommand(VivitekCommands.ExitCommand, "Exit")
              , new InputCommand(VivitekCommands.FreezeCommand, "Freeze")
              , new InputCommand(VivitekCommands.InputCommand, "Input")
              , new InputCommand(VivitekCommands.InputComponentCommand, "Input Component")
              , new InputCommand(VivitekCommands.InputDviHdmi1Command, "Input DVI/HDMI 1")
              , new InputCommand(VivitekCommands.InputHdmi2Command, "Input HDMI 2")
              , new InputCommand(VivitekCommands.InputPcCommand, "Input PC")
              , new InputCommand(VivitekCommands.InputSVideoCommand, "Input S-Video")
              , new InputCommand(VivitekCommands.InputVideoCommand, "Input Video")
              , new InputCommand(VivitekCommands.KeystoneMinusCommand, "Keystone -")
              , new InputCommand(VivitekCommands.KeystonePlusCommand, "Keystone +")
              , new InputCommand(VivitekCommands.MenuCommand, "Menu")
              , new InputCommand(VivitekCommands.MuteCommand, "Mute")
              , new InputCommand(VivitekCommands.PowerCommand, "Power")
              , new InputCommand(VivitekCommands.PowerOffCommand, "Power Off")
              , new InputCommand(VivitekCommands.PowerOnCommand, "Power On")
              , new InputCommand(VivitekCommands.StatusCommand, "Status")
              , new InputCommand(VivitekCommands.VolumeDownCommand, "Volume -")
              , new InputCommand(VivitekCommands.VolumeUpCommand, "Volume +")
              , new InputCommand(VivitekCommands.ZoomInCommand, "Zoom +")
              , new InputCommand(VivitekCommands.ZoomOutCommand, "Zoom -")
      });
    }
  
    /// <summary>
    /// Returns a list of InputCommands which allow querying
    /// the projector
    /// </summary>
    /// <returns></returns>
    public static InputCommandCollection GetVivitekQueryCommands() {
      return new InputCommandCollection(new List<InputCommand> {
              new InputCommand(VivitekCommands.QueryBrightnessCommand, "Brightness")
              , new InputCommand(VivitekCommands.QueryColorCommand, "Color")
              , new InputCommand(VivitekCommands.QueryColorTemperatureCommand, "Color Temp")
              , new InputCommand(VivitekCommands.QueryContrastCommand, "Contrast")
              , new InputCommand(VivitekCommands.QueryInputSelectCommand, "Input Select")
              , new InputCommand(VivitekCommands.QueryLampHoursCommand, "Lamp Hrs")
              , new InputCommand(VivitekCommands.QueryPowerStateCommand, "Power State")
              , new InputCommand(VivitekCommands.QueryProjectionModeCommand, "Proj. Mode")
              , new InputCommand(VivitekCommands.QueryAspectCommand, "Aspect")
              , new InputCommand(VivitekCommands.QuerySoftwareVersionCommand, "Soft. Version")
              , new InputCommand(VivitekCommands.QueryTintCommand, "Tint")
              , new InputCommand(VivitekCommands.QuerySharpnessCommand, "Sharpness")
      });
    }
    
    /// <summary>
    /// Returns a list of InputCommands which allow setting 
    /// values on the projector, mostly corresponding with Query commands 
    /// </summary>
    /// <returns></returns>
    public static InputCommandCollection GetVivitekSetCommands() {
      return new InputCommandCollection(new List<InputCommand> {
              new InputCommand(VivitekCommands.SetBrightnessCommand, "Brightness", Enumerable.Range(0,101), VivitekCommands.QueryBrightnessCommand)
              , new InputCommand(VivitekCommands.SetColorCommand, "Color", Enumerable.Range(0,101), VivitekCommands.QueryColorCommand)
              , new InputCommand(VivitekCommands.SetColorTempCommand, "Color Temp", Enumerable.Range(0,6), VivitekCommands.QueryColorTemperatureCommand)
              , new InputCommand(VivitekCommands.SetContrastCommand, "Contrast", Enumerable.Range(0,101), VivitekCommands.QueryContrastCommand)
              , new InputCommand(VivitekCommands.SetProjectionModeCommand, "Proj. Mode", Enumerable.Range(0,4), VivitekCommands.QueryProjectionModeCommand)
              , new InputCommand(VivitekCommands.SetAspectCommand, "Aspect", Enumerable.Range(0,5), VivitekCommands.QueryAspectCommand)
              , new InputCommand(VivitekCommands.SetTintCommand, "Tint", Enumerable.Range(0,101), VivitekCommands.QueryTintCommand)
              , new InputCommand(VivitekCommands.SetSharpnessCommand, "Sharpness", Enumerable.Range(0,16), VivitekCommands.QuerySharpnessCommand)
      });
    }
  }

  public class InputCommand {
    public InputCommand(string command, string name, IEnumerable<int> range = null, string followUpQueryString = null) {
      AvailableRange = range;
      Command = command;
      Name = name;
      DependentCommand = followUpQueryString;
    }
    public InputCommand() {}
    public IEnumerable<int> AvailableRange { get; private set; }
    public string Command { get; private set; }
    public string Name { get; private set; }
    public string DependentCommand { get; private set; }
    public bool HasDependentQuery {
      get { return !string.IsNullOrWhiteSpace(DependentCommand); }
    }
    public bool IsSetter {
      get { return AvailableRange != null && AvailableRange.Any(); }
    }
  }
}
