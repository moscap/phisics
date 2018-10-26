using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Math;

namespace WindowsFormsApp1
{
    class complex
    {
        public Complex num { get; set; }
        public double Magnitude => num.Magnitude;
        public double Phase => num.Phase;
        public complex()
        {
            num = new Complex();
        }

        public complex(double real, double imag = 0)
        {
            num = new Complex(real, imag);
        }

        public complex(Complex num)
        {
            this.num = new Complex(num);
        }

        public static complex from_polar(double r, double theta)
        {
            complex num = new complex(r * Math.Cos(theta),
                r * Math.Sin(theta));
            return num;
        }

        public static complex operator +(complex a, complex b)
        {
            complex num = new complex(a.num + b.num);
            return num;
        }

        public static complex operator -(complex a, complex b)
        {
            complex num = new complex(a.num - b.num);
            return num;
        }

        public static complex operator *(complex a, complex b)
        {
            complex num = new complex(a.num * b.num);
            return num;
        }

        public override string ToString()
        {
            return this.num.ToString();
        }

        public static void FastDFT(Complex[] x, int mode = 1)
        {
            var dir = FourierTransform.Direction.Forward;
            if (mode == -1)
            {
                dir = FourierTransform.Direction.Backward;
            }
            FourierTransform.FFT(x, dir);
        }

        public static complex[] SlowDFT(double[] x)
        {
            int N = x.Length;
            complex[] X = new complex[N];

            for (int k = 0; k < N; k++)
            {
                X[k] = new complex(0);

                for (int n = 0; n < N; n++)
                {
                    complex temp = complex.from_polar(
                        1,
                        -2 * Math.PI * n * k / N
                    );
                    temp *= new complex(x[n]); // how to overload *= ??                    
                    X[k] += temp;
                }
                complex koef = new complex(1.0 / Math.Sqrt(x.Length), 0);
                X[k] *= koef;
            }

            return X;
        }
    }
}
