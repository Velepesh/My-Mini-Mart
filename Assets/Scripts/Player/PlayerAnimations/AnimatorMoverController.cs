using UnityEngine;

public class AnimatorMoverController : MonoBehaviour
{
    public static class States
    {
        public const string IsMove = nameof(IsMove);
        public const string IsCarryMove = nameof(IsCarryMove);
        public const string IsEmpty = nameof(IsEmpty);
    }
}