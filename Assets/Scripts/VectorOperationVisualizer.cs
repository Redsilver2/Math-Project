using Game.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VectorOperationVisualizer : MonoBehaviour
{
    [SerializeField] private float rotationSpeed  = 5f;
    [SerializeField] private float movementSpeed = 5f;

    [Space]
    [SerializeField] private Transform parent;
    [SerializeField] private TextMeshProUGUI informationDisplayer;

    [Space]
    [SerializeField] private Button addVectorButton;
    [SerializeField] private Button subtractVectorButton;
    [SerializeField] private Button scalarMultiplicationButton;


    [Space]
    [SerializeField] private Button crossProduct;
    [SerializeField] private Button dotProduct;
    [SerializeField] private Button scalarButton;
    [SerializeField] private Button unitVector;

    [Space]
    [SerializeField] private Button angleBetweenButton;
    [SerializeField] private Button vectorialProjectionButton;

    [Space]
    [SerializeField] private TMP_InputField xComponentVector01;
    [SerializeField] private TMP_InputField yComponentVector01;
    [SerializeField] private TMP_InputField zComponentVector01;


    [Space]
    [SerializeField] private TMP_InputField xComponentVector02;
    [SerializeField] private TMP_InputField yComponentVector02;
    [SerializeField] private TMP_InputField zComponentVector02;

    [Space]
    [SerializeField] private TMP_InputField scalarInputField;

    private SingleVectorVisualizer     vector01, vector02, vector03;
    private MultipleVectorVisualizer[] vectorVisualizers;
    private Camera mainCamera;

    private void Start()
    {
        vectorVisualizers  = MultipleVectorVisualizer.SetAndGetVisualizers(transform, 10f);
        transform.position = Vector3.zero;
        parent.transform.position = Vector3.zero;

        mainCamera = Camera.main;


        CreateRenderer(ref vector01);
        CreateRenderer(ref vector02);
        CreateRenderer(ref vector03);

        vector01.SetRendererMaterial(Resources.Load<Material>("Vectors/1"));
        vector02.SetRendererMaterial(Resources.Load<Material>("Vectors/2"));
        vector03.SetRendererMaterial(Resources.Load<Material>("Vectors/3"));

        vector01.SetUIText(parent);
        vector02.SetUIText(parent);
        vector03.SetUIText(parent);

        GameManager.SetButton(addVectorButton, () =>
        {
            Vector v01 = GetVector(xComponentVector01, yComponentVector01, zComponentVector01);
            Vector v02 = GetVector(xComponentVector02, yComponentVector02, zComponentVector02);
           
            Vector result = new Vector();
            result.Add(v01);
            result.Add(v02);
            
            EnableVisualizer(true, true, true);

            vector01.UpdateRenderer(Vector3.zero, Vector3.right * v01.x + Vector3.up * v01.y + Vector3.forward * v01.z);
            vector02.UpdateRenderer(Vector3.zero, Vector3.right * v02.x + Vector3.up * v02.y + Vector3.forward * v02.z);
            vector03.UpdateRenderer(Vector3.right * v01.x + Vector3.up * v01.y + Vector3.forward * v01.z, Vector3.right * result.x + Vector3.up * result.y + Vector3.forward * result.z);

            if (informationDisplayer != null)
                informationDisplayer.text = $"{v01.ToString()} + {v02.ToString()} = {result.ToString()}";

        }, "Add");

        GameManager.SetButton(subtractVectorButton, () =>
        {
            Vector v01 = GetVector(xComponentVector01, yComponentVector01, zComponentVector01);
            Vector v02 = GetVector(xComponentVector02, yComponentVector02, zComponentVector02);

            Vector result = new Vector();
            result.Add(v01);
            result.Subtract(v02);
            EnableVisualizer(true, true, true);

            vector01.UpdateRenderer(Vector3.zero, Vector3.right * v01.x + Vector3.up * v01.y + Vector3.forward * v01.z);
            vector02.UpdateRenderer(Vector3.zero, Vector3.right * v02.x + Vector3.up * v02.y + Vector3.forward * v02.z);
            vector03.UpdateRenderer(Vector3.right * v01.x + Vector3.up * v01.y + Vector3.forward * v01.z, Vector3.right * result.x + Vector3.up * result.y + Vector3.forward * result.z);

            if (informationDisplayer != null)
                informationDisplayer.text = $"{v01.ToString()} - {v02.ToString()} = {result.ToString()}";
        }, "Substract");

        GameManager.SetButton(scalarMultiplicationButton, () =>
        {
            Vector v01 = GetVector(xComponentVector01, yComponentVector01, zComponentVector01);    
            Vector result = new Vector();
            
            int scalar = int.Parse(scalarInputField.text);
            EnableVisualizer(true, false, false);

            result.Add(v01);
            result.Multiply(scalar);
            vector01.UpdateRenderer(Vector3.zero, new Vector3(result.x, result.y, result.z));

            if (informationDisplayer != null)
                informationDisplayer.text = $"{v01.ToString()} * {scalar} = {result.ToString()}";

        }, "Scalar Multiplication");

        GameManager.SetButton(crossProduct, () =>
        {
            Vector v01 = GetVector(xComponentVector01, yComponentVector01, zComponentVector01);
            Vector v02 = GetVector(xComponentVector02, yComponentVector02, zComponentVector02);
            Vector crossProduct = v01.GetCrossProduct(v02);
           
            EnableVisualizer(true, true, true);

            vector01.UpdateRenderer(Vector3.zero, Vector3.right * v01.x + Vector3.up * v01.y + Vector3.forward * v01.z);
            vector02.UpdateRenderer(Vector3.zero, Vector3.right * v02.x + Vector3.up * v02.y + Vector3.forward * v02.z);
            vector03.UpdateRenderer(Vector3.zero, Vector3.right * crossProduct.x + Vector3.up * crossProduct.y + Vector3.forward * crossProduct.z);

            if (informationDisplayer != null)
                informationDisplayer.text = $"{v01.GetCrossProfuctFormula(v02)} = {crossProduct}";

        }, "Get Cross Product");


        GameManager.SetButton(dotProduct, () =>
        {
            Vector v01 = GetVector(xComponentVector01, yComponentVector01, zComponentVector01);
            Vector v02 = GetVector(xComponentVector02, yComponentVector02, zComponentVector02);
           
            Vector result = new Vector();
            result.Add(v01);
           
            EnableVisualizer(true, true, false);

            vector01.UpdateRenderer(Vector3.zero, Vector3.right * v01.x + Vector3.up * v01.y + Vector3.forward * v01.z);
            vector02.UpdateRenderer(Vector3.zero, Vector3.right * v02.x + Vector3.up * v02.y + Vector3.forward * v02.z);

            if (informationDisplayer != null)
                informationDisplayer.text = $"{result.GetDotProductFormula(v02)} = {result.GetDotProduct(v02).ToString()}";

        }, "Get Dot Product");


        GameManager.SetButton(scalarButton, () =>
        {
            Vector v01 = GetVector(xComponentVector01, yComponentVector01, zComponentVector01);
            EnableVisualizer(true, false, false);

            if (informationDisplayer != null)
                informationDisplayer.text = $"{v01.ScalarFormula()} = {v01.GetScalar()}";

        }, "Get Scalar");

        GameManager.SetButton(unitVector, () =>
        {
            Vector v01 = GetVector(xComponentVector01, yComponentVector01, zComponentVector01);
            EnableVisualizer(true, false, false);

            Vector unitVector = v01.GetUnitVector();
            vector01.UpdateRenderer(Vector3.zero, new Vector3(unitVector.x, unitVector.y, unitVector.z));


            if (informationDisplayer != null)
                informationDisplayer.text = $"{v01.UnitVectorFormula()} = {v01.GetUnitVector()}";

        }, "Get Unit Vector");

        GameManager.SetButton(angleBetweenButton, () =>
        {
            Vector v01 = GetVector(xComponentVector01, yComponentVector01, zComponentVector01);
            Vector v02 = GetVector(xComponentVector02, yComponentVector02, zComponentVector02);
            EnableVisualizer(true, true, false);

            vector01.UpdateRenderer(Vector3.zero, Vector3.right * v01.x + Vector3.up * v01.y + Vector3.forward * v01.z);
            vector02.UpdateRenderer(Vector3.zero, Vector3.right * v02.x + Vector3.up * v02.y + Vector3.forward * v02.z);


            if (informationDisplayer != null)
                informationDisplayer.text = $"{v01.GetAngleBetweenInDegreeFormula(v02)} = {v01.GetAngleBetweenInDegree(v02)} Degree";

        }, "Get Angle Between");

        GameManager.SetButton(vectorialProjectionButton, () =>
        {
            Vector v01    = GetVector(xComponentVector01, yComponentVector01, zComponentVector01);
            Vector v02    = GetVector(xComponentVector02, yComponentVector02, zComponentVector02);
            Vector result;

            if (v01.z != 0)
                v01 = new Vector(v01.x, v01.y);

            if (v02.z != 0)
                v02 = new Vector(v02.x, v02.y);

            result = v01.GetVectorProjection(v02);
            EnableVisualizer(true, true, true);

            vector01.UpdateRenderer(Vector3.zero, Vector3.right * v01.x + Vector3.up * v01.y + Vector3.forward * v01.z);
            vector02.UpdateRenderer(Vector3.zero, Vector3.right * v02.x + Vector3.up * v02.y + Vector3.forward * v02.z);
            vector03.UpdateRenderer(Vector3.zero, Vector3.right * result.x + Vector3.up * result.y + Vector3.forward * result.z);

            if (informationDisplayer != null)
                informationDisplayer.text = $"{v01.VectorProjectionFormula(v02)} = {result.ToString()}";

        }, "Get Vectorial Projection");


        EnableVisualizer(false, false, false);
    }

    private void Update()
    {
        Transform transform = mainCamera.transform;

        if (Input.GetKey(KeyCode.Q))
            transform.position += Vector3.up * movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
            transform.position -= Vector3.up * movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * movementSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.T))
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.R))
            transform.Rotate(-Vector3.up * rotationSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.F))
            transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.G))
            transform.Rotate(-Vector3.right * rotationSpeed * Time.deltaTime);


        foreach (var vectorizer in vectorVisualizers)
            vectorizer.UpdateVisuals(1);

        vector01.UpdateRenderer();
        vector02.UpdateRenderer();
        vector03.UpdateRenderer();  
    }

    private Vector GetVector(TMP_InputField x, TMP_InputField y, TMP_InputField z)
    {
        float _x, _y, _z;

        if (x != null)
            _x = float.Parse(x.text);
        else
            _x = 0;

        if(y != null)
            _y = float.Parse(y.text);
        else
            _y = 0;

        if(z != null)
            _z = float.Parse(z.text);
        else
            _z = 0;

        return new Vector(_x, _y, _z);  
    }
    private void CreateRenderer(ref SingleVectorVisualizer visualizer)
    {
        var result = new GameObject();
        visualizer = result.AddComponent<SingleVectorVisualizer>();
        result.transform.position = Vector3.zero;     
    }

    private void EnableVisualizer(bool enable01, bool enable02, bool enable03)
    {
        vector01.enabled = enable01;
        vector02.enabled = enable02;
        vector03.enabled = enable03;
    }
}
