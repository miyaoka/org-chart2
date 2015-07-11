using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Util {


  public static Color HSVToRGB(float H, float S, float V)
  {
    if (S == 0f) {
      return new Color (V, V, V);
    }
    else if (V == 0f){
      return Color.black;
    }
    else
    {
      Color col = Color.black;
      float Hval = H * 6f;
      int sel = Mathf.FloorToInt(Hval);
      float mod = Hval - sel;
      float v1 = V * (1f - S);
      float v2 = V * (1f - S * mod);
      float v3 = V * (1f - S * (1f - mod));
      switch (sel + 1)
      {
      case 0:
        col.r = V;
        col.g = v1;
        col.b = v2;
        break;
      case 1:
        col.r = V;
        col.g = v3;
        col.b = v1;
        break;
      case 2:
        col.r = v2;
        col.g = V;
        col.b = v1;
        break;
      case 3:
        col.r = v1;
        col.g = V;
        col.b = v3;
        break;
      case 4:
        col.r = v1;
        col.g = v2;
        col.b = V;
        break;
      case 5:
        col.r = v3;
        col.g = v1;
        col.b = V;
        break;
      case 6:
        col.r = V;
        col.g = v1;
        col.b = v2;
        break;
      case 7:
        col.r = V;
        col.g = v3;
        col.b = v1;
        break;
      }
      col.r = Mathf.Clamp(col.r, 0f, 1f);
      col.g = Mathf.Clamp(col.g, 0f, 1f);
      col.b = Mathf.Clamp(col.b, 0f, 1f);
      return col;
    }
  }
  public static string AddOrdinal(int num)
  {
    if( num <= 0 ) return num.ToString();

    switch(num % 100)
    {
    case 11:
    case 12:
    case 13:
      return num + "th";
    }

    switch(num % 10)
    {
    case 1:
      return num + "st";
    case 2:
      return num + "nd";
    case 3:
      return num + "rd";
    default:
      return num + "th";
    }

  }
  public static ArrayList shuffleArrayList(ArrayList inputList0)
  {
    var inputList = inputList0.Clone () as ArrayList;
    var randomList = new ArrayList();

    var r = new System.Random();
    int randomIndex = 0;
    while (inputList.Count > 0)
    {
      randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
      randomList.Add(inputList[randomIndex]); //add it to the new, random list
      inputList.RemoveAt(randomIndex); //remove to avoid duplicates
    }

    return randomList; //return the new random list
  }
  public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
  {
    var rnd = new System.Random();
    return source.OrderBy<T, int>((item) => rnd.Next());
  }
  /*
  public static void Shuffle<T>(this IList<T> list)  
  {  
    Random rng = new Random();  
    int n = list.Count;  
    while (n > 1) {  
      n--;  
      int k = rng.Next(n + 1);  
      T value = list[k];  
      list[k] = list[n];  
      list[n] = value;  
    }  
  }
  */
}


public static class UDFs
{
  private const int MAXIT = 100;
  private const double EPS = 0.0000003;
  private const double FPMIN = 1.0E-30;

  public static double BetaInv(double p, double alpha, double beta, double A, double B)
  {    
    return InverseBeta(p, alpha, beta, A, B);
  }

  private static double InverseBeta(double p, double alpha, double beta, double A, double B)
  {
    double x = 0;
    double a = 0;
    double b = 1;
    double precision = System.Math.Pow(10, -6); // converge until there is 6 decimal places precision

    while ((b - a) > precision)
    {
      x = (a + b) / 2;

      if (IncompleteBetaFunction(x, alpha, beta) > p)
      {
        b = x;
      }
      else
      {
        a = x;
      }
    }

    if ((B > 0) && (A > 0))
    {
      x = x * (B - A) + A;
    }

    return x;
  }

  private static double IncompleteBetaFunction(double x, double a, double b)
  {
    double bt = 0;

    if (x <= 0.0)
    {
      return 0;
    }

    if (x >= 1)
    {
      return 1;
    }
      
    bt = System.Math.Exp(Gammln(a + b) - Gammln(a) - Gammln(b) + a * System.Math.Log(x) + b * System.Math.Log(1.0 - x));

    if (x < ((a + 1.0) / (a + b + 2.0)))
    {
      // Use continued fraction directly.
      return (bt * betacf(a, b, x) / a);
    }
    else
    {
      // Use continued fraction after making the symmetry transformation.
      return (1.0 - bt * betacf(b, a, 1.0 - x) / b);
    }
  }

  private static double betacf(double a, double b, double x)
  {
    int m, m2;
    double aa, c, d, del, h, qab, qam, qap;

    qab = a + b; // These q’s will be used in factors that occur in the coe.cients (6.4.6).
    qap = a + 1.0;
    qam = a - 1.0;

    c = 1.0; // First step of Lentz’s method.

    d = 1.0 - qab * x / qap;

    if (System.Math.Abs(d) < FPMIN)
    {
      d = FPMIN;
    }

    d = 1.0 / d;
    h = d;

    for (m = 1; m <= MAXIT; ++m)
    {
      m2 = 2 * m;
      aa = m * (b - m) * x / ((qam + m2) * (a + m2));
      d = 1.0 + aa * d; //One step (the even one) of the recurrence.

      if (System.Math.Abs(d) < FPMIN)
      {
        d = FPMIN;
      }

      c = 1.0 + aa / c;

      if (System.Math.Abs(c) < FPMIN)
      {
        c = FPMIN;
      }

      d = 1.0 / d;
      h *= d * c;

      aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
      d = 1.0 + aa * d; // Next step of the recurrence (the odd one).

      if (System.Math.Abs(d) < FPMIN)
      {
        d = FPMIN;
      }

      c = 1.0 + aa / c;

      if (System.Math.Abs(c) < FPMIN)
      {
        c = FPMIN;
      }

      d = 1.0 / d;
      del = d * c;
      h *= del;

      if (System.Math.Abs(del - 1.0) < EPS)
      {
        // Are we done?
        break;
      }
    }

    if (m > MAXIT)
    {
      return 0;
    }
    else
    {
      return h;
    }
  }

  public static double Gammln(double xx)
  {
    double x, y, tmp, ser;

    double[] cof = new double[] { 76.180091729471457, -86.505320329416776, 24.014098240830911, -1.231739572450155, 0.001208650973866179, -0.000005395239384953 };

    y = xx;
    x = xx;
    tmp = x + 5.5;
    tmp -= (x + 0.5) * System.Math.Log(tmp);

    ser = 1.0000000001900149;

    for (int j = 0; j <= 5; ++j)
    {
      y += 1;
      ser += cof[j] / y;
    }

    return -tmp + System.Math.Log(2.5066282746310007 * ser / x);
  }
}

