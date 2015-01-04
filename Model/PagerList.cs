using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 用于返回DataGrid控件数据源的泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagerList<T> : IEnumerable<T>
    {
        #region IEnumerable
        private T foot;
        /// <summary>
        /// 页脚的值
        /// </summary>
        public T Foot
        {
            get { return foot; }
            set { foot = value; }
        }

        private T[] list = new T[4];
        public int Count;
        public PagerList() { }
        public PagerList(int i)
        {
            list = new T[i];
            this.Count = i;
        }
        public T this[int i]
        {
            get
            {
                return list[i];
            }
            set
            {
                list[i] = value;
            }
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>)new PagerListEnumerator<T>(this);

        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (IEnumerator<T>)new PagerListEnumerator<T>(this);

        }

        public void Add(T t)
        {
            //数组元素满后重新申请空间
            if (Count == list.Length)
            {
                T[] tmp = list;
                list = new T[Count + 2];
                tmp.CopyTo(list, 0);
            }
            list[Count++] = t;
        }
        public void Remove(int index)
        {
            if (index >= Count || index < 0)
            {
                return;
            }
            for (int i = index; i < Count - 1; i++)
            {
                list[i] = list[i + 1];
            }
        }

        /// <summary>
        /// 重写tostring函数，以便返回指定格式的json字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";

            string json = Newtonsoft.Json.JavaScriptConvert.SerializeObject(this, timeConverter);
            string foot = Foot == null ? "" : Newtonsoft.Json.JavaScriptConvert.SerializeObject(Foot, timeConverter);
            json = "{\"total\":" + this.Total + ",\"rows\":" + json + ",\"footer\":[" + foot + "]}";
            return json;
        }

        private int _total = 0;
        /// <summary>
        /// 数据源总记录数
        /// </summary>
        public int Total
        {
            get { return _total; }
            set { _total = value; }
        }
        private int _pageCount = 0;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get { return _pageCount; }
            set { _pageCount = value; }
        }
        #endregion


    }
    /// <summary>
    /// IEnumerator<int>的实现
    /// </summary>
    public class PagerListEnumerator<T> : IEnumerator<T>
    {
        public PagerList<T> items;
        public int Length;
        public int index = -1;
        public PagerListEnumerator(PagerList<T> item)
        {
            items = item;
            Length = item.Count;
        }
        public T Current
        {
            get
            {
                return items[index];
            }
        }

        public bool MoveNext()
        {
            return (++index) >= Length ? false : true;
        }
        public void Reset()
        {
            index = -1;
        }
        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return items[index]; }
        }
    }
}
