namespace Manager.Extensions
{
    public static class RandHelper
    {
        public static string RndomStr(this int codeLength)
        {
            //组成字符串的字符集合  0-9数字、大小写字母
            string chars = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,P,P,Q,R,S,T,U,V,W,X,Y,Z";

            string[] charArray = chars.Split(new Char[] { ',' });
            string code = "";
            int temp = -1;//记录上次随机数值，尽量避避免生产几个一样的随机数
            Random rand = new();
            //采用一个简单的算法以保证生成随机数的不同
            for (int i = 1; i < codeLength + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));//初始化随机类
                }
                int t = rand.Next(61);
                temp = t;//把本次产生的随机数记录起来
                code += charArray[t];//随机数的位数加一
            }
            return code;
        }

        public static string RndomNum(this int codeLength)
        {
            //组成字符串的字符集合  0-9数字、大小写字母
            string chars = "0,1,2,3,4,5,6,7,8,9";

            string[] charArray = chars.Split(new Char[] { ',' });
            string code = "";
            int temp = -1;//记录上次随机数值，尽量避避免生产几个一样的随机数
            Random rand = new();
            //采用一个简单的算法以保证生成随机数的不同
            for (int i = 1; i < codeLength + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));//初始化随机类
                }
                int t = rand.Next(10);
                temp = t;//把本次产生的随机数记录起来
                code += charArray[t];//随机数的位数加一
            }
            return code;
        }

        //获取随机数 英文+数字
        public static string GetRandomCode(this int CodeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,i,J,K,M,N,P,Q,R,S,T,U,W,X,Y,Z";
            string[] allCharArray = allChar.Split(',');
            string RandomCode = "";
            int temp = -1;

            Random rand = new();
            for (int i = 0; i < CodeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(temp * i * ((int)DateTime.Now.Ticks));
                }

                int t = rand.Next(33);

                while (temp == t)
                {
                    t = rand.Next(33);
                }

                temp = t;
                RandomCode += allCharArray[t];
            }

            return RandomCode;
        }
    }
}