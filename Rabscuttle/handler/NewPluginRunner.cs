using System;

namespace Rabscuttle.core.handler {
    public class NewPluginRunner : MarshalByRefObject {
        public void DoSomething(string name) {
            Console.WriteLine("Hello {0}", name);
        }
    }
}
