using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class BossUIView : MonoBehaviour
    {
        [SerializeField] private CatBoss boss;
        [SerializeField] private BossMouseEventHandler mouseEventHandler;
        [SerializeField] private Image bossPortrait;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI percentCompleteText;
        [SerializeField] private GameObject unlockObject;
        
        public void Populate(BossUIDataObject obj, AnimatedMenu menu)
        {
            boss = obj.boss;
            bossPortrait.sprite = obj.bossPortrait;
            nameText.text = obj.bossName;
            descriptionText.text = obj.bossDescription;
            percentCompleteText.text = obj.percentComplete.ToString();
            if (unlockObject) unlockObject.SetActive(!obj.unlocked);
            mouseEventHandler.Initialize(boss, menu);
            mouseEventHandler.enabled = obj.unlocked;
        }
    }