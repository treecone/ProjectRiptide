using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    private ChunkLoader _chunkLoader;
    [SerializeField]
    private GameObject _mapBackground;
    [SerializeField]
    private Sprite _hidden;
    [SerializeField]
    private Sprite _hiddenIn;

    private MapPart[,] _map;
    private Vector2 _currentChunk;

    private Vector2 _mapTopLeftCorner;
    [SerializeField]
    private RectTransform _mapCursor;
    private float _mapPartLength;

    /// <summary>
    /// Sets up all the parts of the map
    /// </summary>
    public void SetUpMap()
    {
        _chunkLoader = GetComponent<ChunkLoader>();
        _map = new MapPart[_chunkLoader.XSize, _chunkLoader.YSize];
        for (int i = 0; i < _chunkLoader.XSize; i++)
        {
            for (int j = 0; j < _chunkLoader.YSize; j++)
            {
                //Set up map part
                GameObject mapPart = _mapBackground.transform.GetChild(i + (j * 5)).gameObject;
                _map[i, j] = new MapPart(mapPart.GetComponent<Image>(), _hidden, _hiddenIn, i, j);
                _map[i, j].SetHidden();
            }
        }
        _currentChunk = new Vector2(_chunkLoader.CurrentChunkPosition.y, _chunkLoader.CurrentChunkPosition.x);
        _map[(int)_currentChunk.x - 1, (int)_currentChunk.y - 1].SetHiddenIn();

        //Calculate stuff for setting cursor position later
        RectTransform topLeft = _mapBackground.transform.GetChild(0).GetComponent<RectTransform>();
        _mapPartLength = topLeft.rect.width;
        _mapTopLeftCorner = new Vector2(topLeft.anchoredPosition.x - (_mapPartLength / 2), topLeft.anchoredPosition.y + (_mapPartLength / 2));
    }

    /// <summary>
    /// Updates current chunk, changes display as necessary
    /// </summary>
    /// <param name="x">New x pos</param>
    /// <param name="y">New y pos</param>
    public void UpdateCurrentChunk(int x, int y)
    {
        _map[(int)_currentChunk.x - 1, (int)_currentChunk.y - 1].SetHidden();
        _currentChunk = new Vector2(x, y);
        _map[(int)_currentChunk.x - 1, (int)_currentChunk.y - 1].SetHiddenIn();
    }

    /// <summary>
    /// Sets position of map cursor based on player position in the world
    /// </summary>
    public void SetMapCursor()
    {
        Vector2 relativePlayerPos = _chunkLoader.GetRelativePlayerPosition();
        Vector2 cursorPos = new Vector2(relativePlayerPos.x * (250 / _chunkLoader.ChunkLength), relativePlayerPos.y * (250 / _chunkLoader.ChunkLength));
        _mapCursor.anchoredPosition = _mapTopLeftCorner + new Vector2(cursorPos.x, -cursorPos.y);
        _mapCursor.rotation = Quaternion.Euler(0, 0, -_chunkLoader.GetPlayerYEuler() - 90);
    }
}

public class MapPart
{
    private bool _hidden;
    private Image _mapPartImage;
    private Sprite _displaySprite;
    private Sprite _hiddenSprite;
    private Sprite _hiddenInSprite;
    private int _x;
    private int _y;

    public MapPart(Image mapPartImage, Sprite hiddenSprite, Sprite hiddenInSprite, int x, int y)
    {
        _hidden = true;
        _mapPartImage = mapPartImage;
        _hiddenSprite = hiddenSprite;
        _hiddenInSprite = hiddenInSprite;
        _x = x;
        _y = y;
    }

    public void SetHiddenIn()
    {
        _mapPartImage.sprite = _hiddenInSprite;
    }

    public void SetHidden()
    {
        _mapPartImage.sprite = _hiddenSprite;
    }

    public void Reveal()
    {
        _hidden = false;
        _mapPartImage.sprite = _displaySprite;
    }
}

