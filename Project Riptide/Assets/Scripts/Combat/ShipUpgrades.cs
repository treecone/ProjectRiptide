using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUpgrades : MonoBehaviour
{
    /// <summary>
    /// Ship Upgrade System
    /// 
    /// Each Item can have one or more Upgrades in it, which can change stats or provide
    /// special behavior. Upgrades can be a specific type (Shot, OnHit, Movement), which
    /// would be a simple stat-buff, or be Special, which has its own hard-coded behavior
    /// 
    /// Whenever the player's equipment changes, the upgrades will recalculate. This
    /// should be better performance than checking every frame.
    /// </summary>
    [SerializeField]
    public List<Upgrade> upgrades;
    // Start is called before the first frame update
    void Start()
    {
        upgrades = new List<Upgrade>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            upgrades.Add(new ShotUpgrade("Grape Shot", 0, 2, 0));
        }
    }
    public List<Upgrade> UpgradesOfType(Upgrade.UpgradeType type)
    {
        List<Upgrade> shipUpgrades = new List<Upgrade>();
        foreach(Upgrade s in upgrades)
        {
            if(s.upgradeType == type)
            {
                shipUpgrades.Add(s);
            }
        }
        return shipUpgrades;
    }
}


[Serializable]
public abstract class Upgrade
{

    public enum UpgradeType { Shot, OnHit, Movement, Special }
    public string name;
    public UpgradeType upgradeType;

    public Upgrade()
    {

    }
}

[Serializable]
public class ShotUpgrade : Upgrade
{
    public int count;
    public int damage;
    public float fireSpeed;

    public ShotUpgrade(string _name, int _count = 0, int _damage = 0, float _fireSpeed = 0)
    {
        name = _name;
        upgradeType = UpgradeType.Shot;
        count = _count;
        damage = _damage;
        fireSpeed = _fireSpeed;
    }
}