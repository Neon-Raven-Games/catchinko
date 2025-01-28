using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoalShuffler : MonoBehaviour
{
    [SerializeField] private GameObject missPrefab;
    [SerializeField] private GameObject hitPrefab;
    [SerializeField] private GameObject diePrefab;

    [Tooltip("x = hit, y = miss, z = die")]
    public Vector3 goalWeights;

    [SerializeField] private List<Transform> goalZonePositions;
    private readonly List<GoalZone> _goalZones = new();

    private void ShuffleGoals()
    {
        for (var i = 0; i < goalZonePositions.Count; i++)
        {
            var randomValue = Random.Range(0f, goalWeights.x + goalWeights.y + goalWeights.z);
            var pos = new Vector3(goalZonePositions[i].position.x, goalZonePositions[i].position.y, 0);
            
            if (randomValue < goalWeights.x) _goalZones.Add(Instantiate(hitPrefab, pos, Quaternion.identity).GetComponent<GoalZone>());
            else if (randomValue < goalWeights.x + goalWeights.y) _goalZones.Add(Instantiate(diePrefab, pos, Quaternion.identity).GetComponent<GoalZone>());
            else _goalZones.Add(Instantiate(missPrefab, pos, Quaternion.identity).GetComponent<GoalZone>());
        }
    }

    public void SpawnGoalsWeighted()
    {
        for (var i = 0; i < _goalZones.Count; i++) Destroy(_goalZones[i].gameObject);
        _goalZones.Clear();
        ShuffleGoals();
    }

    private void Start() => SpawnGoalsWeighted();
}