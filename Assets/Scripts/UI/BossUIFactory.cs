using System;
using System.Collections.Generic;
using System.Linq;
using Overworld;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

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
private bool _initialized;
    /// <summary>
    /// Create and populate the boss UI from the data objects.
    /// </summary>
    public void Start()
    {
        foreach (var bossUIDataObject in bossUIDataObjects)
        {
            var bossUI = Instantiate(bossUIPrefab, bossUIParent);
            bossUI.name = bossUIDataObject.boss.ToString();
            var boss = OverWorldGameManager.GetInnerLevels(bossUIDataObject.boss);
            var percent = boss.Count(x => x.data.completed) / boss.Count * 100;
            bossUIDataObject.percentComplete = percent;
            bossUI.GetComponent<BossUIView>().Populate(bossUIDataObject, animatedMenu);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex != 0) return;
        foreach (Transform child in bossUIParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var bossUIDataObject in bossUIDataObjects)
        {
            var bossUI = Instantiate(bossUIPrefab, bossUIParent);
            bossUI.name = bossUIDataObject.boss.ToString();
            var boss = OverWorldGameManager.GetInnerLevels(bossUIDataObject.boss);
        
            var totalBosses = boss.Count;
            var completedBosses = boss.Count(x => x.data.completed);
            var percent = (totalBosses > 0) ? ((float)completedBosses / totalBosses) * 100f : 0f;
        
            bossUIDataObject.percentComplete = percent;
            bossUI.GetComponent<BossUIView>().Populate(bossUIDataObject, animatedMenu);
        }
    }

}