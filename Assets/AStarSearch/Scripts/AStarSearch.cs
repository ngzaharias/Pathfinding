using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarSearch : MonoBehaviour
{
	MapGenerator cached_map;
	void Awake()
	{
		cached_map = GetComponent<MapGenerator>();
	}

	public Node[] Search(Coord start, Coord goal)
	{
		List<Node> openSet = new List<Node>();
		List<Node> closedSet = new List<Node>();

		while (openSet.Count > 0)
		{
			Node current = PopLowestHeuristic(openSet);
			if (current.coord == goal)
				return ReconstructPath(current);

			closedSet.Add(current);
			Node[] neighbours = Neighbours(current);
			foreach (Node neighbour in neighbours)
			{
				if (Contains(neighbour, closedSet) == true)
					continue;

				float estimatedScore = current.score + 1;
				if (Contains(neighbour, openSet) == false)
					openSet.Add(neighbour);
				else if (estimatedScore >= neighbour.score)
					continue;
			}
		}

		return null;
	}

	public Node[] Neighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();
		Coord neighbourA = new Coord(node.col - 1, node.row + 0); // left
		Coord neighbourB = new Coord(node.col + 1, node.row + 0); // right
		Coord neighbourC = new Coord(node.col + 0, node.row + 1); // top
		Coord neighbourD = new Coord(node.col + 0, node.row - 1); // bottom
		if (IsWalkable(neighbourA) == true)
			neighbours.Add(new Node(neighbourA, node, node.score + 1));
		if (IsWalkable(neighbourB) == true)
			neighbours.Add(new Node(neighbourB, node, node.score + 1));
		if (IsWalkable(neighbourC) == true)
			neighbours.Add(new Node(neighbourC, node, node.score + 1));
		if (IsWalkable(neighbourD) == true)
			neighbours.Add(new Node(neighbourD, node, node.score + 1));

		Coord neighbourE = new Coord(node.col - 1, node.row + 1); // top-left
		Coord neighbourF = new Coord(node.col + 1, node.row + 1); // top-right
		Coord neighbourG = new Coord(node.col - 1, node.row - 1); // bottom-left
		Coord neighbourH = new Coord(node.col + 1, node.row - 1); // bottom-right
		if (IsWalkable(neighbourG) == true)
			neighbours.Add(new Node(neighbourG, node, node.score + 1)); //TODO: diagonal heuristic
		if (IsWalkable(neighbourH) == true)
			neighbours.Add(new Node(neighbourH, node, node.score + 1));
		if (IsWalkable(neighbourE) == true)
			neighbours.Add(new Node(neighbourE, node, node.score + 1));
		if (IsWalkable(neighbourF) == true)
			neighbours.Add(new Node(neighbourF, node, node.score + 1));

		return neighbours.ToArray();
	}

	public Node PopLowestHeuristic(List<Node> nodes)
	{
		Debug.AssertFormat(nodes != null && nodes.Count > 0, "");

		return null;
	}

	public bool Contains(Node node, List<Node> nodes)
	{
		foreach (Node value in nodes)
		{
			if (node.coord == value.coord)
				return true;
		}
		return false;
	}

	public Node[] ReconstructPath(Node node)
	{
		Debug.AssertFormat(node != null, "");

		List<Node> path = new List<Node>();
		do
		{
			path.Add(node);
			node = node.parent;
		}
		while (node != null);

		path.Reverse();
		return path.ToArray();
	}

	public MapTile GetTile(Coord coord)
	{
		if (coord.col < 0 || coord.col >= cached_map.grid.columns)
			return null;
		if (coord.row < 0 || coord.row >= cached_map.grid.rows)
			return null;

		return cached_map.grid.Get(coord.col, coord.row);
	}

	public bool IsWalkable(Coord coord)
	{
		MapTile tile = GetTile(coord);
		return tile != null && tile.type == '0';
	}

	public bool IsWall(Coord coord)
	{
		MapTile tile = GetTile(coord);
		return tile != null && tile.type == '1';
	}
}
