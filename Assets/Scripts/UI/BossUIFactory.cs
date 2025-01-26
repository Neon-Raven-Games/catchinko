using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

[Serializable]
public class BossUIDataObject
{
    public CatBoss boss;
    public Sprite bossPortrait;
    public string bossName;
    public string bossDescription;
    public float percentComplete;
    public bool unlocked;
}

// Set all the boss data in the inspector in reverse order
// this will populate the scroll rect
public class BossUIFactory : MonoBehaviour
{
    /// <summary>
    /// Reversed list of boss ui data objects for easy editing boss data. Was going to use SO, but those suck.
    /// </summary>
    [Header("Reversed List Order")] public List<BossUIDataObject> bossUIDataObjects;

    /// <summary>
    /// The prefab that should have <see cref="BossUIView"/> attached to it.
    /// </summary>
    [SerializeField] private GameObject bossUIPrefab;
    
    /// <summary>
    /// Scroll rect parent to populate the boss UI from data.
    /// </summary>
    [SerializeField] private Transform bossUIParent;
    
    /// <summary>
    /// The animated menu to reduce references assigned in inspector. Needs a bit of refactoring.
    /// </summary>
    [SerializeField] private AnimatedMenu animatedMenu;
    
    /// <summary>
    /// Create and populate the boss UI from the data objects.
    /// </summary>
    public void Start()
    {
        foreach (var bossUIDataObject in bossUIDataObjects)
        {
            var bossUI = Instantiate(bossUIPrefab, bossUIParent);
            bossUI.name = bossUIDataObject.boss.ToString();
            bossUI.GetComponent<BossUIView>().Populate(bossUIDataObject, animatedMenu);
        }
    }
}