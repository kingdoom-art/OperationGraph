using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    class MatrixDecomposition
    {
        //достижимое
        List<int> R = new List<int>();
        //контрдостижимое
        List<int> Q = new List<int>();
        //цикл графа
        List<int> resultPlenty = new List<int>();
        int iter;
        public string name;

        public MatrixDecomposition(int iter)
        {
            this.iter = iter;
            name = "V" + iter;
        }

        /// <summary>
        /// Проверяет наличие вершины в множестве достижимости
        /// </summary>
        /// <param name="tmp">Номер вершины</param>
        public bool R_Contains(int tmp)
        {
            return R.Contains(tmp);
        }

        /// <summary>
        /// Проверяет наличие вершины в множестве контрдостижимости
        /// </summary>
        /// <param name="tmp">Номер вершины</param>
        public bool Q_Contains(int tmp)
        {
            return Q.Contains(tmp);
        }

        /// <summary>
        /// ДОбавление вершины в множество достежимости, если такая вершина есть, не добавится
        /// </summary>
        /// <param name="tmp">Номер добавляемой вершины</param>
        public void Add_R(int tmp)
        {
            //если вершины еще нет, добавляем
            if (!R.Contains(tmp)) R.Add(tmp);
        }

        /// <summary>
        /// Добавление вершины в множество контрдостижимости, если такая вершина есть, не добавится
        /// </summary>
        /// <param name="tmp">Номер добавляемой вершины</param>
        public void Add_Q(int tmp)
        {
            //если вершины еще нет, добавляем
            if (!Q.Contains(tmp)) Q.Add(tmp);
        }

        /// <summary>
        /// Вывод множества достижимости
        /// </summary>
        public void ShowR()
        {
            Console.WriteLine("Достижимое множество Итеррация:" + iter);
            R.Sort();
            R.ForEach(t => Console.Write(t + " "));
            Console.WriteLine();
        }

        /// <summary>
        /// Вывод множества контрдостижимости
        /// </summary>
        public void ShowQ()
        {
            Console.WriteLine("Контрдостижимое множество Итеррация:" + iter);
            Q.Sort();
            Q.ForEach(t => Console.Write(t + " "));
            Console.WriteLine();
        }

        /// <summary>
        /// Вывод пересечения множеств достижимости и контрдостижимости
        /// </summary>
        public void ShowIntersect()
        {
            Console.WriteLine("Пересечени множеств Итеррация:" + iter);
            resultPlenty.Sort();
            resultPlenty.ForEach(t => Console.Write(t + " "));
            Console.WriteLine();
        }

        /// <summary>
        /// Операция пересечения множеств
        /// </summary>
        public void Intersect()
        {
            //Пересечение множеств
            resultPlenty = R.Intersect(Q).ToList();
        }

        /// <summary>
        /// Удаление вершин цикла из исходной последовательности вершин
        /// </summary>
        /// <param name="point">Последовательность вершин</param>
        public List<int> ShrederPoint(List<int> point)
        {
            //разности множеств всех вершин и вершин цикла
            var result = point.Except(resultPlenty).ToList();
            return result;
        }

        /// <summary>
        /// Удаление связей вершин цикла с остальными в графе
        /// </summary>
        /// <param name="mas">Исходный граф</param>
        public List<Edge> ShrederEdge(List<Edge> mas)
        {
            //Идем по всем вершинам, если находим исходящую или входящую, которая была в цикле, пропускаем
            var result = mas.Where(r => !resultPlenty.Contains(r.a) & !resultPlenty.Contains(r.b)).ToList();
            return result;
        }
    }
}
