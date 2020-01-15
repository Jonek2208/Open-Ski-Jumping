using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Replay")]
public class Replay : ScriptableObject
{
    [SerializeField]
    private List<ReplayFrame> frames;

    public List<ReplayFrame> Frames { get => frames; set => frames = value; }

    public void AddFrame(Transform[] value)
    {
        this.frames.Add(new ReplayFrame(value));
    }
}