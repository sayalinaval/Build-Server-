using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class TestDriver2 : ITest
    {
        private Test11 t1;
        private Test22 t2;
        private Test34 t3;
        public  TestDriver2()
        {
            t1 = new Test11();
            t2 = new Test22();
            t3 = new Test33();
        }
        public bool test()
        {
            if (t1.addTwoNumbers(3, 6) == 9 && t2.divideTwoNumbers(9, 3) == 27 && t3.addTwoNumbers(3, 9) == 12)
                return true;
            return false;
            
        }
        public static void Main(String[] args)
        {
            TestDriver2 t = new TestDriver2();
            if(t.test())
            {
                Console.WriteLine("True retured");
            }
            else
            {
                Console.WriteLine("False return");
            }
        }
    }

    public interface ITest
    {
        bool test();
    }
}
