/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
    /// <summary>
    /// Moves the selected interactable 1 to 1 with the interactor. For example, if your interactor moves up and to the right, the selected interactable will also move up and to the right.
    /// </summary>
    public class StableMoveTargetProvider : MonoBehaviour, IMovementProvider
    {
        public IMovement CreateMovement()
        {
            return new StableMoveTarget();
        }
    }

    public class StableMoveTarget : IMovement
    {
        protected Vector3 originPoint, originRotationVector;
        KalmanFilterVector3 kalmanV3Origin, kalmanV3Rotation;
        CircularBuffer.CircularBuffer<Vector3> originHistory, rotationHistory;
        [Header("Kalman Filters")]
        [SerializeField] private int originHistoryWindow = 4;
        [SerializeField] private int rotationHistoryWindow = 10;
        [SerializeField] private float k1q = 0.1f, k2q = 0.0001f, k1r = 0.1f, k2r = 0.1f;
        public Pose Pose { get; private set; } = Pose.identity;
        public bool Stopped => true;

        public StableMoveTarget(){
            ResetKalmanFilters();
            List<string> datafields = new List<string>{"k1q","k1r","k2q","k2r","originWindow","rotationWindow"};
            List<Action<float>> callbacks = new List<Action<float>> { 
                (float newVal) => {k1q = newVal;},
                (float newVal) => {k1r = newVal;},
                (float newVal) => {k2q = newVal;},
                (float newVal) => {k2r = newVal;},
                (float newVal) => {originHistoryWindow = (int) newVal;},
                (float newVal) => {rotationHistoryWindow = (int) newVal;},
            };
            new FirebaseTracking(datafields, callbacks);
        }

        public void StopMovement()
        {
        }

        public void MoveTo(Pose target)
        {
            // Pose = target;
            Pose = getStablePose(target);
        }

        public void UpdateTarget(Pose target)
        {
            // Pose = target;
            Pose = getStablePose(target);
        }

        public void StopAndSetPose(Pose source)
        {
            Pose = source;
        }

        public void Tick()
        {
        }

        public void ResetKalmanFilters()
        {
            kalmanV3Origin = new KalmanFilterVector3(k1q, k1r);
            kalmanV3Rotation = new KalmanFilterVector3(k2q, k2r);

            originHistory = new CircularBuffer.CircularBuffer<Vector3>(originHistoryWindow);
            rotationHistory = new CircularBuffer.CircularBuffer<Vector3>(rotationHistoryWindow);
        }

        private Pose getStablePose(Pose target){
            Vector3 position = GetStableObjPosition(target.position);
            // Vector3 rotation = GetStableObjOrientation(target.rotation);
            // return new Pose(position, Quaternion.Euler(rotation));
            return new Pose(position, target.rotation);
        }

        private Vector3 GetStableObjPosition(Vector3 position)
        {
            originPoint = position;
            originHistory.PushBack(originPoint);
            originPoint = kalmanV3Origin.Update(originHistory.Back(), k1q, k1r);
            return originPoint;
        }

        private Vector3 GetStableObjOrientation(Quaternion rotation)
        {
            Debug.Log("k2q is " + k2q);
            originRotationVector = rotation.eulerAngles;
            rotationHistory.PushBack(originRotationVector);
            originRotationVector = kalmanV3Rotation.Update(rotationHistory.Back(), k2q, k2r);
            return originRotationVector;
        }
    }
}
