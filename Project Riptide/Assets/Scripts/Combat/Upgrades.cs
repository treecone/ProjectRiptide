using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum StatusType {Armor, Hardiness, MaxHealth, Regeneration, DropQuality, DropRate, Maneuverability, ShipSpeed, Count, Damage, FireSpeed, ShotSize, SpreadAngle, VerticalRatio, Fire, Poison}
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
    private StatusEffects _statusEffects;

    public List<Upgrade> upgrades;
    public MasterUpgrade masterUpgrade;
    // Start is called before the first frame update
    void Start()
    {
        upgrades = new List<Upgrade>();
        masterUpgrade = new MasterUpgrade();
        _statusEffects = GetComponent<StatusEffects>();
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

        if(inventory != null)
        {
            for (int i = 0; i < inventory.Size; i++)
            {
                Item item = inventory[i];
                if (item.Id != 0 && item.Equipped == true)
                {
                    foreach (Upgrade u in item.Upgrades)
                    {
                        equippedUpgrades.Add(u);
                    }
                }
            }
        }
        
        for(int i = 0; i < _statusEffects.ActiveStatusEffects.Count; i++)
        {
            equippedUpgrades.Add(new Upgrade(_statusEffects.ActiveStatusEffects[i]));
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
    public StatusType upgradeType;
    public float upgradeValue;

    /// <summary>
    /// Creates a new upgrade, given a name and data
    /// </summary>
    /// <param name="_name">The name of the upgrade. This is entirely for flavor 
    /// and should not affect functionality.</param>
    /// <param name="_upgradeType">The upgrade type - the stat that the upgrade affects</param>
    /// <param name="_upgradeValue">The value of the upgrade - a float</param>
    public Upgrade(string _name, string _upgradeType, float _upgradeValue)
    {
        name = _name;
        Enum.TryParse(_upgradeType, out StatusType __upgradeType);
        upgradeType = __upgradeType;
        upgradeValue = _upgradeValue;
    }

    public Upgrade(StatusEffect statusEffect)
    {
        name = statusEffect.Type + " status effect";
        upgradeType = statusEffect.Type;
        upgradeValue = statusEffect.Level;
    }

    public virtual float this[StatusType key]
    {
        get
        {
            if(upgradeType == key)
            {
                return upgradeValue;
            } else
            {
                return 0;
            }
        }
    }

}

public class AdvancedUpgrade : Upgrade
{
    private StatusType scalingStat;
    private float scalingAmount;

    public AdvancedUpgrade(string _name, string _upgradeType, string _scalingStat, float _scalingAmount)
        :base(_name, _upgradeType, 0)
    {
        Enum.TryParse(_scalingStat, out StatusType __scalingStat);
        scalingStat = __scalingStat;
        scalingAmount = _scalingAmount;
    }

    public void Recalculate(MasterUpgrade masterUpgrade)
    {
        upgradeValue = masterUpgrade[scalingStat] * scalingAmount;
    }
}
[Serializable]
public class MasterUpgrade : Upgrade
{
    /// <summary>
    /// The upgrades that make up the Master Upgrade at the time
    /// </summary>
    [SerializeField]
    private List<Upgrade> _componentUpgrades;
    private Dictionary<StatusType, float> upgradeInfo;

    private List<AdvancedUpgrade> _advancedUpgrades;
    public MasterUpgrade() : base("master upgrade", "null", 0)
    {
        upgradeInfo = new Dictionary<StatusType, float>();
        _advancedUpgrades = new List<AdvancedUpgrade>();
    }

    public MasterUpgrade(string _name, Dictionary<StatusType, float> _upgradeInfo) : base(_name, "null", 0)
    {
        upgradeInfo = _upgradeInfo;
        _advancedUpgrades = new List<AdvancedUpgrade>();
    }
    /// <summary>
    /// Combines a list of upgrades into one master upgrade
    /// For multiple upgrades of the same category, adds numbers together
    /// </summary>
    public void Recalculate(List<Upgrade> upgrades)
    {
        _componentUpgrades = upgrades;
        upgradeInfo.Clear();
        foreach(Upgrade upgrade in upgrades)
        {
            if(upgradeInfo.ContainsKey(upgrade.upgradeType))
            {
                upgradeInfo[upgrade.upgradeType] += upgrade.upgradeValue;
            } else
            {
                upgradeInfo[upgrade.upgradeType] = upgrade.upgradeValue;
            }
        }
        foreach(AdvancedUpgrade advancedUpgrade in _advancedUpgrades)
        {
            advancedUpgrade.Recalculate(this);
        }
    }

    /// <summary>
    /// An indexer to access the different stats of the master upgrade.
    /// </summary>
    /// <param name="key">The name of the </param>
    /// <returns></returns>
    public override float this[StatusType key]
    {
        get
        {
            if (upgradeInfo.ContainsKey(key))
            {
                return upgradeInfo[key];
            }
            else
            {
                return 0;
            }
        }
    }
}