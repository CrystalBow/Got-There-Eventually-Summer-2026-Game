using UnityEngine;
using System.Collections.Generic;

public class GenerateEnemies : MonoBehaviour
{
    /// <summary>
    /// This loads a tutorial bug into an enemy list to allow
    /// us to create an encounter for the prototype.
    /// </summary>
    /// <returns></returns>
    static public Dictionary<string, UnitData> createTutorialEnemies()
    {
        Dictionary<string, UnitData> toReturn = new Dictionary<string, UnitData>();

        toReturn.Add("Tutorial Bug", DataCenter.Instance.Locations["Tutorial"]["Tutorial Bug"]);

        return toReturn;
    }
}
