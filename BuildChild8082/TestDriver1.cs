using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class TestDriver1 : ITest
    {
        private Test1 t1;
        private Test2 t2;
        public  TestDriver1()
        {
            t1 = new Test1();
            t2 = new Test2();
        }
        public bool test()
        {
            if (t1.addTwoNumbers(3, 6) == 9 && t2.divideTwoNumbers(9, 3) == 27)
                return true;
            return false;
            
        }
        public static void Main(String[] args)
        {
            TestDriver1 t = new TestDriver1();
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
