using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipBase { Default, Raft, JunkT1, JunkT2, JunkT3, JunkT4, SloopT1, SloopT2, SloopT3, SloopT4, SchoonerT1, SchoonerT2, SchoonerT3, SchoonerT4, BrigantineT1, BrigantineT2, BrigantineT3, BrigantineT4, GalleonT1, GalleonT2, GalleonT3, GalleonT4};
public enum ShipSails { Default, PatchworkSails, SilkSailsT1, SilkSailsT2, SilkSailsT3, SilkSailsT4, BurlapSailsT1, BurlapSailsT2, BurlapSailsT3, BurlapSailsT4, CanvasSailsT1, CanvasSailsT2, CanvasSailsT3, CanvasSailsT4, GoldScaleSailsT1, GoldScaleSailsT2, GoldScaleSailsT3, GoldScaleSailsT4, CrabClawSailsT1, CrabClawSailsT2, CrabClawSailsT3, CrabClawSailsT4, FlowerPetalSailsT1, FlowerPetalSailsT2, FlowerPetalSailsT3, FlowerPetalSailsT4, SeaweedSailsT1, SeaweedSailsT2, SeaweedSailsT3, SeaweedSailsT4 };
public enum ShipHull { Default, RudimentaryHull, WoodenHullT1, WoodenHullT2, WoodenHullT3, WoodenHullT4, SteelHullT1, SteelHullT2, SteelHullT3, SteelHullT4, BoulderHullT1, BoulderHullT2, BoulderHullT3, BoulderHullT4, ClamShellHullT1, ClamShellHullT2, ClamShellHullT3, ClamShellHullT4, SeaglassHullT1, SeaglassHullT2, SeaglassHullT3, SeaglassHullT4, MonkeyHullT1, MonkeyHullT2, MonkeyHullT3, MonkeyHullT4};
public enum ShipCannon { Default, RustedCannon, CannonT1, CannonT2, CannonT3, CannonT4, CrabClawCannonT1, CrabClawCannonT2, CrabClawCannonT3, CrabClawCannonT4, SteelCannonT1, SteelCannonT2, SteelCannonT3, SteelCannonT4, FireworksCannonT1, FireworksCannonT2, FireworksCannonT3, FireworksCannonT4, PoisonCannonT1, PoisonCannonT2, PoisonCannonT3, PoisonCannonT4, BubbleBeamCannonT1, BubbleBeamCannonT2, BubbleBeamCannonT3, BubbleBeamCannonT4, FlameCannonT1, FlameCannonT2, FlameCannonT3, FlameCannonT4 };
public enum ShipTrinket { Default, GrandmasCookies, MonkeysCrown, LuckyCarpChimes, FishFeatherFan, LotusBlossom, PearlescentMedicine, Lemon, Lemonade}

[CreateAssetMenu(menuName = "Custom Assets/Singletons/StartingItems")]
public class StartingItems : ScriptableObject
{
    private static StartingItems _instance;

    public static StartingItems Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.LoadAll<StartingItems>("ScriptableObjectInstances")[0];
            return _instance;
        }
    }

    void OnEnable()
    {
        _instance = this;
        Setup();
    }

    public void Setup()
    {
        //Set up ship base dictionary
        _shipBaseDictionary.Add(ShipBase.Default, "raft1");
        _shipBaseDictionary.Add(ShipBase.Raft, "raft1");
        _shipBaseDictionary.Add(ShipBase.JunkT1, "junk1");
        _shipBaseDictionary.Add(ShipBase.JunkT2, "junk2");
        _shipBaseDictionary.Add(ShipBase.JunkT3, "junk3");
        _shipBaseDictionary.Add(ShipBase.JunkT4, "junk4");
        _shipBaseDictionary.Add(ShipBase.SloopT1, "sloop1");
        _shipBaseDictionary.Add(ShipBase.SloopT2, "sloop2");
        _shipBaseDictionary.Add(ShipBase.SloopT3, "sloop3");
        _shipBaseDictionary.Add(ShipBase.SloopT4, "sloop4");
        _shipBaseDictionary.Add(ShipBase.SchoonerT1, "schooner1");
        _shipBaseDictionary.Add(ShipBase.SchoonerT2, "schooner2");
        _shipBaseDictionary.Add(ShipBase.SchoonerT3, "schooner3");
        _shipBaseDictionary.Add(ShipBase.SchoonerT4, "schooner4");
        _shipBaseDictionary.Add(ShipBase.BrigantineT1, "brigantine1");
        _shipBaseDictionary.Add(ShipBase.BrigantineT2, "brigantine2");
        _shipBaseDictionary.Add(ShipBase.BrigantineT3, "brigantine3");
        _shipBaseDictionary.Add(ShipBase.BrigantineT4, "brigantine4");
        _shipBaseDictionary.Add(ShipBase.GalleonT1, "galleon1");
        _shipBaseDictionary.Add(ShipBase.GalleonT2, "galleon2");
        _shipBaseDictionary.Add(ShipBase.GalleonT3, "galleon3");
        _shipBaseDictionary.Add(ShipBase.GalleonT4, "galleon4");

        //Set up sails dictionary
        _shipSailsDictionary.Add(ShipSails.Default, "patchworksails1");
        _shipSailsDictionary.Add(ShipSails.PatchworkSails, "patchworksails1");
        _shipSailsDictionary.Add(ShipSails.SilkSailsT1, "silksails1");
        _shipSailsDictionary.Add(ShipSails.SilkSailsT2, "silksails2");
        _shipSailsDictionary.Add(ShipSails.SilkSailsT3, "silksails3");
        _shipSailsDictionary.Add(ShipSails.SilkSailsT4, "silksails4");
        _shipSailsDictionary.Add(ShipSails.CanvasSailsT1, "canvassails1");
        _shipSailsDictionary.Add(ShipSails.CanvasSailsT2, "canvassails2");
        _shipSailsDictionary.Add(ShipSails.CanvasSailsT3, "canvassails3");
        _shipSailsDictionary.Add(ShipSails.CanvasSailsT4, "canvassails4");
        _shipSailsDictionary.Add(ShipSails.BurlapSailsT1, "burlapsails1");
        _shipSailsDictionary.Add(ShipSails.BurlapSailsT2, "burlapsails2");
        _shipSailsDictionary.Add(ShipSails.BurlapSailsT3, "burlapsails3");
        _shipSailsDictionary.Add(ShipSails.BurlapSailsT4, "burlapsails4");
        _shipSailsDictionary.Add(ShipSails.CrabClawSailsT1, "crabclawsails1");
        _shipSailsDictionary.Add(ShipSails.CrabClawSailsT2, "crabclawsails2");
        _shipSailsDictionary.Add(ShipSails.CrabClawSailsT3, "crabclawsails3");
        _shipSailsDictionary.Add(ShipSails.CrabClawSailsT4, "crabclawsails4");
        _shipSailsDictionary.Add(ShipSails.GoldScaleSailsT1, "goldscalesails1");
        _shipSailsDictionary.Add(ShipSails.GoldScaleSailsT2, "goldscalesails2");
        _shipSailsDictionary.Add(ShipSails.GoldScaleSailsT3, "goldscalesails3");
        _shipSailsDictionary.Add(ShipSails.GoldScaleSailsT4, "goldscalesails4");
        _shipSailsDictionary.Add(ShipSails.SeaweedSailsT1, "seaweedsails1");
        _shipSailsDictionary.Add(ShipSails.SeaweedSailsT2, "seaweedsails2");
        _shipSailsDictionary.Add(ShipSails.SeaweedSailsT3, "seaweedsails3");
        _shipSailsDictionary.Add(ShipSails.SeaweedSailsT4, "seaweedsails4");
        _shipSailsDictionary.Add(ShipSails.FlowerPetalSailsT1, "flowerpetalsails1");
        _shipSailsDictionary.Add(ShipSails.FlowerPetalSailsT2, "flowerpetalsails2");
        _shipSailsDictionary.Add(ShipSails.FlowerPetalSailsT3, "flowerpetalsails3");
        _shipSailsDictionary.Add(ShipSails.FlowerPetalSailsT4, "flowerpetalsails4");

        //Set up hull dictionary
        _shipHullDictionary.Add(ShipHull.Default, "rudimentaryhull1");
        _shipHullDictionary.Add(ShipHull.RudimentaryHull, "rudimentaryhull1");
        _shipHullDictionary.Add(ShipHull.WoodenHullT1, "woodenhull1");
        _shipHullDictionary.Add(ShipHull.WoodenHullT2, "woodenhull2");
        _shipHullDictionary.Add(ShipHull.WoodenHullT3, "woodenhull3");
        _shipHullDictionary.Add(ShipHull.WoodenHullT4, "woodenhull4");
        _shipHullDictionary.Add(ShipHull.SteelHullT1, "steelhull1");
        _shipHullDictionary.Add(ShipHull.SteelHullT2, "steelhull2");
        _shipHullDictionary.Add(ShipHull.SteelHullT3, "steelhull3");
        _shipHullDictionary.Add(ShipHull.SteelHullT4, "steelhull4");
        _shipHullDictionary.Add(ShipHull.SeaglassHullT1, "seaglasshull1");
        _shipHullDictionary.Add(ShipHull.SeaglassHullT2, "seaglasshull2");
        _shipHullDictionary.Add(ShipHull.SeaglassHullT3, "seaglasshull3");
        _shipHullDictionary.Add(ShipHull.SeaglassHullT4, "seaglasshull4");
        _shipHullDictionary.Add(ShipHull.BoulderHullT1, "boulderhull1");
        _shipHullDictionary.Add(ShipHull.BoulderHullT2, "boulderhull2");
        _shipHullDictionary.Add(ShipHull.BoulderHullT3, "boulderhull3");
        _shipHullDictionary.Add(ShipHull.BoulderHullT4, "boulderhull4");
        _shipHullDictionary.Add(ShipHull.ClamShellHullT1, "clamshellhull1");
        _shipHullDictionary.Add(ShipHull.ClamShellHullT2, "clamshellhull2");
        _shipHullDictionary.Add(ShipHull.ClamShellHullT3, "clamshellhull3");
        _shipHullDictionary.Add(ShipHull.ClamShellHullT4, "clamshellhull4");
        _shipHullDictionary.Add(ShipHull.MonkeyHullT1, "monkeyhull1");
        _shipHullDictionary.Add(ShipHull.MonkeyHullT2, "monkeyhull2");
        _shipHullDictionary.Add(ShipHull.MonkeyHullT3, "monkeyhull3");
        _shipHullDictionary.Add(ShipHull.MonkeyHullT4, "monkeyhull4");

        //Set up cannon dictionary
        _shipCannonDictionary.Add(ShipCannon.Default, "rustedcannon1");
        _shipCannonDictionary.Add(ShipCannon.RustedCannon, "rustedcannon1");
        _shipCannonDictionary.Add(ShipCannon.CannonT1, "cannon1");
        _shipCannonDictionary.Add(ShipCannon.CannonT2, "cannon2");
        _shipCannonDictionary.Add(ShipCannon.CannonT3, "cannon3");
        _shipCannonDictionary.Add(ShipCannon.CannonT4, "cannon4");
        _shipCannonDictionary.Add(ShipCannon.SteelCannonT1, "steelcannon1");
        _shipCannonDictionary.Add(ShipCannon.SteelCannonT2, "steelcannon2");
        _shipCannonDictionary.Add(ShipCannon.SteelCannonT3, "steelcannon3");
        _shipCannonDictionary.Add(ShipCannon.SteelCannonT4, "steelcannon4");
        _shipCannonDictionary.Add(ShipCannon.PoisonCannonT1, "poisoncannon1");
        _shipCannonDictionary.Add(ShipCannon.PoisonCannonT2, "poisoncannon2");
        _shipCannonDictionary.Add(ShipCannon.PoisonCannonT3, "poisoncannon3");
        _shipCannonDictionary.Add(ShipCannon.PoisonCannonT4, "poisoncannon4");
        _shipCannonDictionary.Add(ShipCannon.FlameCannonT1, "flamecannon1");
        _shipCannonDictionary.Add(ShipCannon.FlameCannonT2, "flamecannon2");
        _shipCannonDictionary.Add(ShipCannon.FlameCannonT3, "flamecannon3");
        _shipCannonDictionary.Add(ShipCannon.FlameCannonT4, "flamecannon4");
        _shipCannonDictionary.Add(ShipCannon.FireworksCannonT1, "fireworkscannon1");
        _shipCannonDictionary.Add(ShipCannon.FireworksCannonT2, "fireworkscannon2");
        _shipCannonDictionary.Add(ShipCannon.FireworksCannonT3, "fireworkscannon3");
        _shipCannonDictionary.Add(ShipCannon.FireworksCannonT4, "fireworkscannon4");
        _shipCannonDictionary.Add(ShipCannon.CrabClawCannonT1, "crabclawcannon1");
        _shipCannonDictionary.Add(ShipCannon.CrabClawCannonT2, "crabclawcannon2");
        _shipCannonDictionary.Add(ShipCannon.CrabClawCannonT3, "crabclawcannon3");
        _shipCannonDictionary.Add(ShipCannon.CrabClawCannonT4, "crabclawcannon4");
        _shipCannonDictionary.Add(ShipCannon.BubbleBeamCannonT1, "bubblebeam1");
        _shipCannonDictionary.Add(ShipCannon.BubbleBeamCannonT2, "bubblebeam2");
        _shipCannonDictionary.Add(ShipCannon.BubbleBeamCannonT3, "bubblebeam3");
        _shipCannonDictionary.Add(ShipCannon.BubbleBeamCannonT4, "bubblebeam4");

        //Setup trinket dictonary
        _shipTrinketDictionary.Add(ShipTrinket.Default, "grandmascookies1");
        _shipTrinketDictionary.Add(ShipTrinket.GrandmasCookies, "grandmascookies1");
        _shipTrinketDictionary.Add(ShipTrinket.Lemonade, "lemonade1");
        _shipTrinketDictionary.Add(ShipTrinket.MonkeysCrown, "monkeyscrown1");
        _shipTrinketDictionary.Add(ShipTrinket.LuckyCarpChimes, "luckycarpchimes1");
        _shipTrinketDictionary.Add(ShipTrinket.PearlescentMedicine, "pearlescentmedicine1");
        _shipTrinketDictionary.Add(ShipTrinket.FishFeatherFan, "fishfeatherfan1");
        _shipTrinketDictionary.Add(ShipTrinket.LotusBlossom, "lotusblossom1");
        _shipTrinketDictionary.Add(ShipTrinket.Lemon, "lemon1");
    }

    private Dictionary<ShipBase, string> _shipBaseDictionary = new Dictionary<ShipBase, string>();
    private Dictionary<ShipSails, string> _shipSailsDictionary = new Dictionary<ShipSails, string>();
    private Dictionary<ShipHull, string> _shipHullDictionary = new Dictionary<ShipHull, string>();
    private Dictionary<ShipCannon, string> _shipCannonDictionary = new Dictionary<ShipCannon, string>();
    private Dictionary<ShipTrinket, string> _shipTrinketDictionary = new Dictionary<ShipTrinket, string>();
    [Space]
    [Header("MAKE SURE CUSTOM STARTING ITEMS ARE TURNED ON")]
    [Header("IN INVENTORY METHODS")]
    [SerializeField]
    private ShipBase _startingBase = ShipBase.Default;
    [SerializeField]
    private ShipSails _startingSails = ShipSails.Default;
    [SerializeField]
    private ShipHull _startingHull = ShipHull.Default;
    [SerializeField]
    private ShipCannon _startingCannon = ShipCannon.Default;
    [SerializeField]
    private ShipTrinket _startingTrinket = ShipTrinket.Default;

    /// <summary>
    /// Gets a list of current starting items using their slugs
    /// Used in inventory methods to set up starting items
    /// </summary>
    /// <returns></returns>
    public List<string> GetStartingItemList()
    {
        List<string> items = new List<string>();
        items.Add(_shipBaseDictionary[_startingBase]);
        items.Add(_shipSailsDictionary[_startingSails]);
        items.Add(_shipHullDictionary[_startingHull]);
        items.Add(_shipCannonDictionary[_startingCannon]);
        items.Add(_shipTrinketDictionary[_startingTrinket]);
        return items;
    }

    /// <summary>
    /// Gets a list of default starting items using their slugs
    /// Used in inventory methods to set up starting items
    /// </summary>
    /// <returns></returns>
    public List<string> GetDefaultStartingItemList()
    {
        List<string> items = new List<string>();
        items.Add(_shipBaseDictionary[ShipBase.Default]);
        items.Add(_shipSailsDictionary[ShipSails.Default]);
        items.Add(_shipHullDictionary[ShipHull.Default]);
        items.Add(_shipCannonDictionary[ShipCannon.Default]);
        items.Add(_shipTrinketDictionary[ShipTrinket.Default]);
        return items;
    }
}
