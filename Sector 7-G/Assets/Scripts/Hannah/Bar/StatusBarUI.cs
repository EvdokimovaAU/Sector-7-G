using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusBarUI : MonoBehaviour
{
    [Header("Segments")]
    [SerializeField] private RectTransform segmentsContainer;

    [Tooltip("—прайт одного маленького делени€")]
    [SerializeField] private Sprite segmentSprite;

    [SerializeField] private int segmentCount = 20;

    [SerializeField] private float spacing = 2f;


    [Header("Text")]
    [SerializeField] private TMP_Text valueText;


    private Image[] segments;


    private void Awake()
    {
        CreateSegments();
    }


    private void CreateSegments()
    {
        if (segmentsContainer == null)
        {
            Debug.LogError(
                "[StatusBarUI] Segments Container не назначен."
            );

            return;
        }

        if (segmentSprite == null)
        {
            Debug.LogError(
                "[StatusBarUI] Segment Sprite не назначен."
            );

            return;
        }


        // ”дал€ем старые созданные делени€.
        for (int i = segmentsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(
                segmentsContainer.GetChild(i).gameObject
            );
        }


        segments = new Image[segmentCount];


        // –азмер контейнера.
        float containerWidth =
            segmentsContainer.rect.width;

        float containerHeight =
            segmentsContainer.rect.height;


        // –ассчитываем ширину одного делени€.
        float totalSpacing =
            spacing * (segmentCount - 1);

        float segmentWidth =
            (containerWidth - totalSpacing)
            / segmentCount;


        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment =
                new GameObject(
                    "Segment_" + (i + 1),
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(Image)
                );


            segment.transform.SetParent(
                segmentsContainer,
                false
            );


            RectTransform rect =
                segment.GetComponent<RectTransform>();


            // якорь слева по центру.
            rect.anchorMin =
                new Vector2(0f, 0.5f);

            rect.anchorMax =
                new Vector2(0f, 0.5f);

            rect.pivot =
                new Vector2(0f, 0.5f);


            rect.sizeDelta =
                new Vector2(
                    segmentWidth,
                    containerHeight
                );


            rect.anchoredPosition =
                new Vector2(
                    i * (segmentWidth + spacing),
                    0f
                );


            Image image =
                segment.GetComponent<Image>();

            image.sprite = segmentSprite;

            image.type = Image.Type.Sliced;

            image.raycastTarget = false;


            segments[i] = image;
        }
    }


    public void SetValue(int value)
    {
        value = Mathf.Clamp(
            value,
            0,
            100
        );


        if (segments != null)
        {
            int activeCount =
                Mathf.CeilToInt(
                    value / 100f *
                    segments.Length
                );


            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].gameObject.SetActive(
                    i < activeCount
                );
            }
        }


        if (valueText != null)
        {
            valueText.text =
                value + "%";
        }
    }
}