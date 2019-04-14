using System.Collections.Generic;

namespace NodeOptimization
{
    public static class Faults
    {
        public static Dictionary<string, int> States = new Dictionary<string, int>();
        public static bool LinkState(string node1, string node2)
        {
            //Dictionary<string, int> vals = new Dictionary<string, int>();
            /*
            vals.Add("0A", 1);
            vals.Add("AE", 1);
            vals.Add("AC", 1);
            vals.Add("AB", 1);
            vals.Add("BD", 0);
            vals.Add("CD", 1);
            vals.Add("CE", 1);
            */
            /*
            vals.Add("BG", 1);
            vals.Add("CB", 1);
            vals.Add("AG", 1);
            vals.Add("AC", 1);
            vals.Add("AF", 1);
            vals.Add("GH", 1);
            vals.Add("HF", 1);
            vals.Add("HJ", 1);
            vals.Add("JZ", 0); //switch
            vals.Add("HZ", 1);
            vals.Add("FZ", 0);
            vals.Add("CD", 1);
            vals.Add("DF", 1);
            vals.Add("DE", 1);
            vals.Add("EF", 0);
            vals.Add("EZ", 1);
            vals.Add("0A", 1);
            */
            //return vals[node1 + node2] == 1 ? true : false;
            return States[node1 + node2] == 1 ? true : false;
        }
        public static Dictionary<string, int> Distances = new Dictionary<string, int>();
        public static int LinkDistance(string node1, string node2)
        {
            //Dictionary<string, int> vals = new Dictionary<string, int>();
            /*
            vals.Add("0A", 1);
            vals.Add("AE", 15);
            vals.Add("AC", 4);
            vals.Add("AB", 7);
            vals.Add("BD", 2);
            vals.Add("CD", 11);
            vals.Add("CE", 3);
            */

            /*
            vals.Add("BG", 9);
            vals.Add("CB", 6);
            vals.Add("AG", 47);
            vals.Add("AC", 9);
            vals.Add("AF", 5);
            vals.Add("GH", 9);
            vals.Add("HF", 16);
            vals.Add("HJ", 1);
            vals.Add("JZ", 1);
            vals.Add("HZ", 7);
            vals.Add("FZ", 10);
            vals.Add("CD", 5);
            vals.Add("DF", 20);
            vals.Add("DE", 30);
            vals.Add("EF", 19);
            vals.Add("EZ", 3);
            vals.Add("0A", 1);
            //return vals[node1 + node2];
            */
            return Distances[node1 + node2];
        }
    }
}
