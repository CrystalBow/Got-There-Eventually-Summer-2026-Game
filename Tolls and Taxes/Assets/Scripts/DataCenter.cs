using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// A massive read only database that houses all our game data
/// </summary>
/// <remarks>
/// It is a singleton that can accessed anywhere in the game using DataCenter.Instance
/// </remarks>
public class DataCenter : MonoBehaviour
{
    public static DataCenter Instance { get; private set; }
    private GameDataWrapper _data;
    private Dictionary<int, string> _idToEffectName;

    // These are the read-only data points that are available to the rest of the code.
    public IReadOnlyDictionary<string, PlayableData> Allies => _data.Ally;
    public IReadOnlyDictionary<string, Dictionary<string, UnitData>> Locations => _data.Location;
    public IReadOnlyDictionary<string, int> Effects => _data.Effects;
    public IReadOnlyCardBook GlobalCards => _data.Cards;

    private void Awake()
    {
        //Ensuring we only have one instance of this Database
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Opens the GameData.json and then plugs it in to our database.
    /// </summary>
    /// <remarks>
    /// Also makes an effect map for easy effect searching. The _data is ported to a bunch of container objects, so
    /// that Newtonsoft can then unpack the json format with ease.
    /// </remarks>
    private void LoadGameData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("GameData");
        if (jsonFile != null)
        {
            _data = JsonConvert.DeserializeObject<GameDataWrapper>(jsonFile.text);
            
            // Generate the reverse map immediately after loading using the updated PascalCase property
            _idToEffectName = _data.Effects.ToDictionary(pair => pair.Value, pair => pair.Key);
            
            Debug.Log("Game Data, Mappings, and Card Libraries Loaded Successfully.");
        }
    }
    
    // 2 Useful helper functions for the effect map.
    public string GetEffectName(int id)
    {
        if (_idToEffectName.TryGetValue(id, out string effectName))
        {
            return effectName;
        }
        return "Unknown";
    }

    public List<string> GetEffectNamesForCard(List<int> ids)
    {
        List<string> names = new List<string>();
        foreach (int id in ids)
        {
            names.Add(GetEffectName(id));
        }
        return names;
    }
    
    // This ensures that whenever the game is run, this object will be instantiated.
    // This way we don't need to cluttered our scenes with the game object every single scene.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeScene()
    {
        GameObject g = new GameObject("Generated_DataCenter");
        g.AddComponent<DataCenter>();
    }
    
}