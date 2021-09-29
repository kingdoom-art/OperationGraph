using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Resources;

namespace Graph
{
    class Program
    {
        struct Edge
        {
            public int a, b;
            public int num;

            public Edge(string a, string b, string num)
            {
                this.a = Convert.ToInt32(a);
                this.b = Convert.ToInt32(b);
                this.num = Convert.ToInt32(num);
            }
        }

        static List<Edge> GetGraph()
        {
            List<Edge> mas = new List<Edge>();
            try
            {                
                using (StreamReader str = new StreamReader("Graph.txt"))
                {
                    string tmp;
                    while((tmp = str.ReadLine()) != null)
                    {
                        var tmpMas = tmp.Split(' ');
                        mas.Add(new Edge(tmpMas[0], tmpMas[1],tmpMas[2]));
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return mas;
        }

        static void ShowMatrix(List<Edge> mas, List<int> point)
        {
            Console.Write("   ");
            point.ForEach(x => Console.Write(String.Format("{0,3}",x)));
            Console.WriteLine();


            foreach (var i in point)
            {
                Console.Write(String.Format("{0,3}",i));
                int[] result = new int[point.Count()];
                mas.Where(r => r.a == i).ToList().ForEach(t => result[t.b-1] = 1);
                result.ToList().ForEach(m => Console.Write(String.Format("{0,3}",m)));
                Console.WriteLine();
            }

        }

        static void ShowIncidents(List<Edge> mas, List<int> point)
        {
            var reb = (from i in mas orderby i.num select i.num).ToList();
            Console.Write("   ");
            reb.ForEach(t => Console.Write(String.Format("{0,3}",t)));
            Console.WriteLine();

            foreach (var i in point)
            {
                Console.Write(String.Format("{0,3}",i));
                int[] result = new int[reb.Count()];
                mas.Where(t => t.a == i).ToList().ForEach(r => result[r.num - 1] = 1);
                mas.Where(t => t.b == i).ToList().ForEach(r => result[r.num - 1] = -1);
                result.ToList().ForEach(m => Console.Write(String.Format("{0,3}", m)));
                Console.WriteLine();

            }
        }

        static void ShowPlenty(List<Edge> mas)
        {
            var tmp = from i in mas group i by i.a;

            foreach(var i in tmp)
            {
                Console.Write("G("+i.Key+")=(");
                string res = "";
                i.ToList().ForEach(t => res += "," + t.b);
                Console.WriteLine(res.Remove(0,1) + "); ");
            }

            var tmp2 = from i in mas group i by i.b;

            foreach (var i in tmp2)
            {
                Console.Write("G-1(" + i.Key + ")=(");
                string res = "";
                i.ToList().ForEach(t => res += "," + t.a);
                Console.WriteLine(res.Remove(0, 1) + "); ");
            }
        }

        static void Decomposition(List<Edge> mas, List<int> point)
        {
            Stack<int> tmpStack = new Stack<int>();
            List<int> R = new List<int>();
            List<int> Q = new List<int>();

            //находим достижимое
            tmpStack.Push(point[0]);

            while (tmpStack.Count() > 0)
            {
                int tmp = tmpStack.Pop();
                if (!R.Contains(tmp)) R.Add(tmp);

                var plenty = mas.Where(r => r.a == tmp && !R.Contains(r.b)).Select(m => m.b).ToList();
                plenty.ForEach(t => tmpStack.Push(t));
            }

            R.ForEach(t => Console.Write(t + " "));
            Console.WriteLine();

            //контрдостижимое
            tmpStack.Push(point[0]);
            while(tmpStack.Count() > 0)
            {
                int tmp = tmpStack.Pop();
                if (!Q.Contains(tmp)) Q.Add(tmp);

                var plenty = mas.Where(r => r.b == tmp && !Q.Contains(r.a)).Select(m => m.a).ToList();
                plenty.ForEach(t => tmpStack.Push(t));
            }
            Q.ForEach(t => Console.Write(t + " "));
            Console.WriteLine();

            //Пересечение множеств
            var resultPlenty = R.Intersect(Q).ToList();

            resultPlenty.ForEach(t => Console.Write(t + " "));
        }

        static void Main(string[] args)
        {
            List<Edge> mas = GetGraph();
            foreach(var i in mas)
            {
                Console.WriteLine(i.a+" "+i.b);
            }

            var dist = mas.Select(m => m.a).Union(mas.Select(n => n.b)).ToList();
            dist.Sort();

            //ShowMatrix(mas, dist);
            //ShowIncidents(mas, dist);
            //ShowPlenty(mas);

            Decomposition(mas, dist);
            Console.ReadKey();
        }
    }
}
