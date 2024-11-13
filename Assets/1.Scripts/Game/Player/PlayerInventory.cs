using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    private readonly Dictionary<string, int> _inventory = new Dictionary<string, int>();
    public event Action<ItemData, int> CheckQuestAction; 
    
    public void AddItem(ItemData data)
    {
        if (_inventory.ContainsKey(data.ItemName) == false)
        {
            _inventory.Add(data.ItemName, 0);
        }

        _inventory[data.ItemName] += data.Count;
        CheckQuestAction?.Invoke(data, _inventory[data.ItemName]);
    }

    public void RemoveItem(QuestData data)
    {
        foreach (var target in data.TargetInfos)
        {
            if (_inventory.ContainsKey(target.TargetName) == false)
            {
                continue;
            }

            _inventory[target.TargetName] -= target.TargetCount;
        }
    }
}
