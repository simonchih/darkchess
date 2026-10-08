using System;
using System.Numerics;
namespace DarkChessUnity
{
    // Exact binary64 fused multiply-add, matching the original arm64 Cython build.
    // Managed implementation: no native library or external runtime dependency.
    public static class SourceFloat
    {
        static void Parts(double value,out BigInteger mantissa,out int exponent)
        {
            long bits=BitConverter.DoubleToInt64Bits(value);
            int raw=(int)((bits>>52)&2047);
            mantissa=new BigInteger(bits&0xfffffffffffffL);
            if(raw!=0)mantissa+=BigInteger.One<<52;
            if(bits<0)mantissa=-mantissa;
            exponent=raw==0?-1074:raw-1075;
        }
        static int Bits(BigInteger value)
        {
            byte[] bytes=value.ToByteArray();int last=bytes.Length-1;
            while(last>0&&bytes[last]==0)last--;
            int bits=last*8;for(int b=bytes[last];b!=0;b>>=1)bits++;return bits;
        }
        public static double Fma(object first,object second,object third)
        {
            double a=P.Number(first),b=P.Number(second),c=P.Number(third);
            if(double.IsNaN(a)||double.IsNaN(b)||double.IsNaN(c)||double.IsInfinity(a)||double.IsInfinity(b)||double.IsInfinity(c))return a*b+c;
            Parts(a,out BigInteger ma,out int ea);Parts(b,out BigInteger mb,out int eb);Parts(c,out BigInteger mc,out int ec);
            int ep=ea+eb,e=Math.Min(ep,ec);
            BigInteger total=((ma*mb)<<(ep-e))+(mc<<(ec-e));
            if(total.IsZero)return 0.0;
            bool negative=total.Sign<0;total=BigInteger.Abs(total);
            int unit=Math.Max(e+Bits(total)-53,-1074),shift=unit-e;
            BigInteger rounded;
            if(shift>0)
            {
                rounded=total>>shift;
                BigInteger remainder=total-(rounded<<shift),half=BigInteger.One<<(shift-1);
                if(remainder>half||(remainder==half&&!rounded.IsEven))rounded++;
            }
            else rounded=total<<(-shift);
            double result=(double)rounded*Math.Pow(2.0,unit);
            return negative?-result:result;
        }
    }
}
