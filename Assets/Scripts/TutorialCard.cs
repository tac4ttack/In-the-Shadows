using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial card", menuName = "In The Shadows/Tutorial Card")]
public class TutorialCard : ScriptableObject
{
    [SerializeField] public string Title;
    [SerializeField] [TextArea(10, 20)] public string Content;
    [SerializeField] public Sprite Picture;
}