using System;
using UniRx;

namespace CurrentGame.GameFlow
{
    public class EventManager
    {
        public static string modelUpdated = "ModelUpdated";
        public static string buttonClicked = "ButtonClicked";
        
        
        private Subject<string> eventBus = new Subject<string>();
        
        public IObservable<string> EventBus => eventBus;
        
        public void TriggerEvent(string eventName)
        {
            eventBus.OnNext(eventName);
        }
    }
}