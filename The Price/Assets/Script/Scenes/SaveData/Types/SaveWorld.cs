public enum ReasonSave { Null, closeGame, deadSystem }
[System.Serializable]
public class SaveWorld
{
    public int positionGame = 0;
    // Tutorial system removed
    public ReasonSave reasonSave;
    public int currentWorld = 0;

    public int wasInTerrenal;
    public int wasInCielo;
    public int wasInInfierno;
    public int wasInAstral;
    public int wasInInframundo;

    public int deadInTerrenal;
    public int deadInCielo;
    public int deadInInfierno;
    public int deadInInframundo;

    public int killedMBoss_Terrenal;
    public int killedMBoss_Cielo;
    public int killedMBoss_Infierno;
    public int killedMBoss_Inframundo;

    public int killedBoss_Terrenal;
    public int killedBoss_Cielo;
    public int killedBoss_Infierno;

    public bool killedThanatos = false;
}
