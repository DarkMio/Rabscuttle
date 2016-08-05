using System;
using System.Collections.Generic;
using Rabscuttle.networking.io;

namespace Rabscuttle.handler {
    public abstract class ObservableHandler : IObservable<NetworkMessage> {
        protected List<IObserver<NetworkMessage>> observers;

        public abstract void HandleCommand(NetworkMessage message);
        public abstract void HandleReply(NetworkMessage message);

        public ObservableHandler() {
            observers = new List<IObserver<NetworkMessage>>();
        }

        /**
         * Notifies all obserser with given message.
         */
        protected void NotifyAll(NetworkMessage message) {
            foreach (IObserver<NetworkMessage> observer in observers) {
                observer.OnNext(message);
            }
        }

        public IDisposable Subscribe(IObserver<NetworkMessage> observer) {
            if (!observers.Contains(observer)) {
                observers.Add(observer);
            }
            return new Unsubscriber(observers, observer);
        }

    }

    public sealed class Unsubscriber : IDisposable {
          private readonly List<IObserver<NetworkMessage>>_observers;
          private readonly IObserver<NetworkMessage> _observer;

          public Unsubscriber(List<IObserver<NetworkMessage>> observers, IObserver<NetworkMessage> observer) {
             _observers = observers;
             _observer = observer;
          }

          public void Dispose() {

              if (_observer != null && _observers.Contains(_observer)) {
                  _observers.Remove(_observer);
              }
              GC.SuppressFinalize(this);
          }
    }
}
