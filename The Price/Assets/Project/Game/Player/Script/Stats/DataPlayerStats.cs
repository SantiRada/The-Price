using TMPro;
using UnityEngine;

public class DataPlayerStats : MonoBehaviour {

    [Header("Stats")]
    private bool _inStats { get; set; }
    [Space]
    public TextMeshProUGUI _textHealth;
    public TextMeshProUGUI _textEnergy;
    public TextMeshProUGUI _textSpeed;
    public TextMeshProUGUI _textDamage;
    public TextMeshProUGUI _textDamageSkills;
    public TextMeshProUGUI _textCritical;
    [Space]
    public TextMeshProUGUI _textTitleData;
    public TextMeshProUGUI _textDescriptionData;

    public bool InStats{
        get { return _inStats; }
        set { _inStats = value; }
    }
}
