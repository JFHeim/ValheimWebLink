namespace VWL_WorldObjectsData;

public static class HandlerSendPlayerInventory
{
    public static void SendInventoryToServer(long _, string name)
    {
        Debug($"Server asking me for inventory of {name}");
        var pl = m_localPlayer;
        if (!pl || !pl.GetPlayerName().Equals(name)) return;
        var result = new InventoryData();
        foreach (var itemData in pl.GetInventory().m_inventory)
        {
            result.items.Add(new()
            {
                prefabName = itemData.m_dropPrefab.name,
                durability = itemData.m_durability,
                quality = itemData.m_quality,
                stack = itemData.m_stack,
                crafterName = itemData.m_crafterName,
                variant = itemData.m_variant,
                gridPos = itemData.m_gridPos,
            });
        }

        ZRoutedRpc.instance.InvokeRoutedRPC("VWL_GotInventory_Server", name, JSON.ToJSON(result));
    }
}