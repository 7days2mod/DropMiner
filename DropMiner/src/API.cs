using System.Collections.Generic;

public class API : IModApi
{
  public static Queue<int> DespawnQueue = new Queue<int>();

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
    GameManager.Instance.World.EntityLoadedDelegates += OnEntityLoaded;
  }

  private static void OnEntityLoaded(Entity entity)
  {
    if (entity is EntityFallingBlock)
    {
      DespawnQueue.Enqueue(entity.entityId);
    }
  }
}
