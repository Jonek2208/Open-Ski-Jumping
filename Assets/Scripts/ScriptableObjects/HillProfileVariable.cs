using UnityEngine;
using HillProfile;

[CreateAssetMenu]
public class HillProfileVariable : ScriptableObject
{
    private ProfileData value;

    public ProfileData Value { get => value; set => this.value = value; }
}