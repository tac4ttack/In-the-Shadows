using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial card", menuName = "In The Shadows/Tutorial Card")]
public class TutorialCard : ScriptableObject
{
    [SerializeField] public string Title;
    [SerializeField] [Multiline] public string Content;
    [SerializeField] public Sprite Picture;
}