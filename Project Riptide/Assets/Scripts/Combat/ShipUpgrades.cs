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
    /// 

        
    public List<Upgrade> upgrades;
    public ShotUpgrade masterShotUpgrade;
    public MovementUpgrade masterMovementUpgrade;
    // Start is called before the first frame update
    void Start()
    {
        upgrades = new List<Upgrade>();
        masterShotUpgrade = new ShotUpgrade("master shot");
        masterMovementUpgrade = new MovementUpgrade("master movement");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            AddUpgrade(new ShotUpgrade("Grape Shot", 0, 2, 0));
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            AddUpgrade(new MovementUpgrade("High Sails", 0.2f));
        }
    }
    /// <summary>
    /// Gets all upgrades on the ship of a given type
    /// </summary>
    /// <param name="type">The type of upgrade you are looking for </param>
    /// <returns>A list populated with the upgrades of that type</returns>
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

    public void AddUpgrade(Upgrade u)
    {
        upgrades.Add(u);
        Recalculate();
    }
    public void Recalculate()
    {
        masterShotUpgrade.Recalculate(UpgradesOfType(Upgrade.UpgradeType.Shot));
        masterMovementUpgrade.Recalculate(UpgradesOfType(Upgrade.UpgradeType.Movement));
    }
}

/// <summary>
/// An abstract base class for upgrades
/// </summary>
[Serializable]
public abstract class Upgrade
{

    public enum UpgradeType { Shot, OnHit, Movement, Special }
    public string name;
    public UpgradeType upgradeType;
}

/// <summary>
/// An upgrade to the ship's shooting power
/// </summary>
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

    public void Recalculate(List<Upgrade> shotUpgrades)
    {
        count = 0;
        damage = 0;
        fireSpeed = 0;
        foreach(Upgrade u in shotUpgrades)
        {
            ShotUpgrade su = (ShotUpgrade)u;
            count += su.count;
            damage += su.damage;
            fireSpeed += su.fireSpeed;
        }
    }
}

/// <summary>
/// An upgrade to the ship's moving ability
/// </summary>
public class MovementUpgrade : Upgrade
{
    public float velocity;

    public MovementUpgrade(string _name, float _velocity = 1)
    {
        name = _name;
        upgradeType = UpgradeType.Movement;
        velocity = _velocity;
    }

    public void Recalculate(List<Upgrade> movementUpgrades)
    {
        velocity = 1;
        foreach(Upgrade u in movementUpgrades)
        {
            MovementUpgrade mu = (MovementUpgrade)u;
            velocity += mu.velocity;
        }
    }
}