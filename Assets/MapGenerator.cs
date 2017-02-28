using UnityEngine;

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
