using UnityEngine;

public struct Vector
{
    public float x { get; private set; }
    public float y { get; private set; }
    public float z { get; private set; }

    public Vector(float x)
    {
        this.x = x;
        this.y = 0f;
        this.z = 0f;
    }
    public Vector(float x, float y)
    {
        this.x = x;
        this.y = y;
        this.z = 0f;
    }
    public Vector(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public void Add(Vector v)
    {
        x += v.x;
        y += v.y;
        z += v.z;
    }
    public void Subtract(Vector v)
    {
        x -= v.x;
        y -= v.y;
        z -= v.z;
    }

    public void Multiply(Vector v) => Multiply(v.GetScalar());
  

    public void Multiply(float scalar)
    {
        x *= scalar;
        y *= scalar;
        z *= scalar;
    }

    public float GetScalar()             => Mathf.Sqrt(Mathf.Pow(this.x, 2) + Mathf.Pow(this.y, 2) + Mathf.Pow(this.z, 2));
    public float GetDotProduct(Vector v) => this.x * v.x + this.y * v.y + this.z * v.z;
    public string GetDotProductFormula(Vector v) => $"({this.x} * {v.x} + {this.y} * {v.y} + {this.z} * {v.z})";

    public float GetAngleBetweenInRadian(Vector v) => Mathf.Acos(this.GetDotProduct(v) / (this.GetScalar() * v.GetScalar()));
    public float GetAngleBetweenInDegree(Vector v) => GetAngleBetweenInRadian(v) * 180 / Mathf.PI;

    public string GetAngleBetweenInDegreeFormula(Vector v) => $"(Acos({this.x} * {v.x} + {this.y} * {v.y} + {this.z} * {v.z} / ({ScalarFormula()} * {v.ScalarFormula()})) * 180 / {System.MathF.Round(Mathf.PI, 2)}";

    public Vector GetVectorProjection(Vector v)
    {
        Vector temp = new Vector(v.x, v.y, v.z);
        temp.Multiply(this.GetDotProduct(v) / Mathf.Pow(v.GetScalar(), 2));
        return temp;
    }

    public Vector GetPlanProjection(Vector v)
    {
        Vector temp = new Vector(this.x, this.y, this.z);
        temp.Subtract(this.GetVectorProjection(v));
        return temp;    
    }

    public string VectorProjectionFormula(Vector v) => $"({v.x} * ({this.GetDotProductFormula(v)} / {v.ScalarFormula()} ^ 2), {v.y} * ({this.GetDotProductFormula(v)} / {v.ScalarFormula()} ^ 2), {v.z} * ({this.GetDotProductFormula(v)} / {v.ScalarFormula()} ^ 2)))";

    public  Vector GetCrossProduct(Vector v) {
        float x, y, z;

        if (this.z * v.x == 0)
            y = -1 * (this.x * v.z);
        else
            y = this.x * v.z - this.z * v.x;

        x = this.y * v.z - this.z * v.y;
        z = this.x * v.y - this.y * v.x;


        return new Vector(x, y, z);
    }

    public string GetCrossProfuctFormula(Vector v) => $"({this.x} * {v.z} - {this.z} * {v.x}, {this.y} * {v.z} - {this.z } * {v.y}, {this.x} * {v.y} - {this.y} * {v.x})";

    public  Vector GetUnitVector()
    {
        float scalar = GetScalar();
        return new Vector(this.x  / scalar, this.y / scalar, this.z / scalar);
    }

    public string UnitVectorFormula()
    {
        float scalar = GetScalar();
        return $"({this.x} / {scalar}, {this.y} / {scalar}, {this.z} / {scalar})";
    }

    public string ScalarFormula()
    {
        return $"√{this.x} ^ 2 + {this.y} ^ 2 + {this.z}^ 2";
    }

    public override string ToString() => $"({this.x}, {this.y}, {this.z})";
}
