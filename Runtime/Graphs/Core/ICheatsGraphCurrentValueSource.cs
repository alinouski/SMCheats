using UnityEngine;

namespace SMCheats
{
    public interface ICheatsGraphCurrentValueSource
    {
        Color? Color { get; }
        string Name { get; }
        float GetValue();
        string GetValueString();
    }
}