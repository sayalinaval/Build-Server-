using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class TestDriver3 : ITest
    {
        private Test31 t1;
        private Test32 t2;
        public  TestDriver3()
        {
            t1 = new Test31();
            t2 = new Test32();
        }
        public bool test()
        {
            if (t1.divideTwoNumbers(12, 6) == 3 && t2.divideTwoNumbers(9, 3) == 27)
                return true;
            return false;
            
        }
        public static void Main(String[] args)
        {
            TestDriver3 t = new TestDriver3();
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
