using UnityEngine;

[CreateAssetMenu(fileName = "NewObject", menuName = "Object/Canceled Object")]
public class CanceledObject : Object {

    private float[] amount = new float[11];
    private float[] amountMax = new float[11];

    [Header("Object Per Time")]
    public bool isActive = false;
    public float timer = 3f;
    private float baseTimer = 3f;

    private void OnEnable()
    {
        baseTimer = timer;
    }
    public void Timer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = baseTimer;
            CancelContent();
        }
    }
    public override void Use()
    {
        if (!canActive) return;

        if(hasCooling) canActive = false;

        // FUNCIONAMIENTO DEL TIMER
        isActive = true;
        timer = baseTimer;
        // ------------------------

        for (int i = 0; i < statsModifiable.Length; i++)
        {
            amount[i] += statsModifiable[i];
            playerStats.SetValue(i, statsModifiable[i], false);
        }
        for (int i = 0; i < statsMaxModifiable.Length; i++)
        {
            amountMax[i] += statsMaxModifiable[i];
            playerStats.SetValue(i, statsMaxModifiable[i], true);
        }
    }
    public override void CancelContent()
    {
        for (int i = 0; i < statsModifiable.Length; i++) { playerStats.SetValue(i, -amount[i], false); }
        for (int i = 0; i < statsMaxModifiable.Length; i++) { playerStats.SetValue(i, -amountMax[i], true); }

        isActive = false;
    }
}
