using UnityEngine;
using System.Collections.Generic;

public class ArgumentDisplay : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    [SerializeField] private GameObject name;
    [SerializeField] private GameObject level;
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject icon;
    
}
