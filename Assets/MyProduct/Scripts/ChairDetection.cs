using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity;

public class ChairDetection : MonoBehaviour {

    public float kMinAreaForComplete = 50.0f;
    public float kMinHorizAreaForComplete = 25.0f;
    public float kMinWallAreaForComplete = 10.0f;

    public GameObject prefab = null;

    private bool triggerd = false;

    private void LateUpdate()
    {
        if (DoesScanMeetMinBarForCompletion)
        {
            SpatialUnderstanding.Instance.RequestFinishScan();
        }

        if (!triggerd && SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
        {
            triggerd = true;

            SpatialUnderstandingDllShapes.ShapeComponent shapeComponents = new SpatialUnderstandingDllShapes.ShapeComponent(
                new List<SpatialUnderstandingDllShapes.ShapeComponentConstraint>()
                {
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceHeight_Between(0.25f, 0.6f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceCount_Min(1),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_SurfaceArea_Min(0.035f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_IsRectangle(),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleLength_Between(0.1f, 0.5f),
                    SpatialUnderstandingDllShapes.ShapeComponentConstraint.Create_RectangleWidth_Between(0.1f, 0.4f),
                });

            // 定義の追加
            if (SpatialUnderstandingDllShapes.AddShape(
                "Chair",
                1,
                HoloToolkit.Unity.SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(shapeComponents),
                shapeComponents.ConstraintCount,
                HoloToolkit.Unity.SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(shapeComponents.Constraints)
                ) == 0)
            {
                Debug.LogError("Failed to create custom shape description");
            }

            // 解析の開始
            SpatialUnderstandingDllShapes.ActivateShapeAnalysis();

            // 解析結果を取得 ShapeResultは配列で512から変更するとesultsShape[i].positionがずれた?謎??
            SpatialUnderstandingDllShapes.ShapeResult[] resultsShape = new SpatialUnderstandingDllShapes.ShapeResult[512];

            SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());

            SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();

            // 検知された椅子の座標にオブジェクト配置
            IntPtr resultsShapePtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsShape);
            int resulltCount = SpatialUnderstandingDllShapes.QueryShape_FindShapeHalfDims(
                "Chair",
                resultsShape.Length,
                resultsShapePtr);

            if(0 < i)
            {
                Instantiate(prefab, resultsShape[i].position, Quaternion.LookRotation(alignment.BasisZ, alignment.BasisY));

                // メッシュの情報を非表示
                SpatialUnderstanding.Instance.GetComponent<SpatialUnderstandingCustomMesh>().DrawProcessedMesh = false;
            }
        }
    }

    private bool DoesScanMeetMinBarForCompletion
    {
        get
        {
            if ((SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Scanning) ||
                (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding))
            {
                return false;
            }

            IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
            if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
            {
                return false;
            }

            SpatialUnderstandingDll.Imports.PlayspaceStats stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
            if ((stats.TotalSurfaceArea > kMinAreaForComplete) ||
                (stats.HorizSurfaceArea > kMinHorizAreaForComplete) ||
                (stats.WallSurfaceArea > kMinWallAreaForComplete))
            {
                return true;
            }

            return false;
        }
    }
}
