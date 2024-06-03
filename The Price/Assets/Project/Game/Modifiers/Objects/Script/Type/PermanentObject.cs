using UnityEngine;

public class PermanentObject : Object {

    public override void Use()
    {
        bool canUse = false;
        for(int i = 0; i < statsModifiable.Length; i++)
        {
            if (statsModifiable[i] != 0)
            {
                canUse = true;
                position = i;
                value = statsModifiable[i];

                break;
            }
        }
        if (!canUse)
        {
            for (int i = 0; i < statsMaxModifiable.Length; i++)
            {
                if (statsMaxModifiable[i] != 0)
                {
                    canUse = true;
                    position = i;
                    value = statsMaxModifiable[i];

                    break;
                }
            }
        }

        if (!canUse) Debug.Log("No se modificó ninguna estadística con este objeto...");
    }
}
