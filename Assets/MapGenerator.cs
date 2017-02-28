using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public struct Coord
{
	public int col, row;

	public Coord(int column, int row)
	{
		this.col = column;
		this.row = row;
	}

	public override string ToString()
	{
		return col.ToString() + ":" + row.ToString();
	}

	public static Coord operator +(Coord lhs, Coord rhs)
	{
		return new Coord(lhs.col + rhs.col, lhs.row + rhs.row);
	}

	public static Coord operator -(Coord lhs, Coord rhs)
	{
		return new Coord(lhs.col - rhs.col, lhs.row - rhs.row);
	}

	public static bool operator ==(Coord lhs, Coord rhs)
	{
		return lhs.col == rhs.col && lhs.row == rhs.row;
	}

	public static bool operator !=(Coord lhs, Coord rhs)
	{
		return lhs.col != rhs.col || lhs.row != rhs.row;
	}

	public static bool IsDiagonal(Coord value)
	{
		return (value.col != 0) && (value.row != 0);
	}

	public static bool IsNaturalNeighbour(Coord lhs, Coord rhs)
	{
		return (rhs.col - lhs.col != 0) != (rhs.row - lhs.row != 0);
	}
}

[System.Serializable]
public class Grid<T>
{
	public T[] array;
	public int columns;
	public int rows;

	public Grid(int Columns, int Rows)
	{
		this.columns = Columns;
		this.rows = Rows;
		array = new T[columns * rows];
	}

	public Grid(int Columns, int Rows, T[] Array)
	{
		this.columns = Columns;
		this.rows = Rows;
		array = Array;
	}

	public T Get(int col, int row)
	{
		return array[col + (row * columns)];
	}

	public void Set(int col, int row, T value)
	{
		array[col + (row * columns)] = value;
	}
}

public class MapGenerator : MonoBehaviour
{
	public TextAsset map;
	public MapTile prefab;
	public Grid<MapTile> grid;

	void Awake()
	{
		grid = StringToGrid(map.text);
		transform.position = new Vector2(grid.columns / -2.0f, grid.rows /	-2.0f);
	}

	Grid<MapTile> StringToGrid(string text)
	{
		int columns = text.IndexOf('\n');
		string[] substrings = text.Split('\n');
		int rows = substrings.Length;

		Grid<MapTile> grid = new Grid<MapTile>(columns, rows);
		int count = 0;
		for (int i = 0; i < text.Length; ++i)
		{
			char character = text[i];
			if (character != '\n')
			{
				int column = count % columns;
				int row = count / columns;

				MapTile tile = Instantiate(prefab);
				tile.transform.SetParent(this.transform, false);
				tile.transform.position = new Vector2(column, row);
				tile.parent = this;
				tile.SetType(character);
				tile.gameObject.name = string.Format("[{0},{1}]", column, row);

				grid.Set(column, row, tile);
				count++;
			}
		}
		return grid;
	}
}
