using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[AddComponentMenu("UIMan/Attach/UIFontPrefab")]
[ExecuteInEditMode]
public class UIFontPrefab : MonoBehaviour
{
    public GameObject font;
    private UIFont uiFont;
    public GameObject target;
    private Text text;

#if UNITY_EDITOR
    public bool refresh = false;
#endif

    private void Awake()
    {
        ProcessFont();
    }

    public void ProcessFont()
    {
        if (this.target == null)
        {
            this.text = GetComponent<Text>();
        }
        else
        {
            this.text = this.target.GetComponent<Text>();
        }

        if (this.text != null && this.font != null)
        {
            if (this.uiFont == null)
            {
                this.uiFont = this.font.GetComponent<UIFont>();
            }

            this.text.font = null;
            this.text.font = this.uiFont.font;
        }
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (this.refresh)
        {
            this.refresh = false;
            ProcessFont();
        }
    }

#endif
}