using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LuceneSearch.Data.Model;

namespace LuceneSearch.Library
{
    public sealed class IndexQueue
    {
        private readonly static IndexQueue indexQueue = new IndexQueue();

        private IndexQueue()
        {

        }

        public static IndexQueue GetInstance()
        {
            return indexQueue;
        }

        private Queue<Data> queue = new Queue<Data>();

        /// <summary>
        /// 将数据添加到队列中
        /// </summary>
        /// <param name="data"></param>
        public void AddQueue(Data data)
        {
            queue.Enqueue(data);
        }

        public void StartThread()
        {
            ThreadStart threadStart = new ThreadStart(UpdateIndex);
            Thread thread = new Thread(threadStart);
            thread.IsBackground = true;//后台线程
            thread.Start();
        }

        public void UpdateIndex()
        {
            while (true)
            {
                if (queue.Count > 0)
                {
                    while (queue.Count > 0)
                    {
                        var data = queue.Dequeue();
                        if (data.OptionType == IndexOptionType.Add)
                        {
                            GoLucene.UpdateLuceneIndex(data.SampleData);
                        }
                        else if (data.OptionType == IndexOptionType.Delete)
                        {
                            GoLucene.ClearLuceneIndexRecord(data.SampleData.Id);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(3000);//队列中没有数据，线程睡眠3秒，防止cpu空转
                }
            }
        }

    }
    public enum IndexOptionType
    {
        Add,
        Delete
    }
    public class Data
    {
        public SampleData SampleData { get; set; }
        public IndexOptionType OptionType { get; set; }


    }
}
