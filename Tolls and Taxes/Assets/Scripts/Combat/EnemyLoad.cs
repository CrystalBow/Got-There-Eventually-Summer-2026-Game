using System;
using UnityEngine;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine.U2D;

public class EnemyLoader : MonoBehaviour
{
    public bool combatReady = false;
    private GameObject playablePrefab;
    private GameObject enemyPrefab;
    private Vector3 PlayerOffset;
    private Vector3 EnemeyOffset;
    public void Awake()
    {
        combatReady = false;
        enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
        playablePrefab = Resources.Load<GameObject>("Prefabs/Playable");
    }


    public void SpawnUnits()
    {
        int multiplierPlayer = 2;
        int multiplierFoe = 2;
        int multiplierIncrement = 2;
        PlayerOffset = Vector3.left * multiplierPlayer;
        EnemeyOffset = Vector3.right * multiplierFoe;
        if (TransferCenter.Instance.foeQueue.Count == 0)
        {
            TransferCenter.Instance.foeQueue.Add("Tutorial Bug");
        }
        
        foreach (string item in TransferCenter.Instance.foeQueue)
        {
            GameObject current = Instantiate(enemyPrefab, this.transform);
            current.GetComponent<Combatant>().CombatantName =  item;
            current.transform.localPosition = EnemeyOffset;
            multiplierFoe += multiplierIncrement;
            EnemeyOffset = Vector3.right * multiplierFoe;
        }

        foreach (string item in TransferCenter.Instance.PartyOrder)
        {
            GameObject current = Instantiate(playablePrefab, this.transform);
            current.GetComponent<Combatant>().CombatantName = item;
            current.transform.localPosition = PlayerOffset;
            multiplierPlayer += multiplierIncrement;
            PlayerOffset = Vector3.left * multiplierPlayer;
        }
        combatReady = true;
    }
}
