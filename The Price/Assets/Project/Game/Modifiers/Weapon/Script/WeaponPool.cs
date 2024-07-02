using System.Collections.Generic;
using UnityEngine;

public class WeaponPool : MonoBehaviour {

    public List<WeaponSystem> weapons = new List<WeaponSystem>();

    public WeaponSystem RandomPool()
    {
        int rnd = Random.Range(0, weapons.Count);

        return weapons[rnd];
    }
}