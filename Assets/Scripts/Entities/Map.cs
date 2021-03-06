﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Map : MonoBehaviour {
    #region Attributes
    private static bool debug = false;                  // Whether to debug the maps or not
    [SerializeField]
    protected string path;                                // The name of the map in resources
    [SerializeField]
    protected AudioClip music;                            // The music to play on the map
    protected int width;                                  // The width of the map in tiles
    protected int height;                                 // The height of the map in tiles
    protected List<GameObject> tiles;                     // The list of tiles to reference
    private List<MapNode> nodes;                        // The list of nodes for pathfinding
    protected Dictionary<StaticObject, int> takenTiles;   // The tiles that are occupied on the map
    #endregion

    #region Properties
    // Returns width
    public int Width
    {
        get
        {
            return width;
        }
    }

    // Returns height
    public int Height
    {
        get
        {
            return height;
        }
    }

    // Returns tiles
    public List<GameObject> Tiles
    {
        get
        {
            return tiles;
        }
    }

    // Returns nodes
    public List<MapNode> Nodes
    {
        get
        {
            return nodes;
        }
    }

    // Returns music
    public AudioClip Music
    {
        get
        {
            return music;
        }
    }

    // Returns takenTiles
    public Dictionary<StaticObject, int> TakenTiles
    {
        get
        {
            return takenTiles;
        }
    }
    #endregion

    #region Event Functions
    protected virtual void Awake ()
    {
        // Find and read the mapping image
        Texture2D mapLayout = Resources.Load<Texture2D>("Maps/Textures/" + path);
        List<Color> data = Helper.ReverseInChunks(new List<Color>(mapLayout.GetPixels()), mapLayout.width);
        string[] entries = Resources.Load<TextAsset>("Maps/Tiles/" + path).text.Split(new string[] { "..." }, System.StringSplitOptions.None);
        width = mapLayout.width;
        height = mapLayout.height;
        tiles = new List<GameObject>(width * height);
        List<int> badNodeTiles = new List<int>();
        takenTiles = new Dictionary<StaticObject, int>();
        // Set up special tiles
        List<int> entrances = new List<int>();
        List<int> signs = new List<int>();
        List<int> dungeons = new List<int>();
        List<int> roomSwitches = new List<int>();
        // Match colors in the picture to tile type
        for (int i = 0; i < data.Count; ++i)
        {
            if (data[i] == new Color(0, 1, 0))
            {
                tiles.Add(Instantiate(Resources.Load<GameObject>("Tiles/Ground Tile"), transform));
            }
            else if (data[i] == new Color(1, 0, 0))
            {
                tiles.Add(Instantiate(Resources.Load<GameObject>("Tiles/Wall Tile"), transform));
                badNodeTiles.Add(i);
            }
            else if (data[i] == new Color(0, 0, 1))
            {
                tiles.Add(Instantiate(Resources.Load<GameObject>("Tiles/Entrance Tile"), transform));
                entrances.Add(i);
            }
            else if (data[i] == new Color(0, 1, 1))
            {
                tiles.Add(Instantiate(Resources.Load<GameObject>("Tiles/Dungeon Entrance Tile")));
                dungeons.Add(i);
            }
            else if (data[i] == new Color(1, 0, 1))
            {
                tiles.Add(Instantiate(Resources.Load<GameObject>("Tiles/Sign Tile"), transform));
                signs.Add(i);
            }
            else if (data[i] == new Color(1, 1, 0))
            {
                tiles.Add(Instantiate(Resources.Load<GameObject>("Tiles/Bed Tile"), transform));
            }
            else if (data[i] == new Color(1, 1, 1))
            {
                tiles.Add(Instantiate(Resources.Load<GameObject>("Tiles/Switch Tile"), transform));
            }
            else
            {
                Debug.Log("Tile at (" + (i % width).ToString() + ", " + (i / width).ToString() + ") is not valid. Color is " + data[i].ToString());
            }
            tiles[i].GetComponent<Tile>().UpdatePosition(new Vector2(i % width, -i / width));
        }
        SetUpNodes(badNodeTiles);
        // Add in the special tiles to the given locations
        int entrancesI = 0, dungeonsI = 0, signsI = 0;
        if (entries[0].Trim().Length != 0)
        {
            Tile t;
            string[] args;
            for (int i = 0; i < entries.Length; ++i)
            {
                switch (entries[i].Trim()[0])
                {
                    case 'E':
                        Entrance e = tiles[entrances[entrancesI]].GetComponent<Entrance>();
                        args = entries[i].Split(':')[1].Split(',');
                        e.sceneTo = args[0];
                        e.tileFrom = int.Parse(args[1]);
                        e.moveTo = (Direction)int.Parse(args[2]);
                        t = tiles[entrances[entrancesI++]].GetComponent<Tile>();
                        e.PlaceOnMap(this, -(int)t.position.y * width + (int)t.position.x);
                        break;
                    /*case 'S':
                        tiles[shops[shopsI++]].GetComponent<Shop>().numOfItems = int.Parse(entries[i].Split(':')[1]);
                        break;*/
                    case 'N':
                        Sign s = tiles[signs[signsI]].GetComponent<Sign>();
                        s.path = entries[i].Split(':')[1];
                        t = tiles[signs[signsI++]].GetComponent<Tile>();
                        s.PlaceOnMap(this, -(int)t.position.y * width + (int)t.position.x);
                        break;
                    case 'D':
                        DungeonEntrance d = tiles[dungeons[dungeonsI]].GetComponent<DungeonEntrance>();
                        args = entries[i].Split(':')[1].Split(',');
                        d.sceneTo = args[0];
                        d.tileFrom = int.Parse(args[1]);
                        d.moveTo = (Direction)int.Parse(args[2]);
                        d.roomTo = int.Parse(args[3]);
                        t = tiles[dungeons[dungeonsI++]].GetComponent<Tile>();
                        d.PlaceOnMap(this, -(int)t.position.y * width + (int)t.position.x);
                        break;
                    default:
                        Debug.Log("Invalid entry type");
                        break;
                }
            }
        }
        // Place the map background sprite
        transform.Find("MapSprite").transform.position = new Vector3((float)width / 2 - 0.5f, (float)-height / 2 + 0.5f, 0);
    }

    // Use this for initialization
    void Start ()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {

    }
    #endregion

    #region Methods
    // Returns whether there is a solid object at a point
    public bool TileIsTaken (int tile)
    {
        foreach (StaticObject s in takenTiles.Keys)
        {
            if (takenTiles[s] == tile)
            {
                if (s != null && s.Solid)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Returns the object at a point
    public GameObject ObjectTakingTile (int tile)
    {
        foreach (StaticObject s in takenTiles.Keys)
        {
            if (takenTiles[s] == tile)
            {
                return s.gameObject;
            }
        }
        return null;
    }

    // Return the distance between two tiles
    public int GetDistance (int t1, int t2)
    {
        return Mathf.Abs(t1 % width - t2 % width) + Mathf.Abs(t1 / width - t2 / width);
    }

    // Return the position of the tile above pos if applicable
    public int TileAbove (int pos)
    {
        return Mathf.Max(pos - width, pos % width);
    }

    // Return the position of the tile above pos if possible
    public int TileAboveStrict (int pos)
    {
        return pos < 0 || pos - width < pos % width ? -1 : pos - width;
    }

    // Return the position of the tile dis number of tiles above pos if applicable
    public int TileAboveStrict (int pos, int dis)
    {
        return pos < 0 || pos - dis * width < pos % width ? -1 : pos - dis * width;
    }

    // Return the position of the tile below pos if applicable
    public int TileBelow (int pos)
    {
        return Mathf.Min(pos + width, (height - 1) * width + pos % width);
    }

    // Return the position of the tile below pos if possible
    public int TileBelowStrict (int pos)
    {
        return pos < 0 || pos + width > (height - 1) * width + pos % width ? -1 : pos + width;
    }

    // Return the position of the tile dis number of tiles below pos if applicable
    public int TileBelowStrict (int pos, int dis)
    {
        return pos < 0 || pos + dis * width > (height - 1) * width + pos % width ? -1 : pos + dis * width;
    }

    // Return the position of the tile to the left of pos if applicable
    public int TileLeft (int pos)
    {
        return Mathf.Max(pos - 1, pos / width * width);
    }

    // Return the position of the tile to the left of pos if possible
    public int TileLeftStrict (int pos)
    {
        return pos < 0 || pos - 1 < pos / width * width ? -1 : pos - 1;
    }

    // Return the position of the tile dis number of tiles to the left of pos if applicable
    public int TileLeftStrict (int pos, int dis)
    {
        return pos < 0 || pos - dis < pos / width * width ? -1 : pos - dis;
    }

    // Return the position of the tile to the right of pos if applicable
    public int TileRight (int pos)
    {
        return Mathf.Min(pos + 1, (pos / width + 1) * width - 1);
    }

    // Return the position of the tile to the right of pos if possible
    public int TileRightStrict (int pos)
    {
        return pos < 0 || pos + 1 > (pos / width + 1) * width - 1 ? -1 : pos + 1;
    }

    // Return the position of the tile dis number of tiles to the right of pos if applicable
    public int TileRightStrict (int pos, int dis)
    {
        return pos < 0 || pos + dis > (pos / width + 1) * width - 1 ? -1 : pos + dis;
    }

    // Return the node that the given tile is in
    public MapNode NodeTileIn (int tile)
    {
        foreach (MapNode m in nodes)
        {
            if (m.TileIsIn(tile))
            {
                return m;
            }
        }
        return null;
    }

    // Return the list of nodes that make a path from start to end
    public List<MapNode> FindPath (MapNode start, MapNode end)
    {
        List<MapNode> result = new List<MapNode> { start };
        if (start == end)
        {
            return result;
        }
        // See if end is adjacent to start
        foreach (MapNode mn in start.adjacentNodes.Keys)
        {
            if (mn == end)
            {
                result.Add(mn);
                return result;
            }
        }
        // Perform an A* search based on distance
        List<MapNode> noSearch = new List<MapNode> { start };
        foreach (MapNode m in start.GetSortedKeys(end))
        {
            // To prevent infinite back-and-forth searches
            List<MapNode> search = FindPath(m, end, noSearch);
            if (search[search.Count - 1] == end)
            {
                result.AddRange(search);
                // Remove any unnecessary movements
                for (int i = 0; i < result.Count; ++i)
                {
                    for (int j = i + 2; j < result.Count; ++j)
                    {
                        if (result[i].adjacentNodes.ContainsKey(result[j]))
                        {
                            result.RemoveRange(i + 1, j - (i + 1));
                        }
                    }
                }
                return result;
            }
        }
        return result;
    }

    // A private version of FindPath for recursive calls
    private List<MapNode> FindPath (MapNode start, MapNode end, List<MapNode> noSearch)
    {
        noSearch.Add(start);
        List<MapNode> result = new List<MapNode> { start };
        // See if end is adjacent to start
        foreach (MapNode mn in start.adjacentNodes.Keys)
        {
            if (mn == end)
            {
                result.Add(mn);
                return result;
            }
        }
        // Perform an A* search based on distance
        foreach (MapNode m in start.GetSortedKeys(end))
        {
            // To prevent infinite back-and-forth searches
            if (!noSearch.Contains(m))
            {
                List<MapNode> search = FindPath(m, end, noSearch);
                if (search[search.Count - 1] == end)
                {
                    result.AddRange(search);
                    return result;
                }
            }
        }
        return result;
    }

    // Set up the nodes that are needed for pathfinding
    private void SetUpNodes (List<int> badTiles)
    {
        nodes = new List<MapNode>();
        for (int i = 0; i < tiles.Count; ++i)
        {
            if (NodeTileIn(i) == null && tiles[i].GetComponent<Tile>().type != TileType.Wall)
            {
                nodes.Add(new MapNode(this, i, badTiles));
                nodes[nodes.Count - 1].FindAdjacents(nodes);
                badTiles.AddRange(nodes[nodes.Count - 1].TilesCovered());
            }
        }
        if (debug)
        {
            Texture2D nodePic = Resources.Load<Texture2D>("Maps/Textures/" + path + "Nodes");
            List<Color> data = Helper.ReverseInChunks(new List<Color>(nodePic.GetPixels()), nodePic.width);
            nodePic.SetPixels(data.ToArray());
            foreach (MapNode mn in nodes)
            {
                Color[] colors = Helper.FillList(width * height, Helper.RandomColor(false)).ToArray();
                nodePic.SetPixels(mn.start % width, mn.start / width, mn.width, mn.height, colors);
            }
            nodePic.Apply();
            data = Helper.ReverseInChunks(new List<Color>(nodePic.GetPixels()), nodePic.width);
            nodePic.SetPixels(data.ToArray());
            nodePic.Apply();
            File.WriteAllBytes(Application.dataPath + "/Resources/Maps/Textures/" + path + "Nodes.png", nodePic.EncodeToPNG());
        }
    }

    // Return a tile based on a direction and a start tile
    public int TileInDirection (Direction dir, int start)
    {
        switch (dir)
        {
            case Direction.Up:
                return GameController.instance.map.TileAbove(start);
            case Direction.Down:
                return GameController.instance.map.TileBelow(start);
            case Direction.Left:
                return GameController.instance.map.TileLeft(start);
            case Direction.Right:
                return GameController.instance.map.TileRight(start);
            default:
                return -1;
        }
    }
    #endregion
}
