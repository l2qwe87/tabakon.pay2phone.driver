using PayToPhone.Driver.App.Contracts.Integrator;
using PayToPhone.Driver.App.Contracts.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayToPhone.Driver.App.AppServices.Integrator {
    internal class MessageQueue : IMessageQueue {

        private static readonly Queue<IMessage> _queue = new();

        public void Enqueue(IMessage messege) {
            lock (_queue) { 
                _queue.Enqueue(messege);
            }
        }

        public IMessage Dequeue() {
            lock (_queue) {
                if (_queue.Count == 0) {
                    return null;
                }
                
                return _queue.Dequeue();
            }
        }
    }
}
