using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MapTile : MonoBehaviour
{
	public MapGenerator parent;
	public char type;
	public Color valid;
	public Color invalid;
	public Image image;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	public void SetType(char type)
	{
		this.type = type;
		image.color = (type == '0') ? valid : invalid;
	}

	public void ToggleType()
	{
		SetType((type == '0') ? '1' : '0');
	}

	public void MarkChecked()
	{
		if (image.color != Color.white || type != '1')
			image.color = Color.green;
	}
}
