using Game.Managers;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsEntity : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI informationDisplayer;

    private List<VectorVisualizerType> visualizerTypesAllowed;
    private CancellationTokenSource tokenSource;

    private UnityEvent onUpdatePhysics;
    private Vector2 startingPosition = new Vector3(-8.30000019f, -3.93000007f, 0);

    private float[] directions;

    public void Awake()
    {
        transform.position = startingPosition;

        directions = new float[3];
        visualizerTypesAllowed = new List<VectorVisualizerType>();
        onUpdatePhysics = new UnityEvent();

        foreach (MultipleVectorVisualizer visualizer in MultipleVectorVisualizer.SetAndGetVisualizers(transform))
        {
            onUpdatePhysics.AddListener(() =>
            {
                visualizer.gameObject.SetActive(visualizerTypesAllowed.Contains(visualizer.VisualizerType));

                switch (visualizer.VisualizerType)
                {
                    case VectorVisualizerType.X:
                        visualizer.UpdateVisuals((int)Mathf.Sign(directions[0]));
                        visualizer.SetOffsetMultiplier(transform.localScale.x);
                        break;

                    case VectorVisualizerType.Y:
                        visualizer.UpdateVisuals((int)Mathf.Sign(directions[1]));
                        visualizer.SetOffsetMultiplier(transform.localScale.y);
                        break;

                    case VectorVisualizerType.Z:
                        visualizer.UpdateVisuals((int)Mathf.Sign(directions[2]));
                        visualizer.SetOffsetMultiplier(transform.localScale.z);
                        break;
                }
            });


            visualizer.gameObject.layer = LayerMask.GetMask("3D");
        }
    }

    private void Start() => GameManager.Instance.AddPhysicsEntity(this);

    public void UpdatePhysics()
    {
        onUpdatePhysics.Invoke();

        if (informationDisplayer != null)
            informationDisplayer.text = $"Position ({(int)transform.position.x},{(int)transform.position.y}, {(int)transform.position.z})\n" +
                                        $"Rotation ({(int)transform.localEulerAngles.x},{(int)transform.localEulerAngles.y}, {(int)transform.localEulerAngles.z})\n" +
                                        $"Scale    ({(int)transform.localScale.x},{(int)transform.localScale.y}, {(int)transform.localScale.z})";
    }

    private void OnDisable()
    {
        GameManager.Instance.RemovePhysicsEntity(this);
    }

    private void SetDirections(float x, float y, float z)
    {
        directions[0] = x;
        directions[1] = y;
        directions[2] = z;

        foreach (var direction in directions)
            Debug.Log(Mathf.Sign(direction));
    }

    private void AddVisualizerTypeAllowed(VectorVisualizerType visualizerType)
    {
        if (!visualizerTypesAllowed.Contains(visualizerType))
            visualizerTypesAllowed.Add(visualizerType);
    }

    private void RemoveVisualizerTypeAllowed(VectorVisualizerType visualizerType)
    {
        if (visualizerTypesAllowed.Contains(visualizerType))
            visualizerTypesAllowed.Remove(visualizerType);
    }


    public async void ApplyForce(Vector acceleration, float mass)
    {
        ResetToken();
        acceleration.Multiply(mass);
        visualizerTypesAllowed.Clear();
        await ForceUpdate(acceleration, tokenSource.Token);
    }

    public async void ApplyScaling(Vector v, float duration)
    {
        ResetToken();
        visualizerTypesAllowed.Clear();
        await ScalingUpdate(v, duration, tokenSource.Token);   
    }

    public async void ApplyRotation(Vector v, float duration)
    {
        ResetToken();
        visualizerTypesAllowed.Clear();
        await RotatiomUpdate(v, duration, tokenSource.Token);
    }

    public async void ApplyTranslation(Vector v, float duration)
    {
        ResetToken();
        visualizerTypesAllowed.Clear();
        await TranslaionUpdate(v, duration, tokenSource.Token);
    }

    private async Awaitable RotatiomUpdate(Vector v, float duration, CancellationToken token)
    {
        float t = 0f;
        Vector3 currentRotation = transform.localEulerAngles;
        Vector3 desiredRotation;

        if (v.x != 0)
            AddVisualizerTypeAllowed(VectorVisualizerType.X);

        if (v.y != 0)
            AddVisualizerTypeAllowed(VectorVisualizerType.Y);


        if (v.z != 0)
            AddVisualizerTypeAllowed(VectorVisualizerType.Z);


        v.Add(new Vector(currentRotation.x, currentRotation.y, currentRotation.z));
        desiredRotation = GameManager.CreateVector(v);


        SetDirections(v.x, v.y, v.z);

        while (t < duration && !token.IsCancellationRequested)
        {
            transform.localEulerAngles = Vector3.Lerp(currentRotation, desiredRotation, t / duration);
            t += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }

        if (!token.IsCancellationRequested)
        {
            transform.localEulerAngles = desiredRotation;
            visualizerTypesAllowed.Clear();
        }
    }

    private async Awaitable ScalingUpdate(Vector v, float duration, CancellationToken token)
    {
        float t = 0f;
        Vector3 currentScale = transform.localScale;
        Vector3 desiredScale = GameManager.CreateVector(v);

        if (desiredScale.x == 0)
            desiredScale += Vector3.right * currentScale.x;
        else
            AddVisualizerTypeAllowed(VectorVisualizerType.X);

        if (desiredScale.y == 0)
            desiredScale += Vector3.up * currentScale.y;
        else
            AddVisualizerTypeAllowed(VectorVisualizerType.Y);

        if (desiredScale.z == 0)
            desiredScale += Vector3.forward * currentScale.z;
        else
            AddVisualizerTypeAllowed(VectorVisualizerType.Z);

        SetDirections(v.x, v.y, v.z);

        while (t < duration && !token.IsCancellationRequested)
        {
                transform.localScale = Vector3.Lerp(currentScale, desiredScale, t / duration);
                t += Time.deltaTime;
                await Awaitable.NextFrameAsync();
        }

        if (!token.IsCancellationRequested)
        {
            transform.localScale = desiredScale;
            visualizerTypesAllowed.Clear();
        }
    }

    private async Awaitable ForceUpdate(Vector acceleration, CancellationToken token)
    {
        Vector3 currentPosition = transform.position;
        Vector3 desiredPosition = GameManager.CreateVector(transform, acceleration);

        if (acceleration.x != transform.position.x)
            AddVisualizerTypeAllowed(VectorVisualizerType.X);

        if (acceleration.y != transform.position.y)
            AddVisualizerTypeAllowed(VectorVisualizerType.Y);

        if (acceleration.z != transform.position.z)
            AddVisualizerTypeAllowed(VectorVisualizerType.Z);

        SetDirections(acceleration.x, acceleration.y, acceleration.z);

        while (!token.IsCancellationRequested)
        {
            transform.position += desiredPosition * Time.deltaTime;

            if (transform.position.y > 2000f || transform.position.x > 2000f || transform.position.z > 2000f)
            {
                transform.position = Vector2.zero;
            }

            await Awaitable.NextFrameAsync();
        }


        if (!token.IsCancellationRequested)
            visualizerTypesAllowed.Clear();
    }

    private async Awaitable TranslaionUpdate(Vector v, float duration, CancellationToken token)
    {
        Vector translation = new Vector(transform.position.x, transform.position.y, transform.position.z);
        translation.Add(v);

        Vector3 currentPosition = transform.position;
        Vector3 desiredPosition = GameManager.CreateVector(translation);

        float t = 0f;

        if (translation.x != transform.position.x)
            AddVisualizerTypeAllowed(VectorVisualizerType.X);

        if (translation.y != transform.position.y)
            AddVisualizerTypeAllowed(VectorVisualizerType.Y);

        if (translation.z != transform.position.z)
            AddVisualizerTypeAllowed(VectorVisualizerType.Z);


        SetDirections(v.x, v.y, v.z);

        while (!token.IsCancellationRequested)
        {
            transform.position = Vector3.Lerp(currentPosition, desiredPosition, t / duration);
            t += Time.deltaTime;
            await Awaitable.NextFrameAsync();
        }

        if (!token.IsCancellationRequested)
        {
            transform.position = desiredPosition;
            visualizerTypesAllowed.Clear();
        }
    }


    private void ResetToken()
    {
        if (tokenSource != null)
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
        }

        tokenSource = new CancellationTokenSource();
    }
}