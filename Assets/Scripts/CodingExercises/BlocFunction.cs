using UnityEngine;

namespace CodingExercises
{
    public class BlocFunction : MonoBehaviour
    {
        public enum Function
        {
            Avancer,
            Flip,
            IfJoueur,
            IfObstacle,
            Attaquer,
            Boucle,
            EndIf
        }

        public Function function;
    }
}
