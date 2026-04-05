using System;

namespace Phase3_Intermediate_CSharp
{
    internal class DelegatesEventsExamples
    {
        private static void Main()
        {
            // 1) Simple delegate usage
            //    - `Logger` is a delegate type (type-safe function pointer).
            //    - Assigning `ConsoleLogger` makes `logger` refer to that method.
            //    - Invoking `logger(...)` calls the referenced method.
            Logger logger = ConsoleLogger;
            logger("Logging via delegate");

            // 2) Multicast delegate
            //    - Delegates can hold multiple targets. When invoked, they call
            //      each target in order. This is useful for broadcast-style
            //      notifications such as logging pipelines or middleware hooks.
            Logger multi = ConsoleLogger;
            multi += AnotherLogger; // add a second handler
            multi("Message for multiple handlers");

            // 3) Events (publisher/subscriber pattern)
            //    - Events are a language-level pattern built on delegates that
            //      restrict direct invocation so only the declaring type can
            //      raise them. Consumers can only subscribe/unsubscribe.
            var publisher = new Publisher();
            var subscriber = new Subscriber(publisher);

            // Raise an event; subscriber receives it.
            publisher.RaiseEvent("Hello subscribers!");

            // Demonstrate detaching: after unsubscribe the handler no longer runs.
            subscriber.Unsubscribe();
            publisher.RaiseEvent("No one is listening.");

            // Interview tip: explain difference between delegate and event:
            // - Delegate: variable that holds method references and can be invoked by anyone with access.
            // - Event: exposes += and -= to subscribers but only the owner can invoke.
        }

        // Delegate definition: a delegate is a type that represents a method
        // signature. Here `Logger` represents methods that accept a string and
        // return void.
        private delegate void Logger(string message);

        // Example target methods for the delegate.
        private static void ConsoleLogger(string message)
        {
            Console.WriteLine($"[Console] {message}");
        }

        private static void AnotherLogger(string message)
        {
            Console.WriteLine($"[Another] {message}");
        }

        private class Publisher
        {
            // Event declaration: exposes subscription but prevents external
            // callers from invoking the event directly. Only the Publisher
            // class can raise the event (via RaiseEvent).
            public event EventHandler<MessageEventArgs> MessageSent;

            // Method that raises the event. The null-conditional operator
            // ensures safe invocation when there are no subscribers.
            public void RaiseEvent(string message)
            {
                MessageSent?.Invoke(this, new MessageEventArgs(message));
            }
        }

        private class Subscriber
        {
            private readonly Publisher _publisher;

            public Subscriber(Publisher publisher)
            {
                _publisher = publisher;
                // Subscribe to the publisher's event. The subscriber provides a
                // handler that matches the EventHandler<MessageEventArgs>
                // signature (object sender, MessageEventArgs e).
                _publisher.MessageSent += HandleMessage;
            }

            public void Unsubscribe()
            {
                // Detach handler when no longer interested. Important to avoid
                // memory leaks in long-lived publishers.
                _publisher.MessageSent -= HandleMessage;
            }

            private void HandleMessage(object? sender, MessageEventArgs e)
            {
                Console.WriteLine($"Subscriber received: {e.Message}");
            }
        }

        private class MessageEventArgs : EventArgs
        {
            public string Message { get; }
            public MessageEventArgs(string message) => Message = message;
        }
    }
}
