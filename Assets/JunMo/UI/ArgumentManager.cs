using UnityEngine;

public class ArgumentManager : MonoBehaviour
{
    [SerializeField] private ArgumentClick[] argumentClick = new ArgumentClick[3];
    [SerializeField] private ArgumentDisplay[] argumentDisplay = new ArgumentDisplay[3];

    void OnEnable()
    {
        for (int i = 0; i < argumentClick.Length; i++)
        {
            // argumentClick[i].OnClick += () => OnArgumentClicked(i);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < argumentClick.Length; i++)
        {
            // argumentClick[i].OnClick -= OnArgumentClicked(i);
        }
    }
}