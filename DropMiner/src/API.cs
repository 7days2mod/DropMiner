using System.Collections.Generic;
using System.Linq;
using DropMiner.Commands;

public class API : IModApi
{
  public static Queue<int> DespawnQueue = new Queue<int>();
  public static int EntityCountTrigger = 0;
  public static int FallingCountTrigger = 0;

  public void InitMod()
  {
    ModEvents.GameStartDone.RegisterHandler(GameStartDone);
    ModEvents.GameUpdate.RegisterHandler(GameUpdate);
  }

  private static void GameUpdate()
  {
    while (DespawnQueue.Count > 0)
    {
      GameManager.Instance.World.RemoveEntity(DespawnQueue.Dequeue(), EnumRemoveEntityReason.Despawned);
    }
  }

  private static void GameStartDone()
  {
    //Todo: check config and set or not
    BMDropMiner.SetEnabled();
  }

  internal static void OnEntityLoaded(Entity entity)
  {
    //If total entities is greater than the trigger then continue
    if (EntityCountTrigger != 0 && GameManager.Instance.World.Entities.Count < EntityCountTrigger) { return; }

    //If the entity is not a falling block then return
    if (!(entity is EntityFallingBlock)) { return; }

    if (FallingCountTrigger != 0 &&
        GameManager.Instance.World.Entities.list.OfType<EntityFallingBlock>()
          .Count() < FallingCountTrigger) { return; }

    DespawnQueue.Enqueue(entity.entityId);
  }
}
