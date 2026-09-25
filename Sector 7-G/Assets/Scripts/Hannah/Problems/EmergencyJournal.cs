using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmergencyJournal : MonoBehaviour
{
    // =========================================================
    // BOOK
    // =========================================================

    [Header("Book")]

    [SerializeField]
    private GameObject journalWindow;


    // =========================================================
    // LEFT PAGE
    // =========================================================

    [Header("Left Page")]

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text descriptionText;

    [SerializeField]
    private TMP_Text sectorText;

    [SerializeField]
    private TMP_Text phoneText;


    // =========================================================
    // RIGHT PAGE
    // =========================================================

    [Header("Right Page")]

    [Tooltip(
        "Готовые строки шагов из Hierarchy: " +
        "Step1, Step2, Step3, Step4."
    )]
    [SerializeField]
    private List<EmergencyJournalStepUI> stepItems = new();


    // =========================================================
    // NAVIGATION
    // =========================================================

    [Header("Navigation")]

    [SerializeField]
    private Button previousButton;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private TMP_Text pageNumberText;


    // =========================================================
    // JOURNAL PAGES
    // =========================================================

    [Header("Journal Pages")]

    [SerializeField]
    private List<EmergencyJournalPage> pages = new();


    // =========================================================
    // CURRENT EMERGENCY
    // =========================================================

    [Header("Current Emergency")]

    [SerializeField]
    private TaskTriggeredEmergency emergency;


    // =========================================================
    // RUNTIME
    // =========================================================

    private int currentPageIndex = 0;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (previousButton != null)
        {
            previousButton.onClick.AddListener(
                PreviousPage
            );
        }


        if (nextButton != null)
        {
            nextButton.onClick.AddListener(
                NextPage
            );
        }
    }


    private void Start()
    {
        currentPageIndex = 0;

        RefreshPage();


        // При запуске игры книга закрыта.
        if (journalWindow != null)
        {
            journalWindow.SetActive(false);
        }
    }


    private void OnEnable()
    {
        SubscribeToEmergency();
    }


    private void OnDisable()
    {
        UnsubscribeFromEmergency();
    }


    private void OnDestroy()
    {
        if (previousButton != null)
        {
            previousButton.onClick.RemoveListener(
                PreviousPage
            );
        }


        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(
                NextPage
            );
        }
    }


    // =========================================================
    // EMERGENCY EVENTS
    // =========================================================

    private void SubscribeToEmergency()
    {
        if (emergency == null)
            return;


        emergency.OnEmergencyStarted +=
            HandleEmergencyChanged;

        emergency.OnEmergencyProgressChanged +=
            HandleEmergencyChanged;

        emergency.OnEmergencyResolved +=
            HandleEmergencyChanged;
    }


    private void UnsubscribeFromEmergency()
    {
        if (emergency == null)
            return;


        emergency.OnEmergencyStarted -=
            HandleEmergencyChanged;

        emergency.OnEmergencyProgressChanged -=
            HandleEmergencyChanged;

        emergency.OnEmergencyResolved -=
            HandleEmergencyChanged;
    }


    private void HandleEmergencyChanged()
    {
        RefreshPage();
    }


    // =========================================================
    // OPEN / CLOSE
    // =========================================================

    public void OpenJournal()
    {
        if (journalWindow == null)
        {
            Debug.LogError(
                "[EmergencyJournal] Journal Window не назначен."
            );

            return;
        }


        if (UIWindowManager.Instance != null)
        {
            UIWindowManager.Instance.OpenWindow(
                journalWindow
            );
        }
        else
        {
            journalWindow.SetActive(true);
        }


        RefreshPage();
    }


    public void CloseJournal()
    {
        if (journalWindow == null)
            return;


        if (UIWindowManager.Instance != null)
        {
            UIWindowManager.Instance.CloseWindow(
                journalWindow
            );
        }
        else
        {
            journalWindow.SetActive(false);
        }
    }


    public void ToggleJournal()
    {
        if (journalWindow == null)
        {
            Debug.LogError(
                "[EmergencyJournal] Journal Window не назначен."
            );

            return;
        }


        bool shouldOpen =
            !journalWindow.activeSelf;


        journalWindow.SetActive(
            shouldOpen
        );


        if (shouldOpen)
        {
            RefreshPage();
        }
    }


    // =========================================================
    // NAVIGATION
    // =========================================================

    public void NextPage()
    {
        if (pages == null ||
            pages.Count == 0)
        {
            return;
        }


        currentPageIndex++;


        if (currentPageIndex >= pages.Count)
        {
            currentPageIndex = 0;
        }


        RefreshPage();
    }


    public void PreviousPage()
    {
        if (pages == null ||
            pages.Count == 0)
        {
            return;
        }


        currentPageIndex--;


        if (currentPageIndex < 0)
        {
            currentPageIndex =
                pages.Count - 1;
        }


        RefreshPage();
    }


    // =========================================================
    // REFRESH PAGE
    // =========================================================

    private void RefreshPage()
    {
        if (pages == null ||
            pages.Count == 0)
        {
            ClearPage();
            return;
        }


        if (currentPageIndex < 0 ||
            currentPageIndex >= pages.Count)
        {
            currentPageIndex = 0;
        }


        EmergencyJournalPage page =
            pages[currentPageIndex];


        // -------------------------
        // TITLE
        // -------------------------

        if (titleText != null)
        {
            titleText.text =
                page.Title;
        }


        // -------------------------
        // DESCRIPTION
        // -------------------------

        if (descriptionText != null)
        {
            descriptionText.text =
                page.Description;
        }


        // -------------------------
        // SECTOR
        // -------------------------

        if (sectorText != null)
        {
            sectorText.text =
                "Цех: " +
                page.Sector.ToString();
        }


        // -------------------------
        // PHONE
        // -------------------------

        if (phoneText != null)
        {
            phoneText.text =
                "Телефон: " +
                page.PhoneNumber;
        }


        // -------------------------
        // STEPS
        // -------------------------

        RefreshSteps(page);


        // -------------------------
        // PAGE NUMBER
        // -------------------------

        if (pageNumberText != null)
        {
            pageNumberText.text =
                $"{currentPageIndex + 1} / {pages.Count}";
        }


        // -------------------------
        // NAVIGATION BUTTONS
        // -------------------------

        bool hasSeveralPages =
            pages.Count > 1;


        if (previousButton != null)
        {
            previousButton.interactable =
                hasSeveralPages;
        }


        if (nextButton != null)
        {
            nextButton.interactable =
                hasSeveralPages;
        }
    }


    // =========================================================
    // REFRESH STEPS
    // =========================================================

    private void RefreshSteps(
        EmergencyJournalPage page)
    {
        if (stepItems == null)
            return;


        // Квадраты/галочки должны появляться
        // только если:
        //
        // 1. Сейчас вообще есть активная авария.
        // 2. Открытая страница относится именно
        //    к этой аварии.
        bool isCurrentEmergency =
            emergency != null &&
            emergency.IsActive &&
            page.EmergencyID ==
            emergency.EmergencyID;


        for (int i = 0;
             i < stepItems.Count;
             i++)
        {
            EmergencyJournalStepUI item =
                stepItems[i];


            if (item == null)
                continue;


            // Если на странице меньше шагов,
            // чем создано UI-строк,
            // лишние строки полностью скрываем.
            if (page.Steps == null ||
                i >= page.Steps.Count)
            {
                item.gameObject.SetActive(false);
                continue;
            }


            // Строка инструкции видна всегда.
            item.gameObject.SetActive(true);


            bool completed =
                IsStepCompleted(
                    page,
                    i
                );


            // Записываем текст и выбираем
            // пустой квадрат / галочку.
            item.Setup(
                page.Steps[i],
                completed
            );


            // А сам квадратик показываем
            // только у текущей аварии.
            item.SetCheckmarkVisible(
                isCurrentEmergency
            );
        }
    }


    // =========================================================
    // CHECK COMPLETED STEP
    // =========================================================

    private bool IsStepCompleted(
        EmergencyJournalPage page,
        int stepIndex)
    {
        if (emergency == null)
            return false;


        // Нет активной аварии.
        if (!emergency.IsActive)
            return false;


        // Открыта страница другой аварии.
        if (page.EmergencyID !=
            emergency.EmergencyID)
        {
            return false;
        }


        // Например:
        //
        // CompletedStepCount = 2
        //
        // Step 0 = выполнен
        // Step 1 = выполнен
        // Step 2 = не выполнен
        return stepIndex <
               emergency.CompletedStepCount;
    }


    // =========================================================
    // CLEAR PAGE
    // =========================================================

    private void ClearPage()
    {
        if (titleText != null)
        {
            titleText.text = "";
        }


        if (descriptionText != null)
        {
            descriptionText.text = "";
        }


        if (sectorText != null)
        {
            sectorText.text = "";
        }


        if (phoneText != null)
        {
            phoneText.text = "";
        }


        if (pageNumberText != null)
        {
            pageNumberText.text = "";
        }


        if (stepItems == null)
            return;


        foreach (
            EmergencyJournalStepUI item
            in stepItems)
        {
            if (item != null)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}