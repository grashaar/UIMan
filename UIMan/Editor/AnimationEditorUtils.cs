using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace UnuGames
{
    public class AnimationEditorUtils : Editor
    {
        public static Animator GenerateAnimator(GameObject target, params string[] clipsName)
        {
            var config = EditorHelper.GetOrCreateScriptableObject<UIManConfig>(false);

            Animator anim = target.GetComponent<Animator>();

            if (anim == null)
                anim = Undo.AddComponent<Animator>(target);

            // Create folder
            var assetPath = Application.dataPath.Substring(Application.dataPath.IndexOf("/Assets") + 1) + "/" + config.animRootFolder;
            var rootFolderPath = string.Format("{0}{1}", assetPath, target.name);
            EditorUtils.CreatePath(rootFolderPath);

            // Create controller
            var controllerFilePath = string.Format("{0}/{1}.controller", rootFolderPath, target.name);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerFilePath);

            // Change the name of the first layer.
            AnimatorControllerLayer[] layers = controller.layers;
            layers[0].name = "Base Layer";
            controller.layers = layers;

            // Get the root state machine .
            AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

            // Set the default state.
            AnimatorState defaultState = rootStateMachine.AddState("Default");
            defaultState.motion = null;
            rootStateMachine.defaultState = defaultState;

            // Add states and transitions
            for (var i = 0; i < clipsName.Length; i++)
            {
                // Create empty clip and add motion
                var clip = new AnimationClip();
                var clipName = clipsName[i];
                var clipPath = string.Format("{0}/{1}.anim", rootFolderPath, clipName);
                clip.name = clipName;
                AssetDatabase.CreateAsset(clip, clipPath);

                // Add motion to controller
                var motion = AssetDatabase.LoadAssetAtPath(clipPath, typeof(AnimationClip)) as Motion;
                AnimatorState newState = rootStateMachine.AddState(clipName);
                newState.motion = motion;
                var smb = (UIAnimationState)newState.AddStateMachineBehaviour(typeof(UIAnimationState));

                if (clipName == UIManDefine.ANIM_SHOW)
                {
                    smb.Init(UIAnimationType.Show, true, false);
                }
                else if (clipName == UIManDefine.ANIM_HIDE)
                {
                    smb.Init(UIAnimationType.Hide, true, true);
                }
                else if (clipName == UIManDefine.ANIM_IDLE)
                {
                    smb.Init(UIAnimationType.Idle, true, false);
                }
            }

            anim.runtimeAnimatorController = controller;
            return anim;
        }
    }
}