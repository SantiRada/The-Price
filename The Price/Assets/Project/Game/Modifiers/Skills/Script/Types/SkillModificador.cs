using System.Collections.Generic;
using UnityEngine;

public class SkillModificador : SkillManager {

    [Header("Modificador")]
    public float[] modification = new float[11];

    private List<int> index = new List<int>();

    protected override void TakeEffect()
    {
        for(int i = 0; i < modification.Length; i++)
        {
            if (modification[i] != 0)
            {
                index.Add(i);

                _player.SetValue(i, modification[i], false);
                _player.SetValue(i, modification[i], true);
            }
        }
    }
    protected override void DestroySkill()
    {
        // DEVOLVER LOS VALORES PREVIOS AL CAMBIO DE STATS
        for(int i = 0; i < index.Count; i++)
        {
            _player.SetValue(index[i], -modification[index[i]], false);
            _player.SetValue(index[i], -modification[index[i]], true);
        }

        base.DestroySkill();
    }
}
