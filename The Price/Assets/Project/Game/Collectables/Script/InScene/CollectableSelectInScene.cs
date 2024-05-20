using TMPro;

public class CollectableSelectInScene : CollectableInScene {

    private CollectableSelectable _collect;
    private TextMeshProUGUI _textContent;

    public override void InitialValues()
    {
        _collect = FindAnyObjectByType<CollectableSelectable>();
    }
    public override void LoadData()
    {
        _textContent = _windowAppear.GetComponentInChildren<TextMeshProUGUI>();

        switch (_typeElement)
        {
            case TypeElement.Skills: _textContent.text = LanguageManager.GetValue(106); break;
            case TypeElement.Aptitudes: _textContent.text = LanguageManager.GetValue(107); break;
            case TypeElement.Objects: _textContent.text = LanguageManager.GetValue(108); break;
        }
    }
    public override void Select()
    {
        _collect.gameObject.SetActive(true);
        _collect.RandomValues(3);
    }
}
