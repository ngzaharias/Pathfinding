

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

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return col ^ row;
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