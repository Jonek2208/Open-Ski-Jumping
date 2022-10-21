using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Competition.Runtime;
using OpenSkiJumping.ScriptableObjects;
using OpenSkiJumping.UI;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.TVGraphics
{
    public interface ITVGraphics
    {
        void Show();
        void Hide();
        void InstantHide();
    }

    public abstract class JumpUIManager : MonoBehaviour, ITVGraphics
    {
        [SerializeField] protected FlagsData flagsData;
        [SerializeField] protected RuntimeResultsManager resultsManager;
        [SerializeField] protected RuntimeCompetitorsList competitors;
        public abstract void Hide();
        public abstract void Show();
        public abstract void InstantHide();

        protected Competitor GetCompetitorById(int id, int subround)
        {
            return competitors.competitors[resultsManager.OrderedParticipants[id].competitors[subround]];
        }
    }

    public abstract class PreJumpUIManager : JumpUIManager
    {
        [SerializeField] protected ImageCacher imageCacher;
        [SerializeField] private Image jumperImage;

        protected void LoadImage()
        {
            var id = resultsManager.GetCurrentJumperId();
            var path = competitors.competitors[id].imagePath;
            StartCoroutine(imageCacher.GetSpriteAsync(path, SetJumperImage));
        }

        private void SetJumperImage(Sprite value, bool succeeded)
        {
            if (!succeeded)
            {
                jumperImage.enabled = false;
                return;
            }

            jumperImage.enabled = true;
            jumperImage.sprite = value;
        }
    }

    public abstract class PostJumpUIManager : JumpUIManager
    {
        [SerializeField] protected JudgesMarkUI[] judgesMarks;
        [SerializeField] protected CompensationUI wind;
        [SerializeField] protected CompensationUI gate;
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
}