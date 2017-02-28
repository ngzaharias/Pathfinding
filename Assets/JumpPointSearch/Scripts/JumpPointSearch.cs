using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
	public Coord coord;
	public Node parent;
	public float value;

	public int col { get { return coord.col; } }
	public int row { get { return coord.row; } }

	public Node(int column, int row, Node parent = null, float value = 0.0f)
	{
		this.coord = new Coord(column, row);
		this.parent = parent;
		this.value = value;
	}

	public Node(Coord coord, Node parent = null, float value = 0.0f)
	{
		this.coord = coord;
		this.parent = parent;
		this.value = value;
	}

	public static Coord Direction(Node from, Node to)
	{
		int x = Mathf.Clamp(to.col - from.col, -1, 1);
		int y = Mathf.Clamp(to.row - from.row, -1, 1);
		return new Coord(x, y);
	}
}

[RequireComponent(typeof(MapGenerator))]
public class JumpPointSearch : MonoBehaviour
{
	MapGenerator map;
	void Start ()
	{
		map = GetComponent<MapGenerator>();
	}

	public void Search()
	{
		Node node = new Node(0, 0);
		Coord goal = new Coord(map.grid.columns - 1, map.grid.rows - 1);
		IdentifySuccessors(node, node.coord, goal);
	}

	//Require: x: current node, s: start node, g: goal node
	//1: successors(x) ← ∅
	//2: neighbours(x) ← prune(x, neighbours(x))
	//3: for all n ∈ neighbours(x) do
	//4:	n ← jump(x, direction(x, n), s, g)
	//5:	add n to successors(x)
	//6: return successors(x)
	public Node[] IdentifySuccessors(Node current, Coord start, Coord goal)
	{
		List<Node> successors = new List<Node>();
		Node[] neighbours = Neighbours(current);
		foreach (Node neighbour in neighbours)
		{
			Coord direction = Node.Direction(current, neighbour);
			Node node = Jump(current, direction, start, goal);
			if (node != null) successors.Add(node);
		}
		return successors.ToArray();
	}

	public Node[] Neighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();
		if (node.parent == null)
		{
			Coord neighbourA = new Coord(node.col - 1, node.row + 0); // left
			Coord neighbourB = new Coord(node.col + 1, node.row + 0); // right
			Coord neighbourC = new Coord(node.col + 0, node.row + 1); // top
			Coord neighbourD = new Coord(node.col + 0, node.row - 1); // bottom
			if (IsWalkable(neighbourA) == true)
				neighbours.Add(new Node(neighbourA, node, 1));
			if (IsWalkable(neighbourB) == true)
				neighbours.Add(new Node(neighbourB, node, 1));
			if (IsWalkable(neighbourC) == true)
				neighbours.Add(new Node(neighbourC, node, 1));
			if (IsWalkable(neighbourD) == true)
				neighbours.Add(new Node(neighbourD, node, 1));

			Coord neighbourE = new Coord(node.col - 1, node.row + 1); // top-left
			Coord neighbourF = new Coord(node.col + 1, node.row + 1); // top-right
			Coord neighbourG = new Coord(node.col - 1, node.row - 1); // bottom-left
			Coord neighbourH = new Coord(node.col + 1, node.row - 1); // bottom-right
			if (IsWalkable(neighbourG) == true)
				neighbours.Add(new Node(neighbourG, node, 1)); //TODO: diagonal heuristic
			if (IsWalkable(neighbourH) == true)
				neighbours.Add(new Node(neighbourH, node, 1));
			if (IsWalkable(neighbourE) == true)
				neighbours.Add(new Node(neighbourE, node, 1));
			if (IsWalkable(neighbourF) == true)
				neighbours.Add(new Node(neighbourF, node, 1));
		}
		else
		{
			Coord current = node.coord;
			Coord direction = Node.Direction(node.parent, node);
			Coord neighbourA = current + direction;
			float heuristic = node.value + 1; //TODO: diagonal

			// horizontal / vertical
			if (Coord.IsDiagonal(direction) == false)
			{
				// add normal neighbours
				if (IsWalkable(neighbourA) == true)
					neighbours.Add(new Node(neighbourA, node, heuristic));

				// add forced neighbours
				Coord forcedB = new Coord(current.col + direction.row, current.row + direction.col);
				Coord forcedC = new Coord(current.col - direction.row, current.row - direction.col);
				Coord neighbourB = new Coord(neighbourA.col + direction.row, neighbourA.row + direction.col);
				Coord neighbourC = new Coord(neighbourA.col - direction.row, neighbourA.row - direction.col);
				if (IsWall(forcedB) == true && IsWalkable(neighbourB) == true)
					neighbours.Add(new Node(neighbourB, node, heuristic));
				if (IsWall(forcedC) == true && IsWalkable(neighbourC) == true)
					neighbours.Add(new Node(neighbourC, node, heuristic));
			}
			// diagonal
			else
			{
				// add normal neighbours
				Coord neighbourB = new Coord(neighbourA.col - direction.col, neighbourA.row);
				Coord neighbourC = new Coord(neighbourA.col, neighbourA.row - direction.row);
				if (IsWalkable(neighbourA) == true)
					neighbours.Add(new Node(neighbourA, node, heuristic));
				if (IsWalkable(neighbourB) == true)
					neighbours.Add(new Node(neighbourB, node, heuristic));
				if (IsWalkable(neighbourC) == true)
					neighbours.Add(new Node(neighbourC, node, heuristic));

				// add forced neighbours
				// TODO: only add one?
				Coord forcedD = new Coord(current.col - direction.col, current.row);
				Coord forcedE = new Coord(current.col, current.row - direction.row); 
				Coord neighbourD = new Coord(neighbourB.col - direction.col, neighbourB.row); 
				Coord neighbourE = new Coord(neighbourC.col, neighbourB.row - direction.row);
				if (IsWall(forcedD) == true && IsWalkable(neighbourD) == true)
					neighbours.Add(new Node(neighbourD, node, heuristic));
				if (IsWall(forcedE) == true && IsWalkable(neighbourE) == true)
					neighbours.Add(new Node(neighbourE, node, heuristic));
			}

		}

		return neighbours.ToArray();
	}

	//Require: x: initial node, ~d: direction, s: start node, g: goal node
	//1: n ← step(x, ~d)
	//2: if n is an obstacle or is outside the grid then
	//3:	return null
	//4: if n == g then
	//5:	return n
	//6: if ∃ n0 ∈ neighbours(n) s.t.n0 is forced then
	//7:	return n
	//8: if ~d is diagonal then
	//9:	for all i ∈ { 1, 2} do
	//10:		if jump(n, ~di, s, g) is not null then
	//11:			return n
	//12: return jump(n, ~d, s, g)
	public Node Jump(Node current, Coord direction, Coord start, Coord goal)
	{
		float heuristic = current.value + 1; //TODO: diagonal
		Node next = new Node(current.coord + direction, current, heuristic);
		if (IsWalkable(next.coord) == false)
			return null;
		if (next.coord == goal)
			return next;
		if (HasForcedNeighbour(current.coord, next.coord) == true)
			return next;

		if (Coord.IsDiagonal(direction) == true)
		{
			Coord horizontal = new Coord(direction.col, 0);
			if (Jump(next, horizontal, start, goal) != null)
				return next;

			Coord vertical = new Coord(0, direction.row);
			if (Jump(next, vertical, start, goal) != null)
				return next;
		}

		return Jump(next, direction, start, goal);
	}

	public bool HasForcedNeighbour(Coord parent, Coord current)
	{
		Coord direction = current - parent;
		direction.col = Mathf.Clamp(direction.col, -1, 1);
		direction.row = Mathf.Clamp(direction.row, -1, 1);
		Coord neighbourA = current + direction;

		// horizontal / vertical
		if (Coord.IsDiagonal(direction) == false)
		{
			Coord neighbourB = new Coord(neighbourA.col + direction.row, neighbourA.row + direction.col);
			Coord neighbourC = new Coord(neighbourA.col - direction.row, neighbourA.row - direction.col);

			Coord forcedB = new Coord(current.col + direction.row, current.row + direction.col);
			Coord forcedC = new Coord(current.col - direction.row, current.row - direction.col);
			if (IsWall(forcedB) == true && IsWalkable(neighbourB) == true)
				return true;
			if (IsWall(forcedC) == true && IsWalkable(neighbourC) == true)
				return true;
		}
		// diagonal
		else
		{
			Coord neighbourB = new Coord(neighbourA.col - direction.col, neighbourA.row);
			Coord neighbourC = new Coord(neighbourA.col, neighbourA.row - direction.row);
			Coord neighbourD = new Coord(neighbourB.col - direction.col, neighbourB.row);
			Coord neighbourE = new Coord(neighbourC.col, neighbourB.row - direction.row);

			Coord forcedD = new Coord(current.col - direction.col, current.row);
			Coord forcedE = new Coord(current.col, current.row - direction.row);
			if (IsWall(forcedD) == true && IsWalkable(neighbourD) == true)
				return true;
			if (IsWall(forcedE) == true && IsWalkable(neighbourE) == true)
				return true;
		}

		return false;
	}

	public MapTile GetTile(Coord coord)
	{
		if (coord.col < 0 || coord.col >= map.grid.columns)
			return null;
		if (coord.row < 0 || coord.row >= map.grid.rows)
			return null;

		return map.grid.Get(coord.col, coord.row);
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
