using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyberpunkJam.Units
{
    public class UnitParts : MonoBehaviour
    {
        private readonly float doublePi = Mathf.PI * 2f;

        [Title("Setup")]
        [SerializeField] private Part partPrefab;
        [SerializeField] private int partCount;
        [SerializeField] private float radius;

        [Title("Animation Parameters")]
        [SerializeField] private float partsMoveSpeed;
        [SerializeField] private float partShowDuration;
        [SerializeField] private float rotationDuration;
        [SerializeField] private float targetFollowSpeed;
        [SerializeField] private float delayBetweenStartPartsCreation;

        private Queue<Part> partsQueue = new();

        private Vector3 followTargetOffset = Vector3.zero;
        private float partsYPosition = 0f;

        private void Start()
        {
            Setup();
        }

        private void Update()
        {
            if (partsQueue.Count == 0)
            {
                return;
            }

            foreach (var part in partsQueue)
            {
                part.SetFollowTargetOffset(followTargetOffset);
            }
        }

        public UnitParts SetYAnglePositionToAllParts(float angle)
        {
            partsYPosition = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;

            foreach (var part in partsQueue)
            {
                var localPosition = part.transform.localPosition;
                var positionInCircle = new Vector3(localPosition.x, partsYPosition, localPosition.z);

                part.LocalMove(positionInCircle, partsMoveSpeed);
            }

            return this;
        }

        public UnitParts SetPartsOffset(Vector3 followTargetOffset)
        {
            this.followTargetOffset = followTargetOffset;

            return this;
        }

        public Part GetPart()
        {
            var part = partsQueue.Dequeue();
            part.transform.parent = null;

            RecalculatePartsPositions();

            return part;
        }

        public void ReturnPart(Part part)
        {
            SetPartSettingsAndAddToQueue(part);
        }

        public void CreateAndSetupPart()
        {
            SetPartSettingsAndAddToQueue(CreatePart());
        }

        private async void Setup()
        {
            for (int i = 0; i < partCount; i++)
            {
                CreateAndSetupPart();

                await UniTask.Delay(TimeSpan.FromSeconds(delayBetweenStartPartsCreation));
            }
        }

        private void SetPartSettingsAndAddToQueue(Part part)
        {
            RecalculatePartsPositionsForNewOne(out float radAngleForNewOne);

            partsQueue.Enqueue(part);

            part.SetTarget(transform);
            part.SetFollowTargetSpeed(targetFollowSpeed);
            part.Show(partShowDuration);

            SetPartPosition(part, radAngleForNewOne);
        }

        private Part CreatePart()
        {
            return Instantiate(partPrefab, transform);
        }

        private void RecalculatePartsPositions()
        {
            var radAngleStep = doublePi / partsQueue.Count;
            var radAngle = radAngleStep;

            foreach (var part in partsQueue)
            {
                SetPartPosition(part, radAngle);

                radAngle += radAngleStep;
            }
        }

        private void RecalculatePartsPositionsForNewOne(out float radAngleForNewOne)
        {
            var radAngleStep = doublePi / (partsQueue.Count + 1);
            var radAngle = radAngleStep;

            foreach (var part in partsQueue)
            {
                SetPartPosition(part, radAngle);

                radAngle += radAngleStep;
            }

            radAngleForNewOne = radAngle;
        }

        private async void SetPartPosition(Part part, float radAngle)
        {
            var positionInCircle = new Vector3(Mathf.Cos(radAngle) * radius, partsYPosition, Mathf.Sin(radAngle) * radius);

            await part.LocalMoveAsync(positionInCircle, partsMoveSpeed);
        }
    }
}