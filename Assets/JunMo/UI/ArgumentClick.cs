    using UnityEngine;
    using UnityEngine.EventSystems;
    using System;

    public class ArgumentClick : MonoBehaviour
    {
        [SerializeField]private ArgumentDisplay argumentDisplay;
        public void OnClick()
        {
            ArgumentManager.Instance.OnArgumentClick(argumentDisplay);
            argumentDisplay.ApplyArgument(); 
        }
    }
