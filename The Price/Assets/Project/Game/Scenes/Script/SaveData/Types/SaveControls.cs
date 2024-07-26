[System.Serializable]
public class SaveControls {

    // PLAYER
    public int moveUp;
    public int moveDown;
    public int moveLeft;
    public int moveRight;

    public int use;
    public int dash;

    public int staticAim;
    public int aimUp;
    public int aimDown;
    public int aimLeft;
    public int aimRight;

    public int stats;
    public int pause;

    public int attackOne;
    public int attackTwo;
    public int attackThree;

    public int skillOne;
    public int skillTwo;
    public int skillThree;

    // UI-CONTENT
    public int back;
    public int select;
    public int resetValues;
    public int otherFunction;

    public int leftUI;
    public int rightUI;

    public int GetValueControl(string name)
    {
        int value = 0;

        switch (name)
        {
            case "Move": value = moveUp; break;
            case "MoveUp": value = moveUp; break;
            case "MoveDown": value = moveDown; break;
            case "MoveLeft": value = moveLeft; break;
            case "MoveRight": value = moveRight; break;

            case "Use": value = use; break;
            case "Dash": value = dash; break;

            case "StaticAim": value = staticAim; break;
            case "Aim": value = aimUp; break;
            case "AimUp": value = aimUp; break;
            case "AimDown": value = aimDown; break;
            case "AimLeft": value = aimLeft; break;
            case "aimRight": value = aimRight; break;

            case "Stats": value = stats; break;
            case "Pause": value = pause; break;

            case "AttackOne": value = attackOne; break;
            case "AttackTwo": value = attackTwo; break;
            case "AttackThree": value = attackThree; break;

            case "SkillOne": value = skillOne; break;
            case "SkillTwo": value = skillTwo; break;
            case "SkillThree": value = skillThree; break;

            case "Back": value = back; break;
            case "Select": value = select; break;
            case "ResetValues": value = resetValues; break;
            case "OtherFunction": value = otherFunction; break;

            case "LeftUI": value = leftUI; break;
            case "RightUI": value = rightUI; break;
        }

        return value;
    }
}
