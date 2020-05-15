using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ILT
{
    static class MainConsole
    {
        public static int framerate = 60;
        public static List<Tuple<string, Action>> menuActions = new List<Tuple<string, Action>>();
        public static bool finished = false;

        static void Main(string[] args)
        {
            string choiceString;
            int choice;
            BuildMenu();
            while (!finished)
            {
                PrintMenu();
                choiceString = Console.ReadLine();
                if (int.TryParse(choiceString, out choice) && choice < menuActions.Count)
                {
                    menuActions[choice].Item2();
                }
                else
                {
                    Console.WriteLine("Invalid action.");
                }
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine("Action to do :");
            for (int i = 0; i < menuActions.Count; i++)
            {
                Console.WriteLine(i + ") " + menuActions[i].Item1);
            }
        }

        static void Exit()
        {
            finished = true;
        }

        static void InputLagMeasure()
        {
            Random rand = new Random();
            Thread.Sleep(2000);
            const int count = 500;
            Point p = new Point(1500, 2);
            long t1, t2;
            long average, max, min;
            bool isRed = false;
            max = 0;
            min = 1000;
            average = 0;
            InputGen.Send(InputGen.ScanCodeShort.SPACE);
            InputGen.Release(InputGen.ScanCodeShort.SPACE);
            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(rand.Next(5000, 6000) / 60);
                Stopwatch s = Stopwatch.StartNew();
                t1 = s.ElapsedMilliseconds;
                InputGen.Send(InputGen.ScanCodeShort.SPACE);
                while (IsColorRed(ScreenChecker.GetColorAt(p)) == isRed) ;
                t2 = s.ElapsedMilliseconds;
                long diff = t2 - t1;
                if (diff < min) min = diff;
                if (diff > max) max = diff;
                average += diff;
                Console.WriteLine(diff);
                InputGen.Release(InputGen.ScanCodeShort.SPACE);
            }
            Console.WriteLine("Max:" + max + ",Min:" + min + ",avg:" + average / count);
        }

        static bool IsColorRed(Color c)
        {
            return c.R > c.B;
        }

        static void BuildMenu()
        {
            menuActions.Add(new Tuple<string, Action>("Exit", Exit));
            menuActions.Add(new Tuple<string, Action>("Input lag measure", InputLagMeasure));
        }
    }
}
