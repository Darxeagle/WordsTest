using System;
using UniRx;

namespace CurrentGame.GameFlow
{
    public class EventManager
    {
        private Subject<EventId> eventBus = new Subject<EventId>();
        
        public IObservable<EventId> EventBus => eventBus;
        
        public void TriggerEvent(EventId eventId)
        {
            eventBus.OnNext(eventId);
        }
    }
}