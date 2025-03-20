using UnityEngine;
using UnityEditor;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using Unity.VisualScripting;
using System.Collections.Generic;

public class Genmap : EditorWindow
{
    public GameObject objectStart;
    public GameObject objectlevel;
    public GameObject objectConnect;
    public GameObject objectEnd;

    bool showOptions = false;

    public int count;
    public int maxCount = 10; // เพิ่ม max count
    public float spacing = 2.0f;

    public enum SpawnDirection { X, Y, Z, Random }
    public SpawnDirection spawnDirection = SpawnDirection.X; // ค่าตั้งต้นคือแกน X

    private Vector3 direction; // ทิศทางการ Spawn (จะใช้ทิศทางเดียวกันสำหรับทุกตัว)

    public int ConnectNum;

    [MenuItem("Tools/Genmap")]
    public static void ShowWindow()
    {
        GetWindow<Genmap>("GenmapTest");
    }

    void OnGUI()
    {
        GUILayout.Label("Object Spawner Settings", EditorStyles.boldLabel);

        objectStart = (GameObject)EditorGUILayout.ObjectField("Select Start:", objectStart, typeof(GameObject), false);
        objectlevel = (GameObject)EditorGUILayout.ObjectField("Select Level:", objectlevel, typeof(GameObject), false);
        objectConnect = (GameObject)EditorGUILayout.ObjectField("Select Connect:", objectConnect, typeof(GameObject), false);
        objectEnd = (GameObject)EditorGUILayout.ObjectField("Select End:", objectEnd, typeof(GameObject), false);

        if (objectStart != null && objectlevel != null && objectConnect != null && objectEnd != null)
        {
            showOptions = true;
        }
        else
        {
            showOptions = false;
        }

        if (GUILayout.Button("Auto Assets"))
        {
            objectStart = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cube1.prefab");
            objectlevel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cube2.prefab");
            objectConnect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cube3.prefab");
            objectEnd = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Cube4.prefab");

            showOptions = true;
        }

        if (showOptions)
        {
            if (GUILayout.Button("Clear"))
            {
                objectStart = null;
                objectlevel = null;
                objectConnect = null;
                objectEnd = null;

                showOptions = false;
            }
            GUILayout.Label("Prefab Spawner", EditorStyles.boldLabel);

            ConnectNum = EditorGUILayout.IntField("Connect", ConnectNum);
            maxCount = EditorGUILayout.IntField("Max Count", maxCount);
            spacing = EditorGUILayout.FloatField("Spacing", spacing);
            spawnDirection = (SpawnDirection)EditorGUILayout.EnumPopup("Spawn Direction", spawnDirection);

            if (GUILayout.Button("Spawn Levels"))
            {
                    SpawnPrefabs();
            }
        }
    }

    private void SpawnPrefabs()
    {
        count = Random.Range(2, maxCount); // สุ่มจำนวน prefab

        // 1. สร้าง objectStart
        GameObject instanceStart = (GameObject)PrefabUtility.InstantiatePrefab(objectStart);
        Vector3 lastPosition = instanceStart.transform.position; // เริ่มต้นที่ objectStart

        // 2. สร้าง instanceLevel ชุดแรก
        direction = GetSpawnDirection(); // สุ่มทิศทางแรก
        int levelCount = Random.Range(1, count);
        for (int j = 0; j < levelCount; j++)
        {
            Vector3 position = lastPosition + direction * spacing;
            GameObject instanceLevel = (GameObject)PrefabUtility.InstantiatePrefab(objectlevel);
            instanceLevel.transform.position = position;
            Undo.RegisterCreatedObjectUndo(instanceLevel, "Spawn Prefabs");
            lastPosition = position;
        }

        // 3. วนลูปสร้าง instanceConnect + instanceLevel ตามจำนวน ConnectNum
        for (int i = 0; i < ConnectNum; i++)
        {
            // 3.1 สร้าง instanceConnect
            GameObject instanceConnect = (GameObject)PrefabUtility.InstantiatePrefab(objectConnect);
            instanceConnect.transform.position = lastPosition + direction * spacing;
            Undo.RegisterCreatedObjectUndo(instanceConnect, "Spawn Prefabs Connect");
            lastPosition = instanceConnect.transform.position;

            // 3.2 เปลี่ยนทิศทางใหม่
            direction = GetSpawnDirection();

            // 3.3 สร้าง instanceLevel ชุดใหม่
            levelCount = Random.Range(1, count);
            for (int j = 0; j < levelCount; j++)
            {
                Vector3 position = lastPosition + direction * spacing;
                GameObject instanceLevel = (GameObject)PrefabUtility.InstantiatePrefab(objectlevel);
                instanceLevel.transform.position = position;
                Undo.RegisterCreatedObjectUndo(instanceLevel, "Spawn Prefabs");
                lastPosition = position;
            }
        }

        Debug.Log("Spawned " + ConnectNum + " groups of objects.");
    }

    // ฟังก์ชันสุ่มทิศทาง
    private Vector3 GetSpawnDirection()
    {
        switch (spawnDirection)
        {
            case SpawnDirection.X:
                return Vector3.right; // ทิศทางในแกน X
            case SpawnDirection.Y:
                return Vector3.up; // ทิศทางในแกน Y
            case SpawnDirection.Z:
                return Vector3.forward; // ทิศทางในแกน Z
            case SpawnDirection.Random:
                int randomAxis = Random.Range(0, 3);
                if (randomAxis == 0) return Vector3.right; // X
                if (randomAxis == 1) return Vector3.up; // Y
                return Vector3.forward; // Z
            default:
                return Vector3.right;
        }
    }

    // ฟังก์ชันคำนวณตำแหน่งการ spawn ตามทิศทางที่สุ่ม
    private Vector3 GetSpawnPosition(int index, Vector3 direction)
    {
        float randomSpacing = Random.Range(spacing * 1, spacing * 1); // เพิ่มการสุ่ม spacing เล็กน้อย
        return direction * (index * randomSpacing); // ใช้ทิศทางเดียวกันสำหรับทุกตัว
    }
}