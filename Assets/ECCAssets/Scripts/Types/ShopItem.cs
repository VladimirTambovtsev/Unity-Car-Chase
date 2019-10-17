using UnityEngine;
using System;

namespace EndlessCarChase.Types
{
	/// <summary>
	/// Defines an item that can be unlocked in a shop with in-game money
	/// </summary>
	[Serializable]
	public class ShopItem
    {
        //[Tooltip("The name of the item, which is displayed in the shop")]
        //public string itemName = "Car";

        [Tooltip("The object representing this item, can be a 2D icon or a 3D object")]
        public Transform itemIcon;
        
        [Tooltip("Is the item locked or not? 0 = locked, 1 = unlocked")]
        public int lockState = 0;

        [Tooltip("How much money we need to unlock this item")]
        public int price = 1000;

    }
}