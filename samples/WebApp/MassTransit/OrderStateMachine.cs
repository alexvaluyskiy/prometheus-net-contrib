using System;
using Automatonymous;

namespace WebApp.MassTransit
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => SubmitOrder, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderAccepted, x => x.CorrelateById(context => context.Message.OrderId));

            Initially(
                When(SubmitOrder)
                    .TransitionTo(Submitted));

            During(Submitted,
                When(OrderAccepted)
                    .Then(context =>
                    {

                    })
                    .TransitionTo(Accepted)
                    .Finalize());
        }

        public Event<SubmitOrder> SubmitOrder { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }


        public State Submitted { get; private set; }
        public State Accepted { get; private set; }
    }

    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
    }

    public interface SubmitOrder
    {
        Guid OrderId { get; }
    }

    public interface OrderAccepted
    {
        Guid OrderId { get; }
    }
}
