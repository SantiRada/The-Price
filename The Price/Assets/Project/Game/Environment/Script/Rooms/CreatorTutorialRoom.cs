using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreatorTutorialRoom : MonoBehaviour {

    public string nameScene;

    private TypeRoom[] _typeRooms;

    private void OnEnable() { nameScene = SceneManager.GetActiveScene().name; }
    public TypeRoom[] CreateRooms()
    {
        _typeRooms = new TypeRoom[18];

        if(nameScene == "Terrenal")
        {
            // Asignar las salas fijas
            _typeRooms[0] = TypeRoom.Basic;
            _typeRooms[1] = TypeRoom.Gold;
            _typeRooms[2] = TypeRoom.Skill;
            _typeRooms[4] = TypeRoom.Gold;
            _typeRooms[7] = TypeRoom.MiniBoss;
            _typeRooms[8] = TypeRoom.Shop;
            _typeRooms[9] = TypeRoom.Skill;
            _typeRooms[15] = TypeRoom.Boss;
            _typeRooms[16] = TypeRoom.Astral;
            _typeRooms[17] = TypeRoom.MaxBoss;

            // Asignar salas Objetos
            SelectedOptionalRooms(TypeRoom.Object, new int[] { 4, 7, 11, 13 }, 2, 4);

            // Asignar salas Aptitudes
            SelectedOptionalRooms(TypeRoom.Aptitud, new int[] { 2, 6, 12, 14 }, 2, 4);

            // Asignar salas aleatorias restantes a Oro
            for (int i = 0; i < _typeRooms.Length; i++)
            {
                if (_typeRooms[i] == TypeRoom.Null) { _typeRooms[i] = TypeRoom.Gold; }
            }

        }
        else if(nameScene == "Cielo")
        {
            // Asignar las salas fijas
            _typeRooms[0] = TypeRoom.Basic;
            _typeRooms[1] = TypeRoom.Basic;
            _typeRooms[2] = TypeRoom.Gold;
            _typeRooms[3] = TypeRoom.Skill;
            _typeRooms[4] = TypeRoom.Gold;
            _typeRooms[7] = TypeRoom.MiniBoss;
            _typeRooms[8] = TypeRoom.Shop;
            _typeRooms[9] = TypeRoom.Skill;
            _typeRooms[15] = TypeRoom.Boss;
            _typeRooms[16] = TypeRoom.Astral;
            _typeRooms[17] = TypeRoom.MaxBoss;

            // Asignar salas Objetos
            SelectedOptionalRooms(TypeRoom.Object, new int[] { 4, 7, 11, 13 }, 2, 4);

            // Asignar salas Aptitudes
            SelectedOptionalRooms(TypeRoom.Aptitud, new int[] { 2, 6, 12, 14 }, 2, 4);

            // Asignar salas aleatorias restantes a Oro
            for (int i = 0; i < _typeRooms.Length; i++)
            {
                if (_typeRooms[i] == TypeRoom.Null) { _typeRooms[i] = TypeRoom.Gold; }
            }
        }

        return _typeRooms;
    }
    private void SelectedOptionalRooms(TypeRoom type, int[] posiciones, int minCount, int maxCount)
    {
        List<int> disponibles = new List<int>();
        foreach (int pos in posiciones)
        {
            if (_typeRooms[pos - 1] == TypeRoom.Null)
            {
                disponibles.Add(pos);
            }
        }
        int count = Random.Range(minCount, Mathf.Min(maxCount, disponibles.Count) + 1);
        List<int> seleccionadas = new List<int>();
        while (seleccionadas.Count < count)
        {
            int randomIndex = Random.Range(0, disponibles.Count);
            if (!seleccionadas.Contains(disponibles[randomIndex]))
            {
                seleccionadas.Add(disponibles[randomIndex]);
            }
        }
        foreach (int pos in seleccionadas)
        {
            _typeRooms[pos - 1] = type;
        }
    }
}
