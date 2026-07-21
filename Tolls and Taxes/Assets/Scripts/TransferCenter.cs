using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton for storing transfer data.
/// </summary>
public class TransferCenter : MonoBehaviour
{
    public static TransferCenter Instance { get; private set; }
    
    [System.Serializable]
    public class CharacterSessionData
    {
        public string CharacterName;
        public Deck Deck;
        public int CurrentHp;
        public int CurrentMp;
    }
    
    private readonly Dictionary<string, CharacterSessionData> _partySessionData = new Dictionary<string, CharacterSessionData>();
    private readonly List<string> _partyOrder = new List<string>();
    
    public IReadOnlyList<string> PartyOrder => _partyOrder;
    
    private void Awake()
    {
        //Ensuring we only have one instance of this Database
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Saves or updates a character's complete exploration/combat state.
    /// </summary>
    public void SaveCharacterState(string characterName, Deck currentDeck, int hp, int mp)
    {
        if (!_partySessionData.ContainsKey(characterName))
        {
            _partyOrder.Add(characterName);
        }

        _partySessionData[characterName] = new CharacterSessionData
        {
            CharacterName = characterName,
            Deck = currentDeck,
            CurrentHp = hp,
            CurrentMp = mp
        };
        Debug.Log("Saved Character: " + characterName);
    }
    
    /// <summary>
    /// Retrieves the vaulted session data for a character. Returns null if none exists.
    /// </summary>
    public CharacterSessionData GetCharacterState(string characterId)
    {
        _partySessionData.TryGetValue(characterId, out var data);
        return data;
    }

    /// <summary>
    /// Clears session data, for returning to main menu or starting a new game.
    /// </summary>
    public void ClearSession()
    {
        _partySessionData.Clear();
        _partyOrder.Clear();
    }
    
    // This ensures that whenever the game is run, this object will be instantiated.
    // This way we don't need to cluttered our scenes with the game object every single scene.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeScene()
    {
        GameObject g = new GameObject("Generated_TransferCenter");
        g.AddComponent<TransferCenter>();
    }
}
