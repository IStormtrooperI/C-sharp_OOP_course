using System;
using System.Collections.Generic;
using System.Text;

namespace Delegates.Observers
{
    public class StackOperationsLogger
    {
        private readonly Observer observers = new Observer();
        private Observer Observers 
        { 
            get
            {
                return observers;
            }
        }

        public void SubscribeOn<T>(ObservableStack<T> stack)
        {
            stack.Add(Observers);
        }

        public string GetLog()
        {
            return Observers.Log.ToString();
        }
    }

    public interface IObserver
    {
        void EventHandle(object sender, object eventData);
    }

    public class Observer : IObserver
    {
        public StringBuilder Log = new StringBuilder();

        public void EventHandle(object sender, object eventData)
        {
            Log.Append(eventData);
        }
    }

    public class ObservableStack<T>
    {
        public event EventHandler<object> Observers;

        public void Add(IObserver observer)
        {
            Observers += observer.EventHandle;
        }

        public void Notify(object eventData)
        {
            Observers?.Invoke(this, eventData);
            //foreach (var observer in Observers)
            //    observer.HandleEvent(eventData);
        }

        public void Remove(IObserver observer)
        {
            Observers -= observer.EventHandle;
        }

        readonly List<T> dataList = new List<T>();

        public void Push(T obj)
        {
            dataList.Add(obj);
            Notify(new StackEventData<T> { IsPushed = true, Value = obj });
        }

        public T Pop()
        {
            if (dataList.Count == 0)
                throw new InvalidOperationException();
            var result = dataList[dataList.Count - 1];
            Notify(new StackEventData<T> { IsPushed = false, Value = result });
            return result;
        }
    }
}