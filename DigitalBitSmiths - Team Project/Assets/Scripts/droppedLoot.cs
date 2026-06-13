using UnityEngine;


[System.Serializable]
public class droppedLoot 
{
    //This will be part of the enemy llot system to help with enemy drops and provide the player some resources while they are fighting through the enemys
    //will still have a few pickups in the enviornment 

    public GameObject itemPrefab;
    [Range(0, 100)] public float dropChance;

   
}
