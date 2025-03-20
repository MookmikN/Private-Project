using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static Genmap;

public class Player_Rat : MonoBehaviour
{
    public float Speed = 1f;

    public enum SpawnDirection { X, Y, Z, Random }
    public SpawnDirection spawnPosition = SpawnDirection.X; // ค่าตั้งต้นคือแกน X

    private List<GameObject> createdPrefabs = new List<GameObject>();

    private Vector3 Randomposition;

    [Header("Object Body")]
    public GameObject Heand_Rat;
    public GameObject Body_Rat;
    public GameObject Tail_Rat;

    [Header("canvas")]
    public Transform canvasOBJ;
    // Start is called before the first frame update
    void Start()
    {
         Randomposition = GetMove();
         CreateOBJ();
    }

    // Update is called once per frame
    void Update()
    {
        createdPrefabs[0].transform.position += Randomposition*Speed*Time.deltaTime;
    }


    public Vector3 GetMove()
    {
        switch (spawnPosition)
        {
            case SpawnDirection.X:
                return Vector3.right; // ทิศทางในแกน X
            case SpawnDirection.Y:
                return Vector3.up; // ทิศทางในแกน Y
            case SpawnDirection.Z:
                return Vector3.forward; // ทิศทางในแกน Z
            case SpawnDirection.Random:
                int randomAxis = Random.Range(0, 3);
                if (randomAxis == 0) return Vector3.right;
                if (randomAxis == 1) return Vector3.up;
                return Vector3.down;
            default:return Vector3.left;
        }    
        
    }

    public void CreateOBJ()
    {
        GameObject newImage = Instantiate(Heand_Rat, canvasOBJ);
        createdPrefabs.Add(newImage);
        Debug.Log("HI : "+createdPrefabs.Count);
    }
}
