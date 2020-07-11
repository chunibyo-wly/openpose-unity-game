using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using static GetPose;
using Debug = UnityEngine.Debug;

public class PoseControl : MonoBehaviour
{
    // ----------------------------------------------
    [Header("Option")]
    // ----------------------------------------------

    // debug 用 cube 用的是 position 这个需要按比例缩小
    public float cubeGlobalScale = 0.001f;

    // debug 用 cube与模型平移一小段距离
    public Vector3 cubeGlobalOffset = new Vector3(1.2f, 0, 0);

    public bool debugMode = true;

    public float frameRate = 30;

    public string url = "http://localhost:5000/getPose";

    public TakePhoto takePhoto;

    // ----------------------------------------------
    [Header("ReadOnly")]
    // ----------------------------------------------
    public Transform[] boneList;

    public Transform[] cubeList;

    /// <summary>
    /// 内建变量
    /// </summary>

    // 关节的总数目
    private const int JointNumber = 17;

    // 父亲关节
    private readonly int[] parentJoints = {1, 2, 4, 5, 7, 8, 11, 12, 14, 15};

    // 子关节
    private readonly int[] childrenJoints = {2, 3, 5, 6, 8, 10, 12, 13, 15, 16};

    private Quaternion[] initInv;

    private Quaternion[] initRot;

    private float timer = 0;

    public WebCamTexture backCam;

    private Vector2[] pose2D;

    private Vector3[] pose3D;

    private void SetPose(float[] inPose2D, float[] inPose3D)
    {
        for (int i = 0; i < pose3D.Length; ++i)
        {
            pose3D[i] = new Vector3(inPose3D[i * 3], -inPose3D[i * 3 + 1], -inPose3D[i * 3 + 2]);
        }
    }

    private void GetPoseFunction()
    {
        var tex = new Texture2D(backCam.width, backCam.height);
        tex.SetPixels(backCam.GetPixels());
        tex.Apply();
        var bytes = tex.EncodeToPNG();
        Singleton.Upload(url, bytes, SetPose);
    }

    // Start is called before the first frame update
    void Start()
    {
        backCam = takePhoto.webCamTexture;
        pose2D = new Vector2[JointNumber];
        pose3D = new Vector3[JointNumber];
        AddBones();
        if (debugMode)
            AddCubes();
        InitRotation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up"))
        {
            GetPoseFunction();
        }

        timer += Time.deltaTime;
        if (timer > (1 / frameRate))
        {
            timer = 0;
            if (debugMode)
            {
                UpdateCubes(pose3D);
                UpdateDebug();
            }

            // UpdatePose(pose);
        }
    }

    private static List<Vector3[]> ReadPosData(string filename)
    {
        var data = new List<Vector3[]>();

        var lines = new List<string>();
        var sr = new StreamReader(filename);
        while (!sr.EndOfStream)
        {
            lines.Add(sr.ReadLine());
        }

        sr.Close();

        try
        {
            foreach (var line in lines)
            {
                var line2 = line.Replace(",", "");
                var str = line2.Split(new[] {" "},
                    StringSplitOptions.RemoveEmptyEntries); // スペースで分割し、空の文字列は削除

                var vs = new Vector3[17];
                for (var i = 0; i < str.Length; i += 4)
                {
                    vs[i / 4] = new Vector3(-float.Parse(str[i + 1]), float.Parse(str[i + 3]),
                        -float.Parse(str[i + 2]));
                }

                data.Add(vs);
            }
        }
        catch (Exception)
        {
            Debug.Log("<color=blue>Error! Pos File is broken(" + filename + ").</color>");
            return null;
        }

        return data;
    }

    private void AddBones()
    {
        var animator = GetComponent<Animator>();
        boneList = new Transform[JointNumber];
        boneList[0] = animator.GetBoneTransform(HumanBodyBones.Hips);
        boneList[1] = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        boneList[2] = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        boneList[3] = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        boneList[4] = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        boneList[5] = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        boneList[6] = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        boneList[7] = animator.GetBoneTransform(HumanBodyBones.Spine);
        boneList[8] = animator.GetBoneTransform(HumanBodyBones.Neck);
        boneList[10] = animator.GetBoneTransform(HumanBodyBones.Head);
        boneList[11] = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        boneList[12] = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        boneList[13] = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        boneList[14] = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        boneList[15] = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        boneList[16] = animator.GetBoneTransform(HumanBodyBones.RightHand);
    }

    private void AddCubes()
    {
        cubeList = new Transform[JointNumber];
        for (var i = 0; i < JointNumber; i++)
        {
            // 创建一个 Cube 实例
            cubeList[i] = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            cubeList[i].localPosition = new Vector3(0, 0, 0);
            // 缩小 Cube
            cubeList[i].localScale = new Vector3(0.05f, 0.05f, 0.05f);
            // Cube 在 Hierarchy 的名字
            cubeList[i].name = i.ToString();
            // 挂载到 unity chan 上
            cubeList[i].parent = transform;
        }
    }

    private void InitRotation()
    {
        initInv = new Quaternion[JointNumber];
        initRot = new Quaternion[JointNumber];

        var initForward = GetNormalVector(boneList[7].position, boneList[4].position, boneList[1].position);
        initInv[0] = Quaternion.Inverse(Quaternion.LookRotation(initForward));
        initRot[0] = boneList[0].rotation;
        for (var i = 0; i < parentJoints.Length; i++)
        {
            var parentJoint = parentJoints[i];
            var childrenJoint = childrenJoints[i];
            initRot[parentJoint] = boneList[parentJoint].rotation;
            initInv[parentJoint] =
                Quaternion.Inverse(
                    Quaternion.LookRotation(boneList[parentJoint].position - boneList[childrenJoint].position,
                        initForward));
        }
    }

    private void UpdateCubes(IReadOnlyList<Vector3> pose)
    {
        for (var i = 0; i < JointNumber; i++)
        {
            // 这里的坐标是相对与父亲也就是 chan 的 root 而言的
            // 这个时候的 chan 的 root 和世界坐标的 root 是重合的
            cubeList[i].localPosition = pose[i] * cubeGlobalScale + cubeGlobalOffset;
        }
    }

    private static Vector3 GetNormalVector(Vector3 a, Vector3 b, Vector3 c)
    {
        var d1 = a - b;
        var d2 = a - c;
        var dd = Vector3.Cross(d1, d2);
        dd.Normalize();
        return dd;
    }

    private void UpdateDebug()
    {
        for (int i = 0; i < JointNumber; i++)
        {
            if (boneList[i] == null)
                continue;

            // x 红色
            Debug.DrawRay(boneList[i].position, boneList[i].right * 0.1f, Color.magenta);
            // y 绿色
            Debug.DrawRay(boneList[i].position, boneList[i].up * 0.1f, Color.green);
            // z 蓝色
            Debug.DrawRay(boneList[i].position, boneList[i].forward * 0.1f, Color.cyan);
        }
    }

    private void UpdatePose(IReadOnlyList<Vector3> pose)
    {
        var posForward = GetNormalVector(pose[7], pose[4], pose[1]);
        var rootRotation = transform.rotation;
        boneList[0].rotation = rootRotation * Quaternion.LookRotation(posForward) * initInv[0] * initRot[0];
        for (var i = 0; i < parentJoints.Length; i++)
        {
            var parentJoint = parentJoints[i];
            var childrenJoint = childrenJoints[i];
            boneList[parentJoint].rotation =
                rootRotation * Quaternion.LookRotation(pose[parentJoint] - pose[childrenJoint], posForward) *
                initInv[parentJoint] * initRot[parentJoint];
            Debug.DrawLine(boneList[parentJoint].position, boneList[childrenJoint].position, Color.blue);
        }
    }
}