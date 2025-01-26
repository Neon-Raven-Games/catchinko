using System;
using Overworld;
using UnityEngine;

[Serializable]
public class OverWorldMapData
{
    public CatBoss boss;
    public bool unlocked;
    public float percentComplete;

    public Transform backgroundSprite;

    // we will most likely use the first level, but edge cases for art, left decoupled
    public Transform initialCameraPosition;
    public float initialCameraOrthoSize;

    public OverWorldInnerLevel lastPlayerLevel;
    public OverWorldInnerLevel firstLevel;
}

[Serializable]
public class OverWorldInnerLevelData
{
    public int index;

    public OverWorldInnerLevel eastNeighbor;
    public OverWorldInnerLevel westNeighbor;
    public OverWorldInnerLevel northNeighbor;
    public OverWorldInnerLevel southNeighbor;

    public bool unlocked;
    public bool completed;
}