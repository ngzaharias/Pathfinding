

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