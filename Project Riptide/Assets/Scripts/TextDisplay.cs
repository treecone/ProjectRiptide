using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject _textDisplayPanel;
    [SerializeField]
    private GameObject _textPrefab;

    private Queue<TextObject> _waitingTextQueue;

    private List<TextObject> _displayText;

    private const float STARTING_HEIGHT = -644;
    private const float BETWEEN_TEXT_DISTANCE = 300;

    // Start is called before the first frame update
    void Start()
    {
        _waitingTextQueue = new Queue<TextObject>();
        _displayText = new List<TextObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            AddText("+25 Wood", Color.black);
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            AddText("+3 Steel", Color.green);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            AddText("+9 Seaweed", Color.red);
        }

        if(_displayText.Count < 5 && _waitingTextQueue.Count > 0)
        {
            //Add new text from queue to display
            TextObject textObj = _waitingTextQueue.Dequeue();
            GameObject textGameObj = Instantiate(_textPrefab, _textDisplayPanel.transform);
            textGameObj.GetComponent<TextMeshProUGUI>().text = textObj.text;
            textGameObj.GetComponent<TextMeshProUGUI>().color = textObj.color;
            textObj.SetObject(textGameObj);
            _displayText.Add(textObj);
            SetTextPositions();
        }

        for(int i = 0; i < _displayText.Count; i++)
        {
            //UPDATE EACH TEXT
            if(_displayText[i].Update())
            {
                _displayText.RemoveAt(i);
                i--;
                SetTextPositions();
            }
        }
    }

    /// <summary>
    /// Sets the position of all text objects being displayed
    /// </summary>
    public void SetTextPositions()
    {
        for(int i = 0; i < _displayText.Count; i++)
        {
            _displayText[i].gameObject.transform.localPosition = new Vector3(0, STARTING_HEIGHT + BETWEEN_TEXT_DISTANCE * i, 0);
        }
    }

    public void AddText(string text, Color32 color)
    {
        _waitingTextQueue.Enqueue(new TextObject(text, color));
    }

}

public class TextObject
{
    public string text;
    public Color32 color;
    public GameObject gameObject;

    private TextMeshProUGUI _textMesh;
    private float _currAlpha = 255;

    private const float DISPLAY_TIME = 3.0f;
    private float _currTime;

    public TextObject(string text, Color32 color)
    {
        this.text = text;
        this.color = color;
    }

    public void SetObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
        _textMesh = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public bool Update()
    {
        if(_currTime >= DISPLAY_TIME)
        {
            _currAlpha -= 255 * Time.deltaTime;
            if(_currAlpha < 0)
            {
                //Destroy object
                GameObject.Destroy(gameObject);
                return true;
            }
            else
            {
                _textMesh.color = new Color(_textMesh.color.r, _textMesh.color.g, _textMesh.color.b, _currAlpha / 255f);
            }
        }

        _currTime += Time.deltaTime;

        return false;
    }
}