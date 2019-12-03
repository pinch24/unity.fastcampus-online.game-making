﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawControl : MonoBehaviour
{
    public GameObject SpawnMonster = null;
    public List<GameObject> MonsterList = new List<GameObject>();
    public int SpawnMaxCount = 50;
    
    void Start()
    {
        InvokeRepeating("Spawn", 3.0f, 5.0f);
    }
    
    void Spawn()
    {
        if (MonsterList.Count > SpawnMaxCount)
        {
            return;
        }
        
        Vector3 spawnPos = new Vector3(Random.Range(-100.0f, 100.0f), 1000.0f, Random.Range(-100.0f, 100.0f));
        Ray ray = new Ray(spawnPos, Vector3.down);
        RaycastHit raycastHit = new RaycastHit();
        
        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
        {
            spawnPos.y = raycastHit.point.y;
        }
        
        GameObject newMonster = Instantiate(SpawnMonster, spawnPos, Quaternion.identity);
        MonsterList.Add(newMonster);
    }
}
