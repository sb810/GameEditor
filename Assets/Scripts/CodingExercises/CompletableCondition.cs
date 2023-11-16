using TMPro;
using UnityEngine;

namespace CodingExercises
{
    public class CompletableCondition : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI checkbox;
        [SerializeField] private TextMeshProUGUI conditionLabel;
        [SerializeField] private TextMeshProUGUI conditionCount;

        public void Check()
        {
            checkbox.text = "[x]";
            checkbox.color = Color.green;
            conditionLabel.color = Color.green;
            if(conditionCount) conditionCount.color = Color.green;
        }
        
        public void MarkRed()
        {
            checkbox.text = "[ ]";
            checkbox.color = Color.red;
            conditionLabel.color = Color.red;
            if(conditionCount) conditionCount.color = Color.red;
        }

        public void Uncheck()
        {
            checkbox.text = "[ ]";
            checkbox.color = Color.white;
            conditionLabel.color = Color.white;
            if(conditionCount) conditionCount.color = Color.white;
        }

        public void UpdateConditionCount(int current, int max)
        {
            if (!conditionCount) return;
            conditionCount.text = "(" + current + "/" + max + ")";
        }
    }
}
