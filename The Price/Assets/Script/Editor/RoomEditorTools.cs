#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Room))]
public class RoomEditorTools : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generar WalkableMap para este Room"))
        {
            Room room = (Room)target;

            WalkableMapGenerator generator = room.GetComponentInChildren<WalkableMapGenerator>();
            if (generator == null)
            {
                Debug.LogError("No se encontró WalkableMapGenerator en el prefab.");
                return;
            }

            generator.SizeMap = room.sizeMap;
            generator.InitialPosition = room.spawnMap.transform.position;
            generator.GenerateWalkableMap();
            var map = generator.walkableMap;

            room.mapSize = room.sizeMap;
            room.flatWalkableMap = new TypeNode[room.sizeMap.x * room.sizeMap.y];

            for (int x = 0; x < room.sizeMap.x; x++)
            {
                for (int y = 0; y < room.sizeMap.y; y++)
                {
                    room.flatWalkableMap[y * room.sizeMap.x + x] = map[x, y];
                }
            }

            EditorUtility.SetDirty(room);
            Debug.Log("WalkableMap generado y guardado en el Room.");
        }
    }
}
#endif
