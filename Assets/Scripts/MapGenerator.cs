using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;
    [Range(0, 20)]
    public int smoothingIterations;
    [Range(0, 8)]
    public int smoothingNeighbourCondition;

    int[,] map;

    private void Start() {
        GenerateMap();        
    }

    void GenerateMap() {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothingIterations; i++) {
            SmoothMap();
        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(map, 1);
    }

    void RandomFillMap() {
        if (useRandomSeed) {
            seed = Time.time.ToString();
        }

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = 1; //Borders of the grid are always walls
                 } else {
                    map[x, y] = pseudoRandom.Next(0, 100) < randomFillPercent ? 1 : 0; //either the node has wall (1) or not (0)
                }               
            }
        }
    }

    void SmoothMap() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                
                if(neighbourWallTiles > smoothingNeighbourCondition) {
                    map[x, y] = 1;
                } else if (neighbourWallTiles < smoothingNeighbourCondition) {
                    map[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int neighbourX = gridX-1 ; neighbourX <= gridX+1 ; neighbourX++) {
            for (int neighbourY = gridY-1 ; neighbourY <= gridY+1 ; neighbourY++) {
                if(neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) { //check that the neighbour is not outside the borders
                    if (neighbourX != gridX || neighbourY != gridY) { //check that the neighbour is not oneself
                        wallCount += map[neighbourX, neighbourY];
                    }
                } else {
                    wallCount++;
                }                
            }
        }
        return wallCount;
    }

    //private void OnDrawGizmos() {
    //    if (map != null) {
    //        for (int x = 0; x < width; x++) {
    //            for (int y = 0; y < height; y++) {
                    


    //                Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
    //                Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
    //                Gizmos.DrawCube(pos, Vector3.one);
    //            }
    //        }
    //    }
    //}

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GenerateMap();
            print("New map");
        }
    }
}
