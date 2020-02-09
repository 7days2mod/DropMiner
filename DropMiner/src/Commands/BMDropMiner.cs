using System.Collections.Generic;

namespace DropMiner.Commands
{
  public class BMDropMiner : BMCmdAbstract
  {
    public static bool Enabled = false;

    private const string PrimaryCmd = "bm-drop";
    private const string SubCmdEnable = "enable";
    private const string SubCmdDisable = "disable";
    private const string SubCmdTriggerCount = "triggercount";
    private const string CmdOptionEntities = "entities";
    private const string CmdOptionFalling = "falling";
    public const int MaxCount = 30000;

    public static readonly List<string> Help = new List<string>
    {
      $"1. {PrimaryCmd}",
      $"2. {PrimaryCmd} {SubCmdEnable}",
      $"3. {PrimaryCmd} {SubCmdDisable}",
      $"4. {PrimaryCmd} {SubCmdTriggerCount} {CmdOptionEntities} <count>",
      $"5. {PrimaryCmd} {SubCmdTriggerCount} {CmdOptionFalling} <count>",
      " 1. Get this help and current settings.",
      " 2. Enable falling block removal.",
      " 3. Disable falling block removal.",
      " 4. Set the total entities limit before falling block removal is triggered.",
      " 5. Set the total falling blocks limit before falling block removal is triggered.",
      " Note: Set option the triggers to 0 to always trigger",
      "       The total entities check is might lighter on the cpu than the falling blocks trigger.",
      "       This will allow for falling blocks when the server is not busy, but remove them when it is.",
      " Example:",
      $"          {PrimaryCmd} {SubCmdTriggerCount} {CmdOptionEntities} 400",
      $"          {PrimaryCmd} {SubCmdTriggerCount} {CmdOptionFalling} 10",
      $"          This will start checking for falling blocks when the total server entities count hits 400 (includes dropped items),",
      $"          and queue for removal any falling blocks when there are more than 10 at once."
    };

    public override string[] GetCommands() => new[] { PrimaryCmd };

    public override string GetDescription() => "Change options for drop mining.";

    public override string GetHelp() => $"{GetDescription()}\n{string.Join("\n", Help)}";

    public override void TryExecute(IList<string> _params, CommandSenderInfo _senderInfo)
    {
      if (_params.Count == 0)
      {
        SendHelp();
        SendConsole($"Entity count trigger: {API.EntityCountTrigger}, Falling block count trigger: {API.FallingCountTrigger}, ");

        return;
      }

      switch (_params[0])
      {
        case SubCmdEnable:
          if (!Enabled) { SetEnabled(); }

          break;
        case SubCmdDisable:
          if (Enabled) { SetDisabled(); }

          break;
        case SubCmdTriggerCount:
          if (_params.Count < 3)
          {
            SendHelp();
            return;
          }

          if (!int.TryParse(_params[2], out var count) || count < 0 || count > MaxCount)
          {
            SendConsole($"Count must be a number between 0 and {MaxCount}");
            return;
          }

          switch (_params[1])
          {
            case CmdOptionEntities:
              API.EntityCountTrigger = count;
              break;
            case CmdOptionFalling:
              API.FallingCountTrigger = count;
              break;
            default:
              SendConsole($"Unknown option for {SubCmdTriggerCount} {_params[1]}");
              break;
          }

          break;
        default:
          SendConsole($"Unknown sub command {_params[0]}");
          break;
      }
    }

    internal static void SetEnabled()
    {
      Enabled = true;
      GameManager.Instance.World.EntityLoadedDelegates += API.OnEntityLoaded;

    }

    internal static void SetDisabled()
    {
      Enabled = false;
      GameManager.Instance.World.EntityLoadedDelegates -= API.OnEntityLoaded;
    }
  }
}
