using UnityEngine;

public class JournalInteraction : MonoBehaviour
{
    [Header("Journal")]
    [SerializeField] private EmergencyJournal journal;

    private void OnMouseDown()
    {
        if (PhoneClick.IsPhoneOpen)
        {
            Debug.Log("[JOURNAL] Телефон открыт. Журнал заблокирован.");
            return;
        }

        if (journal == null)
        {
            Debug.LogError(
                "[JournalInteraction] EmergencyJournal не назначен!"
            );
            return;
        }

        journal.ToggleJournal();
    }
}