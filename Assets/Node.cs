using UnityEngine;

public class Node
{
	public Coord coord;
	public Node parent;
	public float score;

	public int col { get { return coord.col; } }
	public int row { get { return coord.row; } }

	public Node(int column, int row, Node parent = null, float value = 0.0f)
	{
		this.coord = new Coord(column, row);
		this.parent = parent;
		this.score = value;
	}

	public Node(Coord coord, Node parent = null, float value = 0.0f)
	{
		this.coord = coord;
		this.parent = parent;
		this.score = value;
	}

	public static Coord Direction(Node from, Node to)
	{
		int x = Mathf.Clamp(to.col - from.col, -1, 1);
		int y = Mathf.Clamp(to.row - from.row, -1, 1);
		return new Coord(x, y);
	}
}