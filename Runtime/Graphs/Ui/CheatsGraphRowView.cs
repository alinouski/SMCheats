using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    public class CheatsGraphRowView : MonoBehaviour
    {
        [SerializeField]
        private Text label;
        
        [SerializeField]
        private Text value;

        [SerializeField]
        private Image sampleColor;
        
        [SerializeField]
        private GameObject sampleColorRoot;

        public void SetName(string name)
        {
            label.text = name;
        }
        
        public void SetValue(string sampleValue)
        {
            value.text = sampleValue;
        }

        public void SetSampleColor(Color? color)
        {
            var colorHasValue = color.HasValue;
            
            sampleColorRoot.gameObject.SetActive(colorHasValue);
            
            if (colorHasValue)
            {
                sampleColor.color = color.Value;
            }
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}