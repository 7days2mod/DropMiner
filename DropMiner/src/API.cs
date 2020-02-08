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
      var entityId = DespawnQueue.Dequeue();

      GameManager.Instance.World.RemoveEntity(entityId, EnumRemoveEntityReason.Despawned);

      //var message = $"DROP-MINE: Removing {entityId}.";
      //Log.Out(message);
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
      //var message = $"DROP-MINE: Queuing ({entity.entityId}) @ {entity.position} for despawn.";
      //Log.Out(message);

      DespawnQueue.Enqueue(entity.entityId);

      return;
    }

    if (EntityClass.list.TryGetValue(entity.entityClass, out var entityClass))
    {
      var entityCount = GameManager.Instance.World.Entities.Count;
      var message = $"DROP-MINE: Entity Spawned {entityClass.entityClassName}({entity.entityId}) @ {entity.position} - Total: {entityCount}";

      Log.Out(message);
    }
  }

  //private static void SendGlobal(string message) =>
  //  GameManager.Instance.ChatMessageServer(null, EChatType.Global, -1, message, "Server", false, null);
}
