using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIcons : MonoBehaviour
{
    [System.Serializable]
    private struct NamedSprite
    {
        public string name;
        public Sprite sprite;
    }

    [SerializeField]
    private List<NamedSprite> sprites;

    private Dictionary<string, Sprite> spriteDict;

    [SerializeField]
    private GameObject iconPrefab;
    

    [SerializeField]
    private float _maxRowSize;
    [SerializeField]
    private float _spriteWidth;
    [SerializeField]
    private float _spriteHeight;
    // Start is called before the first frame update
    void Start()
    {
        spriteDict = new Dictionary<string, Sprite>();
        for(int i = 0; i < sprites.Count;i++)
        {
            spriteDict[sprites[i].name] = sprites[i].sprite;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void RearrangeStatuses(List<StatusEffect> statusEffects)
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        int numObjs = statusEffects.Count;

        int numRows = Mathf.CeilToInt((1.0f * numObjs) / _maxRowSize);

        float startY = _spriteHeight * numRows;
        float startX = -_spriteWidth * _maxRowSize / 2.0f;

        float currentY = startY;
        float currentX = startX;
        int i = 0;
        foreach(StatusEffect s in statusEffects)
        {
            GameObject iconClone = Instantiate(iconPrefab, transform);
            iconClone.GetComponent<SpriteRenderer>().sprite = spriteDict[s.Type];
            iconClone.transform.localPosition = new Vector3(currentX, currentY, 0);

            i++;
            if(i == _maxRowSize)
            {
                currentX = startX;
                currentY -= _spriteHeight;
                i = 0;
            } else
            {
                currentX += _spriteWidth;
            }
        }
    }
}
