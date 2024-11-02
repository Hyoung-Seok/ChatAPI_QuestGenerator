using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private int count;

    public string ItemName => itemName;
    public int Count => count;
}
