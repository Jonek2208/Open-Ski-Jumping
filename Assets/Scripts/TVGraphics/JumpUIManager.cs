
using UnityEngine;
using UnityEngine.UI;

public interface ITVGraphics
{
    void Show();
    void Hide();
    void InstantHide();
}

public abstract class PreJumpUIManager : MonoBehaviour, ITVGraphics
{
    public FlagsData flagsData;
    public RuntimeResultsManager resultsManager;
    public RuntimeParticipantsList participants;
    public RuntimeCompetitorsList competitors;
    public abstract void Hide();
    public abstract void Show();
    public abstract void InstantHide();
}

public abstract class PostJumpUIManager : MonoBehaviour, ITVGraphics
{
    public FlagsData flagsData;
    public RuntimeResultsManager resultsManager;
    public RuntimeParticipantsList participants;
    public RuntimeCompetitorsList competitors;
    public JudgesMarkUI[] judgesMarks;
    public CompensationUI wind;
    public CompensationUI gate;

    public abstract void Hide();
    public abstract void Show();
    public abstract void InstantHide();

}

public abstract class SpeedUIManager : MonoBehaviour, ITVGraphics
{
    public abstract void Hide();
    public abstract void Show();
    public abstract void InstantHide();

}

public abstract class ToBeatUIManager : MonoBehaviour, ITVGraphics
{
    public abstract void Hide();
    public abstract void Show();
    public abstract void InstantHide();

}



