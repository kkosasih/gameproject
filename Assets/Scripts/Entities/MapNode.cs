﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode {
    #region Attributes
    public int start;                                       // The top-left point
    public int width;                                       // The amount of tiles stretched right
    public int height;                                      // The amount of tiles stretched down
    public Dictionary<MapNode, Direction> adjacentNodes;    // The nodes adjacent to this one
    private Map map;                                        // The map this node is a part of
    #endregion

    #region Constructors
    // A constructor for a node that is made from a map and a position on it
    public MapNode(Map nMap, int nStart, List<int> badTiles)
    {
        // Initialize vars
        map = nMap;
        start = nStart;
        adjacentNodes = new Dictionary<MapNode, Direction>();
        // Stretch right
        int i = 0;
        for (; map.TileRightStrict(start, i) != -1; ++i)
        {
            if (badTiles.Contains(map.TileRightStrict(start, i)))
            {
                break;
            }
        }
        width = i;
        // Stretch downward
        int j = 0;
        for (bool found = false; !found && map.TileBelowStrict(start, j) != -1; ++j)
        {
            for (int k = 0; k < width; ++k)
            {
                if (badTiles.Contains(map.TileRightStrict(map.TileBelowStrict(start, j), k)))
                {
                    found = true;
                    break;
                }
            }
        }
        height = j - 1;
    }
    #endregion

    #region Methods
    // Returns an array of tile numbers that are covered by the node
    public List<int> TilesCovered ()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                result.Add(map.TileRightStrict(map.TileBelowStrict(start, i), j));
            }
        }
        return result;
    }

    // Returns whether the tile is in the node
    public bool TileIsIn (int tile)
    {
        return TileIsBetween(tile, start, BottomRight(), false) && TileIsBetween(tile, start, BottomRight(), true);
    }

    // Returns the bottom-right corner position
    public int BottomRight ()
    {
        return map.TileBelowStrict(map.TileRightStrict(start, width - 1), height - 1);
    }

    // Returns whether a node is adjacent to this one
    public Direction Adjacency (MapNode other)
    {
        // If other's left or right boundaries are within this node's left or right boundaries
        if (other.start % map.Width <= BottomRight() % map.Width && other.BottomRight() % map.Width >= start % map.Width)
        {
            // Return if it is above or below
            if (other.start / map.Width == BottomRight() / map.Width + 1)
            {
                return Direction.Down;
            }
            else if (other.BottomRight() / map.Width == start / map.Width - 1)
            {
                return Direction.Up;
            }
        }
        // If other's top or bottom boundaries are within this node's top or bottom boundaries
        else if (other.start / map.Width <= BottomRight() / map.Width && other.BottomRight() / map.Width >= start / map.Width)
        {
            // Return if it is left or right
            if (other.start % map.Width == BottomRight() % map.Width + 1)
            {
                return Direction.Right;
            }
            else if (other.BottomRight() % map.Width == start % map.Width - 1)
            {
                return Direction.Left;
            }
        }
        return Direction.Invalid;
    }

    // Sets up a list of adjacent nodes given a list
    public void FindAdjacents (List<MapNode> others)
    {
        foreach (MapNode m in others)
        {
            Direction test = Adjacency(m);
            if (test != Direction.Invalid)
            {
                // Add the adjacent node to its list and itself to the node's adjacent list
                adjacentNodes.Add(m, test);
                if (!m.adjacentNodes.ContainsKey(this))
                {
                    Direction opp = Direction.Invalid;
                    switch (test)
                    {
                        case Direction.Up:
                            opp = Direction.Down;
                            break;
                        case Direction.Left:
                            opp = Direction.Right;
                            break;
                        case Direction.Down:
                            opp = Direction.Up;
                            break;
                        case Direction.Right:
                            opp = Direction.Left;
                            break;
                    }
                    m.adjacentNodes.Add(this, opp);
                }
            }
        }
    }

    // Finds the distance between the two starting nodes
    public int GetDistance (MapNode other)
    {
        return Mathf.Abs(start % map.Width - other.start % map.Width) + Mathf.Abs(start / map.Width - other.start / map.Width);
    }

    // Return a list of adjacent nodes organized by distance
    public List<MapNode> GetSortedKeys (MapNode other)
    {
        List<MapNode> result = new List<MapNode>();
        Dictionary<MapNode, int> keyDistances = new Dictionary<MapNode, int>();
        foreach (MapNode mn in adjacentNodes.Keys)
        {
            keyDistances.Add(mn, other.GetDistance(mn));
        }
        for (int i = 0; i < adjacentNodes.Keys.Count; ++i)
        {
            MapNode toAdd = new MapNode(map, 0, new List<int>());
            int min = map.Width * 2;
            foreach (MapNode mn in keyDistances.Keys)
            {
                if (!result.Contains(mn) && keyDistances[mn] < min)
                {
                    toAdd = mn;
                    min = keyDistances[mn];
                }
            }
            result.Add(toAdd);
        }
        return result;
    }

    // Find the tile to transfer to an adjacent node given a specific tile
    public int GateToNode (MapNode other, int startTile)
    {
        // If the parameters aren't valid, don't continue
        if (!adjacentNodes.ContainsKey(other) || !TileIsIn(startTile))
        {
            return -1;
        }
        else
        {
            int distance = 0;
            switch (adjacentNodes[other])
            {
                // If other node is below
                case Direction.Down:
                    distance = other.start / map.Width - startTile / map.Width;
                    // If you need to travel to line up with the node, do so
                    if (startTile % map.Width > other.BottomRight() % map.Width)
                    {
                        return map.TileBelowStrict(map.TileLeftStrict(startTile, startTile % map.Width - other.BottomRight() % map.Width), distance - 1);
                    }
                    else if (startTile % map.Width < other.start % map.Width)
                    {
                        return map.TileBelowStrict(map.TileRightStrict(startTile, other.start % map.Width - startTile % map.Width), distance - 1);
                    }
                    else
                    {
                        return map.TileBelowStrict(startTile, distance - 1);
                    }
                // If other node is right
                case Direction.Right:
                    distance = other.start % map.Width - startTile % map.Width;
                    // If you need to travel to line up with the node, do so
                    if (startTile / map.Width > other.BottomRight() / map.Width)
                    {
                        return map.TileRightStrict(map.TileAboveStrict(startTile, startTile / map.Width - other.BottomRight() / map.Width), distance - 1);
                    }
                    else if (startTile / map.Width < other.start / map.Width)
                    {
                        return map.TileRightStrict(map.TileBelowStrict(startTile, other.start / map.Width - startTile / map.Width), distance - 1);
                    }
                    else
                    {
                        return map.TileRightStrict(startTile, distance - 1);
                    }
                // If other node is above
                case Direction.Up:
                    distance = startTile / map.Width - other.BottomRight() / map.Width;
                    // If you need to travel to line up with the node, do so
                    if (startTile % map.Width > other.BottomRight() % map.Width)
                    {
                        return map.TileAboveStrict(map.TileLeftStrict(startTile, startTile % map.Width - other.BottomRight() % map.Width), distance - 1);
                    }
                    else if (startTile % map.Width < other.start % map.Width)
                    {
                        return map.TileAboveStrict(map.TileRightStrict(startTile, other.start % map.Width - startTile % map.Width), distance - 1);
                    }
                    else
                    {
                        return map.TileAboveStrict(startTile, distance - 1);
                    }
                // If other node is left
                case Direction.Left:
                    distance = startTile % map.Width - other.BottomRight() % map.Width;
                    // If you need to travel to line up with the node, do so
                    if (startTile / map.Width > other.BottomRight() / map.Width)
                    {
                        return map.TileLeftStrict(map.TileAboveStrict(startTile, startTile / map.Width - other.BottomRight() / map.Width), distance - 1);
                    }
                    else if (startTile / map.Width < other.start / map.Width)
                    {
                        return map.TileLeftStrict(map.TileBelowStrict(startTile, other.start / map.Width - startTile / map.Width), distance - 1);
                    }
                    else
                    {
                        return map.TileLeftStrict(startTile, distance - 1);
                    }
            }
            return -1;
        }
    }

    // Find out if a tile is between two others vertically or horizontally
    public bool TileIsBetween (int tile, int s, int e, bool horizontal)
    {
        return horizontal ? tile % map.Width >= s % map.Width && tile % map.Width <= e % map.Width : tile / map.Width >= s / map.Width && tile / map.Width <= e / map.Width;
    }
    #endregion
}
