using UnityEngine;
using Microsoft.MixedReality.OpenXR;
#if WINDOWS_UWP
using Windows.Perception.Spatial;
#endif

namespace QRTracking.WindowsMR
{
    internal enum FrameTime { OnUpdate, OnBeforeRender }

    internal class SpatialGraphNode
    {
        public System.Guid Id { get; private set; }
#if WINDOWS_UWP
            private Windows.Perception.Spatial.SpatialCoordinateSystem CoordinateSystem = null;
#endif

        public static SpatialGraphNode FromStaticNodeId(System.Guid id)
        {
#if WINDOWS_UWP
                var coordinateSystem = Windows.Perception.Spatial.Preview.SpatialGraphInteropPreview.CreateCoordinateSystemForNode(id);
                return coordinateSystem == null ? null :
                    new SpatialGraphNode()
                    {
                        Id = id,
                        CoordinateSystem = coordinateSystem
                    };
#endif
            return null;
        }


        public bool TryLocate(FrameTime frameTime, out Pose pose)
        {
            pose = Pose.identity;

#if WINDOWS_UWP
                Quaternion rotation = Quaternion.identity;
                Vector3 translation = new Vector3(0.0f, 0.0f, 0.0f);
                    
                //System.IntPtr rootCoordnateSystemPtr = (PerceptionInterop.GetSceneCoordinateSystem(UnityEngine.Pose.identity) as SpatialCoordinateSystem).ToPointer();
                //Windows.Perception.Spatial.SpatialCoordinateSystem rootSpatialCoordinateSystem = 
                //    (Windows.Perception.Spatial.SpatialCoordinateSystem)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(rootCoordnateSystemPtr);

                Windows.Perception.Spatial.SpatialCoordinateSystem rootSpatialCoordinateSystem = PerceptionInterop.GetSceneCoordinateSystem(UnityEngine.Pose.identity) as SpatialCoordinateSystem;

                // Get the relative transform from the unity origin
                System.Numerics.Matrix4x4? relativePose = CoordinateSystem.TryGetTransformTo(rootSpatialCoordinateSystem);

                if (relativePose != null)
                {
                    System.Numerics.Vector3 scale;
                    System.Numerics.Quaternion rotation1;
                    System.Numerics.Vector3 translation1;
       
                    System.Numerics.Matrix4x4 newMatrix = relativePose.Value;

                    // Platform coordinates are all right handed and unity uses left handed matrices. so we convert the matrix
                    // from rhs-rhs to lhs-lhs 
                    // Convert from right to left coordinate system
                    newMatrix.M13 = -newMatrix.M13;
                    newMatrix.M23 = -newMatrix.M23;
                    newMatrix.M43 = -newMatrix.M43;

                    newMatrix.M31 = -newMatrix.M31;
                    newMatrix.M32 = -newMatrix.M32;
                    newMatrix.M34 = -newMatrix.M34;

                    System.Numerics.Matrix4x4.Decompose(newMatrix, out scale, out rotation1, out translation1);
                    translation = new Vector3(translation1.X, translation1.Y, translation1.Z);
                    rotation = new Quaternion(rotation1.X, rotation1.Y, rotation1.Z, rotation1.W);
                    pose = new Pose(translation, rotation);
                    return true;
                }
                else
                {
                    // Debug.Log("Id= " + id + " Unable to locate qrcode" );
                }
#endif // WINDOWS_UWP
            return false;
        }
    }
}