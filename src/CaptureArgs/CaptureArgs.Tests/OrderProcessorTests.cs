using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using Should;

namespace CaptureArgs.Tests
{
	[TestFixture]
	public class OrderProcessorTests
	{
		[Test]
		public void Should_prefer_express_orders_to_process()
		{
			var batch1 = new OrderBatch
			{
				Type = OrderType.Normal
			};	
			var batch2 = new OrderBatch
			{
				Type = OrderType.Express
			};		
			var batch3 = new OrderBatch
			{
				Type = OrderType.Normal
			};

			var processor = Stub<IOrderProcessor>();
			var shipper = Stub<IShippingReservationService>();

			var batchProcessor = new OrderBatchProcessor(processor, shipper);

			IList<OrderBatch> args = processor
				.Capture()
				.Args<OrderBatch>((p, batch) => p.Process(batch));

			batchProcessor.ProcessBatches(new[]
			{
				batch1, batch2, batch3
			});

			args.Count().ShouldEqual(3);
			args.First().ShouldEqual(batch2);
		}

		[Test]
		public void Should_reserve_shipping_for_orders()
		{
			var batch1 = new OrderBatch
			{
				Id = 5,
				Type = OrderType.Normal,
				TotalWeight = 10m
			};
			var batch2 = new OrderBatch
			{
				Id = 6,
				Type = OrderType.Normal,
				TotalWeight = 15m
			};

			var processor = Stub<IOrderProcessor>();
			var shipper = Stub<IShippingReservationService>();

			var args = shipper
				.Capture()
				.Args<int, decimal>((s, orderId, weight) => s.Reserve(orderId, weight));

			var batchProcessor = new OrderBatchProcessor(processor, shipper);

			batchProcessor.ProcessBatches(new[]
			{
				batch1, batch2
			});

			args.Count.ShouldEqual(2);
			args[0].Item1.ShouldEqual(batch1.Id);
			args[0].Item2.ShouldEqual(batch1.TotalWeight);
			args[1].Item1.ShouldEqual(batch2.Id);
			args[1].Item2.ShouldEqual(batch2.TotalWeight);
		}


		private static T Stub<T>() where T : class
		{
			return MockRepository.GenerateStub<T>();
		}
	}
}