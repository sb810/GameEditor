using PlayerData;
using UnityEngine;
using UnityEngine.UI;

namespace CodingExercises
{
    public class CodingExerciseButtonHandler : MonoBehaviour
    {
        [SerializeField] private int currentExerciseNumber;
        [SerializeField] private Sprite currentExerciseSprite;
        private Button[] buttons;

        private int unlockedCodingLevel;

        void Start()
        {
            unlockedCodingLevel = PlayerDataManager.Data.unlockedCodingLevel;
            buttons = GetComponentsInChildren<Button>();

            for (int i = currentExerciseNumber - 1; i >= 0; i--)
            {
                buttons[i].interactable = true;
                buttons[i].image.raycastTarget = true;
                SpriteState state = buttons[i].spriteState;
                state.disabledSprite = state.pressedSprite;
            }

            buttons[currentExerciseNumber].image.overrideSprite = currentExerciseSprite;

            if (currentExerciseNumber == 0 && unlockedCodingLevel == 0) return;

            if (unlockedCodingLevel >= buttons.Length) unlockedCodingLevel = buttons.Length;

            for (int i = currentExerciseNumber + 1; i < unlockedCodingLevel; i++)
            {
                buttons[i].interactable = true;
                buttons[i].image.raycastTarget = true;
                SpriteState state = buttons[i].spriteState;
                state.disabledSprite = state.pressedSprite;
            }
        }
    }
}