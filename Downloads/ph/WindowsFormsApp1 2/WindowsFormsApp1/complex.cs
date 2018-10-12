using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class complex
    {
        public double real;
        public double imag;

        public double Magnitude => Math.Sqrt(real * real + imag * imag);

        public double Phase
        {
            get
            {
                if (real != 0)
                {
                    return Math.Atan(imag / real);
                }
                if (imag > 0)
                {
                    return 90;
                }

                return -90;
            }
        }
        public complex()
        {
            real = new double();
            imag = new double();
        }

        public complex(double real, double imag = 0)
        {
            this.real = real;
            this.imag = imag;
        }

        public static complex from_polar(double r, double theta)
        {
            complex num = new complex(r * Math.Cos(theta),
                r * Math.Sin(theta));
            return num;
        }

        public static complex operator +(complex a, complex b)
        {
            complex num = new complex(a.real + b.real, a.imag + b.imag);
            return num;
        }

        public static complex operator -(complex a, complex b)
        {
            complex num = new complex(a.real - b.real, a.imag - b.imag);
            return num;
        }

        public static complex operator *(complex a, complex b)
        {
            complex num = new complex(a.real * b.real - a.imag * b.imag,
                                      a.real * b.imag + a.imag * b.real);
            return num;
        }

        public override string ToString()
        {
            return real + " + i " + imag;
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
            }

            return X;
        }
    }
}
