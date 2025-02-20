using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sharklib.ShineShader {
    public class ShineControl : MonoBehaviour {

        const string nameTime = "_TimeControl";
        const string nameFreq = "_WaveFreq";
        const string namePause = "_WavePause";
        const string nameWidth = "_WaveWidth";
        const string nameFade = "_WaveFade";

        [System.Serializable]
        public class UnityShineLoopEvent : UnityEvent<int, float> { }

        [Tooltip("Animates the Shine Shader on this sprite.")]
        public SpriteRenderer targetSprite;
        [Tooltip("Animates the Shine Shader on this UI Image. Only works if no sprite is set.")]
        public Image targetImage;

        [Tooltip("This is used to ease the animation of the shine.")]
        public AnimationCurve animCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        [Tooltip("If this is not 0, the component will trigger that many loops. Negative numbers create infinite lopos.")]
        public int numLoopsOnEnable = 3;

        [Header("Shine Events")]
        public UnityShineLoopEvent onShineShow;
        public UnityShineLoopEvent onShinePause;
        public UnityEvent onLoopsEnded;

        Material mat;
        int currentLoops;

        float loopShowTime;
        float loopDuration;
        float boundLow;
        float boundHigh;

        void Awake() {
            bool isSetup = SetupMaterial();

            // Material was setup? Then set the sprite to not show any visuals
            if (isSetup)
                SetTime(GetValue(0f));
        }

        // Stop all coroutines, if we have any
        private void OnDisable() {
            if (currentLoops != 0)
                StopAllCoroutines();

            currentLoops = 0;
        }

        // Check if we need to trigger a shine
        private void OnEnable() {
            if (numLoopsOnEnable != 0 && currentLoops == 0)
                TriggerShine(numLoopsOnEnable);
        }

        #region -- Material Setup -----------------

        // Get our material copy (so it's not shared) apply it to the object and set our defaults
        bool SetupMaterial() {
            if (mat == null) {
                mat = GetMaterial();
                ApplyMaterial(mat);
                SetMaterialValues();
            }

            return mat != null;
        }

        Material GetMaterial() {
            if (targetSprite == null && targetImage == null) {
                Debug.LogWarning("[ShinyShaderControl] No targetRenderer or targetImage set!");
                return null;
            }
            else if (targetSprite != null) {
                if (IsLegalMaterial(targetSprite.material))
                    return new Material(targetSprite.material) {
                        name = "ShineControlled (" + targetSprite.material.name + ")",
                        hideFlags = HideFlags.HideAndDontSave
                    };
            }
            else if (targetImage != null) {
                if (IsLegalMaterial(targetImage.materialForRendering))
                    return new Material(targetImage.materialForRendering) {
                        name = "ShineControlled (" + targetImage.materialForRendering.name + ")",
                        hideFlags = HideFlags.HideAndDontSave
                    };
            }
            else {
                Debug.LogWarning("[ShinyShaderControl]  No shiny shader material found on "
                    + (targetSprite != null ? targetSprite.name : (targetImage != null ? targetImage.name : "NULL"))
                    + "!");
                return null;
            }

            return null;
        }

        bool IsLegalMaterial(Material mat) {
            bool legal = mat.HasProperty(nameTime);

            if (!legal)
                Debug.LogWarning("[ShinyShaderControl] Material on tragetRenderer or targetImage is not a legal Shiny Shader!\n(Does not contain a property named " + nameTime + ")");

            return legal;
        }

        // We have created an instance of the material and we apply it here
        void ApplyMaterial(Material mat) {
            if (mat == null)
                return;
            
            if (targetSprite != null) {
                targetSprite.material = mat;
            }
            else if (targetImage != null) {
                targetImage.material = mat;
            }
        }

        void SetMaterialValues() {
            loopShowTime = GetShowDuration();
            loopDuration = loopShowTime + GetPauseDuration(loopShowTime);
            boundLow = -GetBoundOffset();
            boundHigh = 1 - boundLow;
        }

        float GetShowDuration() {
            return 1f / mat.GetFloat(nameFreq);
        }

        float GetPauseDuration(float showTime) {
            return showTime * mat.GetFloat(namePause);
        }

        float GetBoundOffset() {
            return mat.GetFloat(nameWidth) + mat.GetFloat(nameFade);
        }

        #endregion

        // Manually trigger the shine
        public void TriggerShine(int loops) {
            // GameObject not enabled? Can't run Corutines? Warning and stop.
            if (!gameObject.activeInHierarchy) {
                Debug.LogWarning("[ShinyShaderControl] Trying to enable but the gameObject " + gameObject.name + " is not active!");
                return;
            }

            // Are we running a shine? Stop that.
            if (currentLoops != 0)
                StopAllCoroutines();
            // We're not, then ensure materials are set up
            else if (SetupMaterial() == false)
                return;

            currentLoops = loops; // this first to prevent onEnabled triggering twice through activateOnEnable
            StartCoroutine(DoLoops());
        }

        IEnumerator DoLoops() {
            float time;
            int elapsedLoops = 0;

            // Negative numLoops will just go forever
            while (currentLoops != 0) {
                time = 0f;
                onShineShow.Invoke(elapsedLoops, loopShowTime);

                while (time < loopShowTime) {
                    time += Time.deltaTime;
                    SetTime(GetValue(time / loopShowTime));
                    yield return null;
                }

                onShinePause.Invoke(elapsedLoops, loopDuration - loopShowTime);
                SetTime(boundLow);

                while (time < loopDuration) {
                    time += Time.deltaTime;
                    yield return null;
                }
                elapsedLoops++;

                if (currentLoops > 0)
                    currentLoops--;
            }

            onLoopsEnded.Invoke();
        }

        // Take a time value from 0 to 1 and lerp it between the low and high bounds to account for fade and width
        float GetValue(float value) {
            return Mathf.Lerp(boundLow, boundHigh, animCurve.Evaluate(value));
        }

        // Apply the time to our material. If we have a sprite or an unmasked image, we can use our material directly
        // If the image is maskable AND masked then we need to use the materialForRendering (which is in Graphic and does some work)
        // This is because the mask has made a copy of our material and we can't get to it otherwise
        void SetTime(float time) {
            if (targetSprite == null && targetImage != null && targetImage.maskable && targetImage.materialForRendering.GetFloat("_StencilComp") == 3)
                targetImage.materialForRendering.SetFloat(nameTime, time);
            else 
                mat.SetFloat(nameTime, time);
        }

#if UNITY_EDITOR
        // When the component is applied, check if a target is available
        void Reset() {
            targetSprite = GetComponent<SpriteRenderer>();
            targetImage = targetSprite == null ? GetComponent<Image>() : null;
        }
#endif

    }
}