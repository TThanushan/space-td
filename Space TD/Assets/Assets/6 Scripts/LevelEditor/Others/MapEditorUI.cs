using UnityEngine;
using System.Collections;
using TMPro;

public class MapEditorUI : MonoBehaviour
{
    public static MapEditorUI instance;
    public GameObject erasingGraphicPrefab;
    public GameObject deleteEffect;
    public GameObject PlacingEffect;
    public GameObject infoTextCanvas;
    public Animation infoTextCanvasAnimation;

    private GameObject erasingGraphic;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Debug.Log("There is already a MapEditorUI script");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (KeyPressManager.instance.mouse1KeyReady)
        {
            MapEditor.instance.ShowPrefabPreview(false);
            if (!erasingGraphic)
                erasingGraphic = PoolObject.instance.GetPoolObject(erasingGraphicPrefab);
            if (!erasingGraphic.activeSelf)
                ShowErasingGraphic(true);
            erasingGraphic.transform.position = MapEditor.instance.cursor.transform.position;
        }
        else if (erasingGraphic)
        {
            MapEditor.instance.ShowPrefabPreview(true);
            ShowErasingGraphic(false);
        }
    }

    public void CreateDeleteEffect(Vector2 _position)
    {
        GameObject effect = PoolObject.instance.GetPoolObject(deleteEffect);
        effect.transform.position = _position;
    }

    public void CreatePlacingEffect(Vector2 _position)
    {
        GameObject effect = PoolObject.instance.GetPoolObject(PlacingEffect);
        effect.transform.position = _position;
    }

    private void ShowErasingGraphic(bool value)
    {
        erasingGraphic.SetActive(value);
    }

    public void ShowInfoText(string _text)
    {
        TextMeshProUGUI _textMeshProUGUI = infoTextCanvas.GetComponentInChildren<TextMeshProUGUI>();
        _textMeshProUGUI.text = _text;
        _textMeshProUGUI.color = Color.white;
        infoTextCanvasAnimation.Play();
    }

    public void ShowInfoText(string _text, Color _color)
    {
        TextMeshProUGUI _textMeshProUGUI = infoTextCanvas.GetComponentInChildren<TextMeshProUGUI>();
        _textMeshProUGUI.text = _text;
        _textMeshProUGUI.color = _color;
        infoTextCanvasAnimation.Play();
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        Debug.Log("open panel");
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        Debug.Log("close panel");
    }
}
