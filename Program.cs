using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Resources;

namespace Graph
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

    class Program
    {
        

        //получение графа формата (Первая_вершина Вторя_вершина Номер_пути)
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

        //Вывод матицы смежности
        static void ShowMatrix(List<Edge> mas, List<int> point)
        {
            Console.Write("   ");
            point.ForEach(x => Console.Write(String.Format("{0,3}",x)));
            Console.WriteLine();

            //идем по отсортированным, зарание вершинам
            foreach (var i in point)
            {
                Console.Write(String.Format("{0,3}",i));
                int[] result = new int[point.Count()];
                //находим вершину. в которую есть выход, заполняем 1
                mas.Where(r => r.a == i).ToList().ForEach(t => result[t.b-1] = 1);
                //выводим получившийся массив
                result.ToList().ForEach(m => Console.Write(String.Format("{0,3}",m)));
                Console.WriteLine();
            }

        }

        //Матрица инцеденций
        static void ShowIncidents(List<Edge> mas, List<int> point)
        {
            //сортируем по номерам пути
            var reb = (from i in mas orderby i.num select i.num).ToList();
            Console.Write("   ");
            reb.ForEach(t => Console.Write(String.Format("{0,3}",t)));
            Console.WriteLine();

            //идем по отсортированным путям, чтобы не создавать временных массивов
            foreach (var i in point)
            {
                Console.Write(String.Format("{0,3}",i));
                int[] result = new int[reb.Count()];
                //заполняем выходной массив по достижимости вершин путем
                mas.Where(t => t.a == i).ToList().ForEach(r => result[r.num - 1] = 1);
                mas.Where(t => t.b == i).ToList().ForEach(r => result[r.num - 1] = -1);
                //выводим получившийся массив
                result.ToList().ForEach(m => Console.Write(String.Format("{0,3}", m)));
                Console.WriteLine();

            }
        }

        //множественное задание структуры
        static void ShowPlenty(List<Edge> mas)
        {
            //немного sql групперуем первой вершине, навыходе получим страшную структуру данных
            var tmp = from i in mas group i by i.a;

            foreach(var i in tmp)
            {
                //ключ это вершина, из которой будем извлекать достижимые вершины
                Console.Write("G("+i.Key+")=(");
                string res = "";
                //извлекаем группу и записываем сразу в строку
                i.ToList().ForEach(t => res += "," + t.b);
                //ну и удаляем первый символ, ибо он запятая
                Console.WriteLine(res.Remove(0,1) + "); ");
            }

            //теперь групперуем по второй вершине
            var tmp2 = from i in mas group i by i.b;

            foreach (var i in tmp2)
            {
                Console.Write("G-1(" + i.Key + ")=(");
                string res = "";
                i.ToList().ForEach(t => res += "," + t.a);
                Console.WriteLine(res.Remove(0, 1) + "); ");
            }
        }

        //Декомпозиция
        static void Decomposition(List<Edge> mas, List<int> point)
        {
            int iter = 1;
            List<MatrixDecomposition> finalGraph = new List<MatrixDecomposition>();
            while(point.Count > 0)
            {
                MatrixDecomposition tmpGraph = new MatrixDecomposition(iter);
                Stack<int> tmpStack = new Stack<int>();

                //запихиваем вершину в стэк
                tmpStack.Push(point[0]);

                while (tmpStack.Count() > 0)
                {
                    //достаем верхнюю вершину стэка, в стэке она удаляется
                    int tmp = tmpStack.Pop();
                    tmpGraph.Add_R(tmp);

                    /*
                     * идем по графу, ищем нашу вершину на первом местие, при этом вершина на втором месте
                     * не должна содержаться в финальном множестве
                     * достаем вторю вершину, по скольку в нее будет идти путь из первой
                    */
                    var plenty = mas.Where(r => r.a == tmp && !tmpGraph.R_Contains(r.b)).Select(m => m.b).ToList();
                    //запишем весь набор получившихся вершин в стэк
                    plenty.ForEach(t => tmpStack.Push(t));
                }

                //контрдостижимое
                tmpStack.Push(point[0]);
                while (tmpStack.Count() > 0)
                {
                    int tmp = tmpStack.Pop();
                    tmpGraph.Add_Q(tmp);

                    //Аналогично достижимому, только проверяется на первая вершина, а вторая
                    var plenty = mas.Where(r => r.b == tmp && !tmpGraph.Q_Contains(r.a)).Select(m => m.a).ToList();
                    plenty.ForEach(t => tmpStack.Push(t));
                }
                //Делаем песечение множеств
                tmpGraph.Intersect();
                //удаляем все отработанное из графа
                point = tmpGraph.ShrederPoint(point);
                mas = tmpGraph.ShrederEdge(mas);

                //собираем новый граф
                finalGraph.Add(tmpGraph);
                iter++;
            }

            foreach (var i in finalGraph)
            {
                Console.WriteLine(i.name);
                i.ShowR();
                i.ShowQ();
                i.ShowIntersect();
            }
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
