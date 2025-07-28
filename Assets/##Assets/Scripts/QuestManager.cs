using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestData
    {
        public string questID;
        public string title;
        public string description;
        public int rewardGold;
        public bool isPlaceholder;

        public int targetCount;     // Gereken öldürme sayýsý
        public int currentCount;    // Oyuncunun ilerlemesi
        public bool isCompleted => currentCount >= targetCount;
    }

    public List<QuestData> quests = new List<QuestData>();

    [Header("UI")]
    public Transform questListContainer;       // ScrollView içindeki Content objesi
    public GameObject questButtonPrefab;       // Buton prefabý
    public TMP_Text titleText;                 // Sað panel baþlýk
    public TMP_Text descriptionText;           // Sað panel açýklama
    public TMP_Text rewardText;                // Sað panel ödül
    public Button acceptButton;                // Kabul et tuþu

    private QuestData selectedQuest;

    void Start()
    {
        CreateQuests();
        PopulateQuestList();
    }


    void CreateQuests()
    {
        // Gerçek görev
        quests.Add(new QuestData
        {
            questID = "001",
            title = "Orman Kurtlarýný Yok Et",
            description = "Yakýndaki ormanda 3 kurdu yok et.",
            rewardGold = 100,
            isPlaceholder = false
        });

        // Placeholder görevler
        for (int i = 0; i < 5; i++)
        {
            quests.Add(new QuestData
            {
                questID = $"placeholder_{i}",
                title = "Görev Henüz Eklenmedi",
                description = "Bu görev henüz hazýrlanmadý.",
                rewardGold = 0,
                isPlaceholder = true
            });
        }
    }

    void PopulateQuestList()
    {
        foreach (var quest in quests)
        {
            GameObject buttonObj = Instantiate(questButtonPrefab, questListContainer);
            buttonObj.GetComponentInChildren<TMP_Text>().text = quest.title;

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectQuest(quest);
            });
        }
    }

    void SelectQuest(QuestData quest)
    {
        selectedQuest = quest;

        titleText.text = quest.title;
        descriptionText.text = quest.description;
        rewardText.text = $"Ödül: {quest.rewardGold} altýn";

        acceptButton.interactable = !quest.isPlaceholder;

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() => AcceptQuest(quest));
    }

    void AcceptQuest(QuestData quest)
    {
        Debug.Log($"Görev kabul edildi: {quest.title}");
        ActiveQuestTracker.instance.SetActiveQuest(quest);
    }

}
