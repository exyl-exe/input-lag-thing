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
        public static int count = 200;
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
            Console.WriteLine("######################");
            Console.WriteLine("Measures done per command : " + count);
            Console.WriteLine("Actions:");
            for (int i = 0; i < menuActions.Count; i++)
            {
                Console.WriteLine(i + ") " + menuActions[i].Item1);
            }
        }

        static void Exit()
        {
            finished = true;
        }

        static void SetCount()
        {
            Console.WriteLine("New value ?");
            string countString = Console.ReadLine();
            int countBackup = count;
            if (int.TryParse(countString, out count) && count>0)
            {
                Console.WriteLine("The delay will be measured " + count + " times per command.");
            } else
            {
                Console.WriteLine("Invalid value");
                count = countBackup;
            }
        }

        static void InputLagMeasure()
        {
            Random rand = new Random();
            Thread.Sleep(2000);
            Point p = new Point(1500, 2);
            long t1, t2, t3, t4;
            long diff, diffR;
            //Values measured after a press
            long average, max, min;
            //Values measured after a release
            long averageR, maxR, minR;

            min = minR = 9999999999;//dirty but idc
            max = maxR = 0;
            average = averageR = 0;

            //Unpause the game
            InputGen.Send(InputGen.ScanCodeShort.SPACE);
            InputGen.Release(InputGen.ScanCodeShort.SPACE);

            Stopwatch s = Stopwatch.StartNew();

            for (int i = 0; i < count; i++)
            {
                //Sleeps a random time between 0 and 1 frame, to randomize if the input happens at the beginning or the end of a frame
                Thread.Sleep(rand.Next(0,1000) / framerate);

                //Press space, then release it
                t1 = s.ElapsedMilliseconds;
                InputGen.Send(InputGen.ScanCodeShort.SPACE);
                //Wait for screen to go red
                while (!IsColorRed(ScreenChecker.GetColorAt(p))) ;
                t2 = s.ElapsedMilliseconds;

                Thread.Sleep(rand.Next(0,1000) / framerate);
                t3 = s.ElapsedMilliseconds;
                InputGen.Release(InputGen.ScanCodeShort.SPACE);
                //Wait for screen to go blue
                while (IsColorRed(ScreenChecker.GetColorAt(p))) ;
                t4 = s.ElapsedMilliseconds;

                //milliseconds needed for screen color to change after inputing
                diff = t2 - t1;
                diffR = t4 - t3;

                //Compute average, min, max
                min = diff < min ? diff : min;
                max = diff > max ? diff : max;
                average += diff;

                minR = diffR < minR ? diffR : minR;
                maxR = diffR > maxR ? diffR : maxR;
                averageR += diffR;

                Console.WriteLine(diff +"->"+diffR);
                
            }

            Console.WriteLine("-- Press --");
            Console.WriteLine("Max : " + max + "ms");
            Console.WriteLine("Min : " + min + "ms");
            Console.WriteLine("Average : " + average/count + "ms");
            Console.WriteLine("");
            Console.WriteLine("-- Release --");
            Console.WriteLine("Max : " + maxR + "ms");
            Console.WriteLine("Min : " + minR + "ms");
            Console.WriteLine("Average : " + averageR/count + "ms");
        }

        static bool IsColorRed(Color c)
        {
            return c.R > c.B;
        }

        static void BuildMenu()
        {
            menuActions.Add(new Tuple<string, Action>("Exit", Exit));
            menuActions.Add(new Tuple<string, Action>("Input lag measure", InputLagMeasure));
            menuActions.Add(new Tuple<string, Action>("Set the amount of measures per command", SetCount));
        }
    }
}
