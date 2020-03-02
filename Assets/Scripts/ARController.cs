//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Controls the HelloAR example.
/// </summary>
public class ARController: MonoBehaviour
{
    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR
    /// background).
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// A prefab to place when the house image has been found.
    /// </summary>
    public GameEntity HousePrefab;

    /// <summary>
    /// The rotation in degrees need to apply to prefab when it is placed.
    /// </summary>
    private const float k_PrefabRotation = 180;

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error,
    /// otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    private Dictionary<int, GameEntity> m_Entities = new Dictionary<int, GameEntity>();

    private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();

    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake()
    {
        // Enable ARCore to target 60fps camera capture frame rate on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        _UpdateApplicationLifecycle();

        _TrackImage();

    }

    private void _TrackImage()
    {
        // Get updated augmented images for this frame.
        Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

        // Create visualizers and anchors for updated augmented images that are tracking and do
        // not previously have a visualizer. Remove visualizers for stopped images.
        foreach (var image in m_TempAugmentedImages)
        {
            GameEntity entity = null;
            m_Entities.TryGetValue(image.DatabaseIndex, out entity);
            if (image.TrackingState == TrackingState.Tracking && entity == null)
            {
                // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                Anchor anchor = image.CreateAnchor(image.CenterPose);
                entity = Instantiate(HousePrefab, anchor.transform);
                //visualizer.Image = image;
                m_Entities.Add(image.DatabaseIndex, entity);
            }
            else if (image.TrackingState == TrackingState.Stopped && entity != null)
            {
                m_Entities.Remove(image.DatabaseIndex);
                GameObject.Destroy(entity.gameObject);
            }
        }
    }

    //private void _Tap()
    //{// If the player has not touched the screen, we are done with this update.
    //    Touch touch;
    //    if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
    //    {
    //        return;
    //    }

    //    // Should not handle input if the player is pointing on UI.
    //    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
    //    {
    //        return;
    //    }

    //    // Raycast against the location the player touched to search for planes.
    //    TrackableHit hit;
    //    TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
    //        TrackableHitFlags.FeaturePointWithSurfaceNormal;

    //    if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
    //    {
    //        // Use hit pose and camera pose to check if hittest is from the
    //        // back of the plane, if it is, no need to create the anchor.
    //        if ((hit.Trackable is DetectedPlane) &&
    //            Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
    //                hit.Pose.rotation * Vector3.up) < 0)
    //        {
    //            Debug.Log("Hit at back of the current DetectedPlane");
    //        }
    //        else
    //        {
    //            // Choose the prefab based on the Trackable that got hit.
    //            GameObject prefab;
    //            if (hit.Trackable is FeaturePoint)
    //            {
    //                prefab = GameObjectPointPrefab;
    //            }
    //            else if (hit.Trackable is DetectedPlane)
    //            {
    //                DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;
    //                if (detectedPlane.PlaneType == DetectedPlaneType.Vertical)
    //                {
    //                    prefab = GameObjectVerticalPlanePrefab;
    //                }
    //                else
    //                {
    //                    prefab = GameObjectHorizontalPlanePrefab;
    //                }
    //            }
    //            else
    //            {
    //                prefab = GameObjectHorizontalPlanePrefab;
    //            }

    //            // Instantiate prefab at the hit pose.
    //            var gameObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

    //            // Compensate for the hitPose rotation facing away from the raycast (i.e.
    //            // camera).
    //            gameObject.transform.Rotate(0, k_PrefabRotation, 0, Space.Self);

    //            // Create an anchor to allow ARCore to track the hitpoint as understanding of
    //            // the physical world evolves.
    //            var anchor = hit.Trackable.CreateAnchor(hit.Pose);

    //            // Make game object a child of the anchor.
    //            gameObject.transform.parent = anchor.transform;
    //        }
    //    }
    //}

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to
        // appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}

