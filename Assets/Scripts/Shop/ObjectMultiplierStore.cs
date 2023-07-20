using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Jack's code
[System.Serializable]
public struct ObjectMultipliers
{
    // used for crafting speeds
    public float counterMultipler;
    public float crucibleMultipler;
    public float anvilMultipler;
    public float combinerMultipler;
    public float oreMinerMultipler;

    //used for increasing storage
    public int stoneBarrelStorage;
    public int copperBarrelStorage;
    public int tinBarrelStorage;
    public int ironBarrelStorage;
    public int goldBarrelStorage ;

    public ObjectMultipliers(bool useDefaulValues,float counterMultipler_ = 1.0f, float crucibleMultipler_ = 1.0f, float anvilMultipler_ = 1.0f, float combinerMultipler_ = 1.0f, float oreMinerMultipler_ = 1.0f,
     int stoneBarrelStorage_ = 2, int copperBarrelStorage_ = 2, int tinBarrelStorage_ = 2, int ironBarrelStorage_ = 2, int goldBarrelStorage_ = 2) //a list storing all of the multipliers
    {
        counterMultipler = counterMultipler_;
        crucibleMultipler = crucibleMultipler_;
        anvilMultipler = anvilMultipler_;
        combinerMultipler = combinerMultipler_;
        oreMinerMultipler = oreMinerMultipler_;

        stoneBarrelStorage = stoneBarrelStorage_;
        copperBarrelStorage = copperBarrelStorage_;
        tinBarrelStorage = tinBarrelStorage_;
        ironBarrelStorage = ironBarrelStorage_;
        goldBarrelStorage = goldBarrelStorage_;
    }
}
public class ObjectMultiplierStore : MonoBehaviour
{
    public ObjectMultipliers multipliers = new ObjectMultipliers(true);

    private void Start()
    { //create the list using default values
        multipliers = new ObjectMultipliers(true);
    }
}
//end of Jack's code
