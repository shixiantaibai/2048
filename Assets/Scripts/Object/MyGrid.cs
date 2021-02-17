using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{
    public Number n;

    public bool isHaveNum() {
        return n!=null;
    }
    public Number GetNumber() {
        return n;
    }
    public void SetNumber(Number number) {
        this.n = number;
    }
}
