using UnityEngine;

public class SkillModificador : SkillManager {

    [Header("Modificador")]
    public float[] modification = new float[11];
    private bool isDestroyed = false;

    protected override void TakeEffect()
    {
        for(int i = 0; i < modification.Length; i++)
        {
            _player.SetValue(i, modification[i], false);
            _player.SetValue(i, modification[i], true);
        }
    }
    private void OnDestroy() { isDestroyed = true; }
    protected override void DestroySkill()
    {
        // DEVOLVER LOS VALORES PREVIOS AL CAMBIO DE STATS
        for (int i = 0; i < modification.Length; i++)
        {
            _player.SetValue(i, -modification[i], false);
            _player.SetValue(i, -modification[i], true);
        }

        // VERIFICA SI PREVIENE ALGUN TIPO DE DAÑO Y LO REMUEVE DEL PLAYER
        if (preventDamagePerType)
        {
            int[] count = { 0, 0, 0, 0, 0 };
            bool[] reflectFalse = { false, false, false, false, false };
            _player.PreventDamagePerType(count, reflectFalse);
        }

        // QUITAR AFECCION DE ALGUN ESTADO EN EL ATAQUE DEL PLAYER
        if (hasStatePerAttack) _player.AddStatePerDamage(TypeState.Null, 0);

        if(!isDestroyed) Destroy(gameObject);
    }
}