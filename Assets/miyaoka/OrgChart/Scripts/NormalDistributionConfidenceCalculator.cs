using UnityEngine;
using System.Collections;
using System;

public static class NormalDistributionConfidenceCalculator
{
    /// <summary>
    /// 
    /// </summary>
    public static double InverseNormalDistribution(double probability, double min, double max)
    {
        double x = 0;
        double a = 0;
        double b = 1;

        double precision = Math.Pow(10, -3);

        while ((b - a) > precision)
        {
            x = (a + b) / 2;
            if (NormInv(x) > probability)
            {
                b = x;
            }
            else
            {
                a = x;
            }
        }

        if ((max > 0) && (min > 0))
        {
            x = x * (max - min) + min;
        }
        return x;
    }

    /// <summary>
    /// Returns the cumulative density function evaluated at A given value.
    /// </summary>
    /// <param name="x">A position on the x-axis.</param>
    /// <param name="mean"></param>
    /// <param name="sigma"></param>
    /// <returns>The cumulative density function evaluated at <C>x</C>.</returns>
    /// <remarks>The value of the cumulative density function at A point <C>x</C> is
    /// probability that the value of A random variable having this normal density is
    /// less than or equal to <C>x</C>.
    /// </remarks>
    public static double NormalDistribution(double x, double mean, double sigma)
    {
        // This algorithm is ported from dcdflib:
        // Cody, W.D. (1993). "ALGORITHM 715: SPECFUN - A Portabel FORTRAN
        // Package of Special Function Routines and Test Drivers"
        // acm Transactions on Mathematical Software. 19, 22-32.
        int i;
        double del, xden, xnum, xsq;
        double result, ccum;
        double arg = (x - mean) / sigma;
        const double sixten = 1.60e0;
        const double sqrpi = 3.9894228040143267794e-1;
        const double thrsh = 0.66291e0;
        const double root32 = 5.656854248e0;
        const double zero = 0.0e0;
        const double min = Double.Epsilon;
        double z = arg;
        double y = Math.Abs(z);
        const double half = 0.5e0;
        const double one = 1.0e0;

        double[] a =
        {
            2.2352520354606839287e00, 1.6102823106855587881e02, 1.0676894854603709582e03,
            1.8154981253343561249e04, 6.5682337918207449113e-2
        };

        double[] b =
        {
            4.7202581904688241870e01, 9.7609855173777669322e02, 1.0260932208618978205e04,
            4.5507789335026729956e04
        };

        double[] c =
        {
            3.9894151208813466764e-1, 8.8831497943883759412e00, 9.3506656132177855979e01,
            5.9727027639480026226e02, 2.4945375852903726711e03, 6.8481904505362823326e03,
            1.1602651437647350124e04, 9.8427148383839780218e03, 1.0765576773720192317e-8
        };

        double[] d =
        {
            2.2266688044328115691e01, 2.3538790178262499861e02, 1.5193775994075548050e03,
            6.4855582982667607550e03, 1.8615571640885098091e04, 3.4900952721145977266e04,
            3.8912003286093271411e04, 1.9685429676859990727e04
        };
        double[] p =
        {
            2.1589853405795699e-1, 1.274011611602473639e-1, 2.2235277870649807e-2,
            1.421619193227893466e-3, 2.9112874951168792e-5, 2.307344176494017303e-2
        };


        double[] q =
        {
            1.28426009614491121e00, 4.68238212480865118e-1, 6.59881378689285515e-2,
            3.78239633202758244e-3, 7.29751555083966205e-5
        };
        if (y <= thrsh)
        {
            //
            // Evaluate  anorm  for  |X| <= 0.66291
            //
            xsq = zero;
            if (y > double.Epsilon) xsq = z * z;
            xnum = a[4] * xsq;
            xden = xsq;
            for (i = 0; i < 3; i++)
            {
                xnum = (xnum + a[i]) * xsq;
                xden = (xden + b[i]) * xsq;
            }
            result = z * (xnum + a[3]) / (xden + b[3]);
            double temp = result;
            result = half + temp;
        }

        //
        // Evaluate  anorm  for 0.66291 <= |X| <= sqrt(32)
        //
        else if (y <= root32)
        {
            xnum = c[8] * y;
            xden = y;
            for (i = 0; i < 7; i++)
            {
                xnum = (xnum + c[i]) * y;
                xden = (xden + d[i]) * y;
            }
            result = (xnum + c[7]) / (xden + d[7]);
            xsq = Math.Floor(y * sixten) / sixten;
            del = (y - xsq) * (y + xsq);
            result = Math.Exp(-(xsq * xsq * half)) * Math.Exp(-(del * half)) * result;
            ccum = one - result;
            if (z > zero)
            {
                result = ccum;
            }
        }

        //
        // Evaluate  anorm  for |X| > sqrt(32)
        //
        else
        {
            xsq = one / (z * z);
            xnum = p[5] * xsq;
            xden = xsq;
            for (i = 0; i < 4; i++)
            {
                xnum = (xnum + p[i]) * xsq;
                xden = (xden + q[i]) * xsq;
            }
            result = xsq * (xnum + p[4]) / (xden + q[4]);
            result = (sqrpi - result) / y;
            xsq = Math.Floor(z * sixten) / sixten;
            del = (z - xsq) * (z + xsq);
            result = Math.Exp(-(xsq * xsq * half)) * Math.Exp(-(del * half)) * result;
            ccum = one - result;
            if (z > zero)
            {
                result = ccum;
            }
        }

        if (result < min)
            result = 0.0e0;
        return result;
    }

    /// <summary>
    /// Given a probability, a mean, and a standard deviation, an x value can be calculated.
    /// </summary>
    /// <returns></returns>
    public static double NormInv(double probability)
    {
        const double a1 = -39.6968302866538;
        const double a2 = 220.946098424521;
        const double a3 = -275.928510446969;
        const double a4 = 138.357751867269;
        const double a5 = -30.6647980661472;
        const double a6 = 2.50662827745924;

        const double b1 = -54.4760987982241;
        const double b2 = 161.585836858041;
        const double b3 = -155.698979859887;
        const double b4 = 66.8013118877197;
        const double b5 = -13.2806815528857;

        const double c1 = -7.78489400243029E-03;
        const double c2 = -0.322396458041136;
        const double c3 = -2.40075827716184;
        const double c4 = -2.54973253934373;
        const double c5 = 4.37466414146497;
        const double c6 = 2.93816398269878;

        const double d1 = 7.78469570904146E-03;
        const double d2 = 0.32246712907004;
        const double d3 = 2.445134137143;
        const double d4 = 3.75440866190742;

        //Define break-points
        // using Epsilon is wrong; see link above for reference to 0.02425 value
        //const double pLow = double.Epsilon;
        const double pLow = 0.02425;

        const double pHigh = 1 - pLow;

        //Define work variables
        double q;
        double result = 0;

        // if argument out of bounds.
        // set it to a value within desired precision.
        if (probability <= 0) 
            probability = pLow;

        if (probability >= 1)
            probability = pHigh;

        if (probability < pLow)
        {
            //Rational approximation for lower region
            q = Math.Sqrt(-2 * Math.Log(probability));
            result = (((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / ((((d1 * q + d2) * q + d3) * q + d4) * q + 1);
        }
        else if (probability <= pHigh)
        {
            //Rational approximation for lower region
            q = probability - 0.5;
            double r = q * q;
            result = (((((a1 * r + a2) * r + a3) * r + a4) * r + a5) * r + a6) * q /
                (((((b1 * r + b2) * r + b3) * r + b4) * r + b5) * r + 1);
        }
        else if (probability < 1)
        {
            //Rational approximation for upper region
            q = Math.Sqrt(-2 * Math.Log(1 - probability));
            result = -(((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / ((((d1 * q + d2) * q + d3) * q + d4) * q + 1);
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="probability"></param>
    /// <param name="mean"></param>
    /// <param name="sigma"></param>
    /// <returns></returns>
    public static double NormInv(double probability, double mean, double sigma)
    {
        double x = NormInv(probability);
        return sigma * x + mean;
    }
}