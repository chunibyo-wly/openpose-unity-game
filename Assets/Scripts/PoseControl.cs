using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PoseControl : MonoBehaviour
{
    // ----------------------------------------------
    [Header("Option")]
    // ----------------------------------------------
    public float cubeGlobalScale = 0.001f;

    public Vector3 cubeGlobalOffset = new Vector3(1.2f, 0, 0);

    // ----------------------------------------------
    [Header("ReadOnly")]
    // ----------------------------------------------
    public Transform[] boneList;

    public Transform[] cubeList;

    private const int JointNumber = 17;

    /// <summary>
    /// 假数据接口
    /// </summary>
    private List<Vector3[]> readInPose;

    [SerializeField] private int currentFrame = 0;

    private Vector3[] GetPose()
    {
        var pose = readInPose[currentFrame];
        currentFrame += 1;
        return pose;
    }

    // Start is called before the first frame update
    void Start()
    {
        readInPose = ReadPosData("Assets\\Scripts\\pos_sample1.txt");
        AddBones();
        AddCubes();
    }

    // Update is called once per frame
    void Update()
    {
        var pose = GetPose();
        UpdateCubes(pose);
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

    private void UpdateCubes(IReadOnlyList<Vector3> pose)
    {
        for (var i = 0; i < JointNumber; i++)
        {
            // 这里的坐标是相对与父亲也就是 chan 的 root 而言的
            // 这个时候的 chan 的 root 和世界坐标的 root 是重合的
            cubeList[i].localPosition = pose[i] * cubeGlobalScale + cubeGlobalOffset;
        }
    }
}