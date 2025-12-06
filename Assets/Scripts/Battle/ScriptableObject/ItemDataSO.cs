using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "ItemTable", menuName = "Item/ItemTable")]
public class ItemDataSO : ScriptableObject
{
	public string xPItem = "XPitem";
	public List<string> list;

}
