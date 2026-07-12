using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

/*
This file is companion for the data center it contains a bunch of data containers that match the format of our
GameData.json so we can easily use Newtonsoft.json to unpack it. We've also used init;, an IReadOnly interfaces, and
private variables to ensure that there is no accidental editing of this database during runtime.
*/

// Unity compatibility error work around for init;
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit {}
}

/// <summary>
/// A genric class for enemy data and the parent of PlayableData
/// </summary>
public class UnitData 
{
    public int Hp { get; init; }
    public int Attack { get; init; }
    public int Defense { get; init; }
    public int Speed { get; init; }
}

/// <summary>
/// A subclass that adds the data specific to the playable characters.
/// </summary>
public class PlayableData : UnitData
{
    public int Mp { get; init; }
    public float HpScale { get; init; }
    public float MpScale { get; init; }
    public float AttackScale { get; init; }
    public float DefenseScale { get; init; }
    public float SpeedScale { get; init; }

    /// <summary>
    /// Private CardBook exposed via the IReadOnly
    /// </summary>
    [JsonProperty("SpecialtyCards")]
    private CardBook SpecialtyCardsInternal { get; set; }
    
    [JsonIgnore]
    public IReadOnlyCardBook SpecialtyCards => SpecialtyCardsInternal;
}

/// <summary>
/// An interface to make our CardBook class read only.
/// Because it's layout didn't match a collection, we had to make this IReadOnly interface to protect the data.
/// </summary>
public interface IReadOnlyCardBook
{
    IReadOnlyDictionary<string, CardData> Attacks { get; }
    IReadOnlyDictionary<string, CardData> Buffs { get; }
    IReadOnlyDictionary<string, CardData> Healing { get; }
    IReadOnlyDictionary<string, CardData> Debuffs { get; }
}

/// <summary>
/// The CardBook class that implements the IReadOnlyCardBook version of it.
/// This Wrapping makes fully read only.
/// </summary>
public class CardBook : IReadOnlyCardBook
{
    public Dictionary<string, CardData> Attacks { get; set; }
    public Dictionary<string, CardData> Buffs { get; set; }
    public Dictionary<string, CardData> Healing { get; set; }
    public Dictionary<string, CardData> Debuffs { get; set; }
    
    // Read only wrappings for data protection.
    IReadOnlyDictionary<string, CardData> IReadOnlyCardBook.Attacks => Attacks;
    IReadOnlyDictionary<string, CardData> IReadOnlyCardBook.Buffs => Buffs;
    IReadOnlyDictionary<string, CardData> IReadOnlyCardBook.Healing => Healing;
    IReadOnlyDictionary<string, CardData> IReadOnlyCardBook.Debuffs => Debuffs;
}

/// <summary>
/// A read only container for card stats
/// </summary>
public class CardData
{
    public string Description { get; init; }
    public int Damage { get; init; }
    public int Time { get; init; }
    public int Cost { get; init; }
    public List<int> Effects { get; init; }
}

/// <summary>
/// This wrapper brings all the pieces together so Newtonsoft.json can easily unpack our data.
/// </summary>
public class GameDataWrapper 
{
    public Dictionary<string, PlayableData> Ally { get; set; }
    public Dictionary<string, Dictionary<string, UnitData>> Location { get; set; }
    public CardBook Cards { get; set; }
    public Dictionary<string, int> Effects { get; set; }
}
