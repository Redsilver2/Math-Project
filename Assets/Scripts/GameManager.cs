using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float   cameraSpeed    = 5f;
        [SerializeField] private Vector3 cameraOffset   = new Vector3(1.40999997f, 2.22000003f, -4.4000001f);
        [SerializeField] private Vector3 cameraRotation = new Vector3(32.8600044f, 335.459991f, -5.08199264e-07f);



        [Space]
        [SerializeField] private Button applyForceButton;
        [SerializeField] private Button applyScalingButton;
        [SerializeField] private Button applyRotationButton;
        [SerializeField] private Button applyTranslationButton;

        [Space]
        [SerializeField] private TMP_InputField xComponent;
        [SerializeField] private TMP_InputField yComponent;
        [SerializeField] private TMP_InputField zComponent;
        [SerializeField] private TMP_InputField massInputField;

        [Space]
        [SerializeField] private Slider          durationSlider;
        [SerializeField] private TextMeshProUGUI durationText;


        [Space]
        [SerializeField] private TextMeshProUGUI formulaDisplayer;


        private Camera        mainCamera;
        private PhysicsEntity target;
        private int           currentSelectedIndex = 0;
        private float duration;

        private List<PhysicsEntity> physicsEntities;
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(Instance);

            mainCamera = Camera.main;
            physicsEntities = new List<PhysicsEntity>();    
        }

        private void Start()
        {
            SetButton(applyForceButton, () =>
            {
                if (target != null)
                    target.ApplyForce(new Vector(float.Parse(xComponent.text), float.Parse(yComponent.text), float.Parse(zComponent.text)), float.Parse(massInputField.text));
            }, "Apply Force");

            SetButton(applyTranslationButton, () =>
            {
                if (target != null)
                    target.ApplyTranslation(new Vector(float.Parse(xComponent.text), float.Parse(yComponent.text), float.Parse(zComponent.text)), duration);
            }, "Apply Translation");

            SetButton(applyScalingButton, () =>
            {
                if (target != null)
                    target.ApplyScaling(new Vector(float.Parse(xComponent.text), float.Parse(yComponent.text), float.Parse(zComponent.text)), duration);
            }, "Apply Scaling");

            SetButton(applyRotationButton, () =>
            {
                if(target != null)
                    target.ApplyRotation(new Vector(float.Parse(xComponent.text), float.Parse(yComponent.text), float.Parse(zComponent.text)), duration);
            }, "Apply Rotation");


            if (durationSlider != null)
            {
                durationSlider.onValueChanged.AddListener(value =>
                {
                    if(durationText != null)    
                    durationText.text = value.ToString();
                    duration = durationSlider.value;                   
                });

                durationSlider.wholeNumbers = true;
                durationSlider.minValue     = 1;
                durationSlider.maxValue     = 10;
                duration = durationSlider.value;
            }
        }

        private void LateUpdate()
        {
            if (physicsEntities.Count > 0)
            {
                if (currentSelectedIndex < 0) currentSelectedIndex = physicsEntities.Count - 1;
              
                target = physicsEntities[currentSelectedIndex];

                if (target != null && mainCamera != null)
                {
                    target.UpdatePhysics();
                    Transform transform = mainCamera.transform;

                    Vector3 targetPosition = target.transform.position;
                    Vector3 desiredPosition =  Vector3.right   * (targetPosition.x + (cameraOffset.x * target.transform.localScale.x)) +
                                               Vector3.up      * (targetPosition.y +  cameraOffset.y) +
                                               Vector3.forward * (targetPosition.z + (cameraOffset.z * target.transform.localScale.z));
     
                    transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * cameraSpeed);
                    transform.eulerAngles = cameraRotation;
                }
            }
        }

        public static Material GetMaterial(int index)
        {
             Material[] materials = Resources.LoadAll<Material>("Materials");
            if (index < 0) { index = 0; }
            else if (index >= materials.Length) { index = materials.Length - 1; }
            return materials[index];
        }

        public void AddPhysicsEntity(PhysicsEntity entity)
        {
            if (entity != null && !physicsEntities.Contains(entity))
                physicsEntities.Add(entity);
        }
        public void RemovePhysicsEntity(PhysicsEntity entity)
        {
            if (entity != null && physicsEntities.Contains(entity))
            {
                physicsEntities.Remove(entity);
                currentSelectedIndex = GetPhysicicsEntityIndex(entity) - 1;
            }
        }

        public static void SetButton(Button button, UnityAction action, string name)
        {
            if(button != null)
            {
                button.onClick.RemoveAllListeners();

                if(action != null)
                button.onClick.AddListener(action);

                button.GetComponentInChildren<TextMeshProUGUI>().text = name;
            }
        }

        private int GetPhysicicsEntityIndex(PhysicsEntity entity)
        {
            for (int i = 0; i < physicsEntities.Count; i++)
                if(physicsEntities[i] == entity)
                    return i;

            return -1;
        }

        public static Vector3 CreateVector(Transform transform, Vector v)
        {
            if(!transform) return Vector3.zero;

            return  transform.right   * v.x +
                    transform.up      * v.y +
                    transform.forward * v.z;
        }

        public static Vector3 CreateVector(Vector v)
        {
            return  Vector3.right   * v.x +
                    Vector3.up      * v.y +
                    Vector3.forward * v.z;
        }
    }
}

