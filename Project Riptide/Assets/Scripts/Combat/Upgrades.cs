using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    /// <summary>
    /// Ship Upgrade System
    /// 
    /// Each Item can have one or more Upgrades in it, which can change stats or provide
    /// special behavior. Upgrades consist of a collection of stat values, which affect
    /// properties such as shot power or ship movement speed.
    /// 
    /// Whenever the player's equipment changes, the upgrades will recalculate into a single 
    /// "master upgrade". Other scripts should ONLY reference the master upgrade if they are
    /// trying to change ship behavior. This will be better for performance than checking every 
    /// upgrade every frame.
    /// </summary>
    /// 

    public Inventory inventory;
        
    public List<Upgrade> upgrades;
    public MasterUpgrade masterUpgrade;
    // Start is called before the first frame update
    void Start()
    {
        upgrades = new List<Upgrade>();
        masterUpgrade = new MasterUpgrade();
    }

    // Some debug upgrades that could be added
    void Update()
    {/*
        if(Input.GetKeyDown(KeyCode.U))
        {
            AddUpgrade(new Upgrade("Grape Shot", new { damage = 3 }));
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            AddUpgrade(new Upgrade("High Sails", new { shipSpeed = 0.2f }));
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            AddUpgrade(new Upgrade("Extra Cannons", new { count = 1 }));
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            AddUpgrade(new Upgrade("Lob Shot", new { verticalRatio = 0.2f }));
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            upgrades.Clear();
            Recalculate();
        }*/

        if(Input.GetKeyDown(KeyCode.R))
        {
            Recalculate();
        }
    }

    /// <summary>
    /// Directly adds an upgrade to the ship
    /// </summary>
    /// <param name="u">The upgrade to add</param>
    public void AddUpgrade(Upgrade u)
    {
        upgrades.Add(u);
        Recalculate();
    }

    /// <summary>
    /// Recalculates the Master Upgrade, a sum of all upgrades on equipped items.
    /// 
    /// Other parts of the code trying to get upgrade values should reference the master upgrade
    /// </summary>
    public void Recalculate()
    {
        List<Upgrade> equippedUpgrades = new List<Upgrade>();
        for (int i = 0; i < inventory.Size; i++) {
            Item item = inventory[i];
            if (item.equipped == true)
            {
                foreach(Upgrade u in item.upgrades)
                {
                    equippedUpgrades.Add(u);
                }
            }
        }
        masterUpgrade.Recalculate(equippedUpgrades);
    }
}

/// <summary>
/// A base class for upgrades
/// </summary>
[Serializable]
public class Upgrade
{
    public string name;
    /// <summary>
    /// A dictionary of the different properties of the upgrade
    /// The key represents the upgrade category
    /// </summary>
    public Dictionary<string, float> upgradeInfo;

    /// <summary>
    /// Creates a new upgrade, given a name and data
    /// </summary>
    /// <param name="_name">The name of the upgrade. This is entirely for flavor 
    /// and should not affect functionality.</param>
    /// <param name="data">The actual gameplay data of the upgrade. This is input in
    /// the form of an anonymous object - for example:
    /// 
    ///     new { velocity = 0.2f }
    ///     
    /// TODO: add check to make sure all anonymous object parameters are actual stats that exist
    /// current stats:
    /// count       - The amount of cannonballs that are fired
    /// damage      - The amount of damage each cannonball does
    /// fireSpeed   - The physical speed the cannonball comes out at
    /// verticalRatio- The ratio of the vertical component of the shot velocity to the horizontal
    ///                At the moment, if you increase this, horizontal speed will not decrease
    ///                to accomodate for it, so the cannonball will go super far
    /// shipSpeed    - The ship's movement speed
    /// hardiness   - The amount of knockback reduction the ship has (2 hardiness means you take 1/2 knockback, etc)
    /// 
    /// </param>
    public Upgrade(string _name, object data)
    {
        name = _name;
        upgradeInfo = new Dictionary<string, float>();

        if(data != null)
        {
            foreach(PropertyDescriptor pd in TypeDescriptor.GetProperties(data))
            {
                float f = Convert.ToSingle(pd.GetValue(data));
                upgradeInfo.Add(pd.Name, f);
            }
        }
    }

    public Upgrade(string _name, Dictionary<string, float> data)
    {
        name = _name;
        upgradeInfo = data;
    }

    /// <summary>
    /// An indexer to access the different stats of the upgrade.
    /// This is primarily used by the master upgrade in actual gameplay
    /// but having it on any individual upgrade could be helpful for UI stuff
    /// </summary>
    /// <param name="key">The name of the </param>
    /// <returns></returns>
    public float this[string key]
    {
        get {
            if (upgradeInfo.ContainsKey(key))
            {
                return upgradeInfo[key];
            } else
            {
                return 0;
            }
        }
    }
}

public class MasterUpgrade : Upgrade
{
    /// <summary>
    /// The upgrades that make up the Master Upgrade at the time
    /// </summary>
    List<Upgrade> componentUpgrades;

    public MasterUpgrade() : base("master upgrade", null)
    {
        upgradeInfo = new Dictionary<string, float>();
    }
    /// <summary>
    /// Combines a list of upgrades into one master upgrade
    /// For multiple upgrades of the same category, adds numbers together
    /// </summary>
    public void Recalculate(List<Upgrade> upgrades)
    {
        componentUpgrades = upgrades;
        upgradeInfo.Clear();
        foreach(Upgrade upgrade in upgrades)
        {
            foreach(string category in upgrade.upgradeInfo.Keys)
            {
                if(upgradeInfo.ContainsKey(category))
                {
                    upgradeInfo[category] += upgrade.upgradeInfo[category];
                } else
                {
                    upgradeInfo[category] = upgrade.upgradeInfo[category];
                }
            }
        }
    }
}