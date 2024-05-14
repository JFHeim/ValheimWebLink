namespace VWL_WorldObjectsData;

internal static class HandlerSetPlayerData
{
    private static Transform? spawnHolder;
    
    private static void ChangeInventory(Player pl, InventoryData inventoryData)
    {
        if (spawnHolder == null)
        {
            spawnHolder = new GameObject("VWL_SpawnHolder").transform;
            spawnHolder.gameObject.SetActive(false);
        }
        
        var plInv = pl.GetInventory().m_inventory;
        foreach (var item in inventoryData.items)
        {
            var itemData = plInv.Find(x => x.m_gridPos.ToVector2() == item.gridPos);
            if (itemData == null)
            {
                var itemDrop = ObjectDB.instance.GetItem(item.prefabName);
                if (!itemDrop) continue;
                itemDrop = Instantiate(itemDrop, parent: spawnHolder);
                if (item.gridPos.HasValue) itemDrop.m_itemData.m_gridPos = item.gridPos.Value;
                if (item.stack.HasValue) itemDrop.m_itemData.m_stack = item.stack.Value;
                if (item.durability.HasValue) itemDrop.m_itemData.m_durability = item.durability.Value;
                if (item.quality.HasValue) itemDrop.m_itemData.m_quality = item.quality.Value;
                if (item.variant.HasValue) itemDrop.m_itemData.m_variant = item.variant.Value;
                if (item.crafterName != null) itemDrop.m_itemData.m_crafterName = item.crafterName;
                if (item.stack.HasValue) plInv.Add(itemDrop.m_itemData);
                Destroy(itemDrop.gameObject);
                continue;
            }

            if (item.stack.HasValue) itemData.m_stack = item.stack.Value;
            if (item.durability.HasValue) itemData.m_durability = item.durability.Value;
            if (item.quality.HasValue) itemData.m_quality = item.quality.Value;
            if (item.variant.HasValue) itemData.m_variant = item.variant.Value;
            if (item.crafterName != null) itemData.m_crafterName = item.crafterName;
        }
    }

    public static void SetPlayerData(long _, string name, string dataJson)
    {
        PlayerData data;
        try
        {
            data = JSON.ToObject<PlayerData>(dataJson);
        }
        catch (Exception e)
        {
            DebugError($"HandlerSetPlayerData: Failed to parse PlayerData from JSON: {e}");
            return;
        }

        var pl = m_localPlayer;
        if (!pl || name != pl.GetPlayerName()) return;
        Debug($"Got HandlerSetPlayerData for self\n{data.GetObjectString()}");

        if (data.inDebugFly.HasValue) pl.m_debugFly = data.inDebugFly.Value;
        if (data.maxHealth.HasValue) pl.SetMaxHealth(data.maxHealth.Value);
        if (data.health.HasValue) pl.SetHealth(data.health.Value);
        if (data.eitr.HasValue) pl.m_eitr = data.eitr.Value;
        if (data.eitr.HasValue) pl.m_maxEitr = Max(pl.m_eitr, data.eitr.Value);
        if (data.stamina.HasValue) pl.m_stamina = data.stamina.Value;
        if (data.stamina.HasValue) pl.m_maxStamina = Max(pl.m_stamina, data.stamina.Value);
        if (data.pvp.HasValue) pl.SetPVP(data.pvp.Value);
        if (data.inBed.HasValue) pl.SetSleeping(data.inBed.Value);
        if (data.noise.HasValue) pl.m_noiseRange = data.noise.Value;
        if (data.noise.HasValue) pl.m_nview.GetZDO().Set(ZDOVars.s_noise, data.noise.Value);
        if (data.position.HasValue) pl.TeleportTo(data.position.Value, Quaternion.identity, false);
        if (data.publicPosition.HasValue) ZNet.instance.SetPublicReferencePosition(data.publicPosition.Value);
        if (data.inventory.HasValue) ChangeInventory(pl, data.inventory.Value);
    }
}