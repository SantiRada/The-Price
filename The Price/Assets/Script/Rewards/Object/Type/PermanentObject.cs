using UnityEngine;

[CreateAssetMenu(fileName = "NewObject", menuName = "Object/Permanent Object")]
public class PermanentObject : Object {

    public override void Use()
    {
        if (!canActive) return;

        if (hasCooling) canActive = false;

        bool canUse = false;
        for(int i = 0; i < statsModifiable.Length; i++)
        {
            if (statsModifiable[i] != 0)
            {
                canUse = true;
                playerStats.SetValue(i, statsModifiable[i], false);
            }
        }
        if (!canUse)
        {
            for (int i = 0; i < statsMaxModifiable.Length; i++)
            {
                if (statsMaxModifiable[i] != 0)
                {
                    canUse = true;
                    playerStats.SetValue(i, statsMaxModifiable[i], true);
                }
            }
        }

        if (!canUse) Debug.Log("No se modificó ninguna estadística con este objeto...");
    }
    public override void CancelContent() { Debug.Log("Este objeto no tiene cancelamiento."); }
}
