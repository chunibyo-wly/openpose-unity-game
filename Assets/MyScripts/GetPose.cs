using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Models;
using Proyecto26;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class GetPose
{
    private static GetPose _singleton;

    private string pose2d;

    public static GetPose Singleton
    {
        get
        {
            if (_singleton == null) _singleton = new GetPose();
            return _singleton;
        }
    }

    [Serializable]
    private class Pose
    {
        public float[] pose2D;
        public float[] pose3D;
    }

    public delegate void MyCallBackFunction(float[] pose2D, float[] pose3D);

    public void Upload(string mainUrl, byte[] bytesData, MyCallBackFunction setPose)
    {
        var form = new WWWForm();
        form.AddBinaryData("file", bytesData);

        var requestHelper = new RequestHelper
        {
            Uri = mainUrl,
            FormData = form
        };
        RestClient.Post<Pose>(requestHelper).Then(response =>
        {
            setPose(response.pose2D, response.pose3D);
        });
    }
}