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
    private InventoryMethods _invMethods;

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

        //_mapBackground.transform.parent.gameObject.SetActive(true);

        //Calculate stuff for setting cursor position later
        RectTransform topLeft = _mapBackground.transform.GetChild(0).GetComponent<RectTransform>();
        _mapPartLength = topLeft.rect.width;
        _mapTopLeftCorner = new Vector2(topLeft.anchoredPosition.x - (_mapPartLength / 2), topLeft.anchoredPosition.y + (_mapPartLength / 2));

        //_mapBackground.transform.parent.gameObject.SetActive(false);

        _invMethods.UpdateMap();
    }

    /// <summary>
    /// Updates current chunk, changes display as necessary
    /// </summary>
    /// <param name="x">New x pos</param>
    /// <param name="y">New y pos</param>
    public void UpdateCurrentChunk()
    {
        _invMethods.UpdateMap();
    }

    /// <summary>
    /// Exposes current chunk
    /// </summary>
    public void ExposeCurrentChunk()
    {
        _invMethods.ExposeMap();
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

    /// <summary>
    /// Returns if chunk has been exposed
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool ChunkExposed(int x, int y)
    {
        return _invMethods.ChunkExposed(x, y);
    }
}

