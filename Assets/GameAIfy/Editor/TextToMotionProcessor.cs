using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using UnityEditor.Animations;
using UnityEditor.Formats.Fbx.Exporter;

namespace GameAIfySDK
{    
    public class TextToMotionProcessor : EditorWindow
    {
        private static SMPLData smplData = null;
        private static GameObject fbxAsset = null;
        private static float frameRate;        

        private static string tempAssetPath = "Assets/temp_fbx.fbx";

        private static string[] jointPaths = new string[]
            {
            "f_avg_Pelvis",
            "f_avg_Pelvis/f_avg_L_Hip",
            "f_avg_Pelvis/f_avg_R_Hip",
            "f_avg_Pelvis/f_avg_Spine1",
            "f_avg_Pelvis/f_avg_L_Hip/f_avg_L_Knee",
            "f_avg_Pelvis/f_avg_R_Hip/f_avg_R_Knee",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2",
            "f_avg_Pelvis/f_avg_L_Hip/f_avg_L_Knee/f_avg_L_Ankle",
            "f_avg_Pelvis/f_avg_R_Hip/f_avg_R_Knee/f_avg_R_Ankle",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3",
            "f_avg_Pelvis/f_avg_L_Hip/f_avg_L_Knee/f_avg_L_Ankle/f_avg_L_Foot",
            "f_avg_Pelvis/f_avg_R_Hip/f_avg_R_Knee/f_avg_R_Ankle/f_avg_R_Foot",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_Neck",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_L_Collar",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_R_Collar",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_Neck/f_avg_Head",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_L_Collar/f_avg_L_Shoulder",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_R_Collar/f_avg_R_Shoulder",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_L_Collar/f_avg_L_Shoulder/f_avg_L_Elbow",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_R_Collar/f_avg_R_Shoulder/f_avg_R_Elbow",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_L_Collar/f_avg_L_Shoulder/f_avg_L_Elbow/f_avg_L_Wrist",
            "f_avg_Pelvis/f_avg_Spine1/f_avg_Spine2/f_avg_Spine3/f_avg_R_Collar/f_avg_R_Shoulder/f_avg_R_Elbow/f_avg_R_Wrist"
            };        

        public static AnimationClip CreateAnimation(SMPLData data)
        {
            smplData = data;
            GenerateAnimationClipAndAddAnimationToFBX();

            return ConvertFBXToHumanoidAndExtractAnim();
        }

        private static void GenerateAnimationClipAndAddAnimationToFBX()
        {            
            if (smplData != null)
            {
                // Animation Clip ����
                AnimationClip clip = new AnimationClip();
                frameRate = smplData.frame_rate;
                clip.frameRate = frameRate;

                // �� ������ ���� ȸ�� Ű������ ����
                for (int joint = 0; joint < 22; joint++)
                {
                    AnimationCurve curveX = new AnimationCurve();
                    AnimationCurve curveY = new AnimationCurve();
                    AnimationCurve curveZ = new AnimationCurve();

                    for (int frame = 0; frame < smplData.frames.Count; frame++)
                    {
                        Vector3 axisAngle = new Vector3(smplData.frames[frame].joints[joint].x, smplData.frames[frame].joints[joint].y, smplData.frames[frame].joints[joint].z);
                        float angle = axisAngle.magnitude;
                        Vector3 axis = axisAngle.normalized;
                        Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);
                        Vector3 eulerAngles = rotation.eulerAngles;

                        // -180~180 ���� ����
                        eulerAngles.x = (eulerAngles.x > 180f) ? eulerAngles.x - 360f : eulerAngles.x;
                        eulerAngles.y = (eulerAngles.y > 180f) ? eulerAngles.y - 360f : eulerAngles.y;
                        eulerAngles.z = (eulerAngles.z > 180f) ? eulerAngles.z - 360f : eulerAngles.z;

                        // �ʿ信 ���� �� ����
                        eulerAngles.y *= -1;
                        eulerAngles.z *= -1;

                        float time = frame / frameRate;
                        curveX.AddKey(time, eulerAngles.x);
                        curveY.AddKey(time, eulerAngles.y);
                        curveZ.AddKey(time, eulerAngles.z);
                    }

                    // ���� ��� ���� (���� ������ �°� ����)
                    string jointPath = "f_avg_root/" + jointPaths[joint];
                    clip.SetCurve(jointPath, typeof(Transform), "localEulerAngles.x", curveX);
                    clip.SetCurve(jointPath, typeof(Transform), "localEulerAngles.y", curveY);
                    clip.SetCurve(jointPath, typeof(Transform), "localEulerAngles.z", curveZ);
                }

                if (fbxAsset == null)
                {
                    fbxAsset = Resources.Load<GameObject>("SMPL_f_unityDoubleBlends_lbs_10_scale5_207_v1.0.0");
                    
                    fbxAsset.name = "FBX_with_Anim";
                }

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fbxAsset);
                instance.name = "FBX_with_Anim";

                Animator animator = instance.GetComponent<Animator>();
                if (animator == null)
                {
                    animator = instance.AddComponent<Animator>();
                }

                // �ӽ� AnimatorController ���� �� Ŭ�� ���
                string controllerPath = "Assets/TempController.controller";
                AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                AnimatorControllerLayer layer = controller.layers[0];
                AnimatorStateMachine stateMachine = layer.stateMachine;
                AnimatorState state = stateMachine.AddState("AnimState");
                state.motion = clip;
                animator.runtimeAnimatorController = controller;

                ModelExporter.ExportObject(tempAssetPath, instance);
                Debug.Log("Exported FBX with animation to " + tempAssetPath);
                DestroyImmediate(instance);
                AssetDatabase.DeleteAsset(controllerPath);
            }
        }

        public static AnimationClip ConvertFBXToHumanoidAndExtractAnim()
        {            
            // FBX�� ModelImporter ��������
            ModelImporter importer = AssetImporter.GetAtPath(tempAssetPath) as ModelImporter;
            if (importer == null)
            {
                //Debug.LogError("Could not get ModelImporter for: " + fbxWithAnimPath);
                return null;
            }
            // Animation Type�� Humanoid�� ���� (�ʿ��� ��� �߰� ���� ����)
            importer.animationType = ModelImporterAnimationType.Human;
            // ������� ������ ���� ������Ʈ
            AssetDatabase.ImportAsset(tempAssetPath, ImportAssetOptions.ForceUpdate);

            // FBX�� ���� ���¿��� AnimationClip ã��
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(tempAssetPath);
            foreach (Object asset in subAssets)
            {
                if (asset is AnimationClip)
                {
                    AnimationClip clip = asset as AnimationClip;
                    AnimationClip newClip = Object.Instantiate(clip);
                    newClip.name = clip.name + "_Humanoid";

                    return newClip;
                }
            }

            return null;
        }
    }
}
