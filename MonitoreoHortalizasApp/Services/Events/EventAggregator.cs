namespace MonitoreoHortalizasApp.services.Events;

using System;

public interface IEventAggregator
{
    void Subscribe<TEvent>(Action<TEvent> handler);
    void Unsubscribe<TEvent>(Action<TEvent> handler);
    void Publish<TEvent>(TEvent eventData);
}

public class EventAggregator : IEventAggregator
{
    private readonly Dictionary<Type, List<object>> _subscribers = new();

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        var eventType = typeof(TEvent);
        if (!_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = new List<object>();
        }

        _subscribers[eventType].Add(handler);
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler)
    {
        var eventType = typeof(TEvent);
        if (_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType].Remove(handler);
        }
    }

    public void Publish<TEvent>(TEvent eventData)
    {
        var eventType = typeof(TEvent);
        if (_subscribers.ContainsKey(eventType))
        {
            foreach (var subscriber in _subscribers[eventType])
            {
                ((Action<TEvent>)subscriber).Invoke(eventData);
            }
        }
    }
}
