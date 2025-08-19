using UnityEngine;

namespace RedsUtils.LoadingSystem
{
    public abstract class LoadableBehavior : MonoBehaviour, ILoadable
    {
        public float Progress { get; protected set; } = 0f;
        public bool IsDone { get; protected set; } = false;

        public virtual void BeginLoad()
        {
            LoadingEvents.RaiseLoadableStarted(this);
            StartCoroutine(DoLoadRoutine());
        }

        protected abstract System.Collections.IEnumerator DoLoadRoutine();

        protected void ReportProgress(float progress)
        {
            Progress = Mathf.Clamp01(progress);
            LoadingEvents.RaiseLoadableProgress(this, Progress);

            if (Progress >= 1f && !IsDone)
            {
                IsDone = true;
                LoadingEvents.RaiseLoadableCompleted(this);
            }
        }
    }
}