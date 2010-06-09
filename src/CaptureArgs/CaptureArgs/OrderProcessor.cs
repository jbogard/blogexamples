using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptureArgs
{
	public interface IOrderProcessor
	{
		void Process(OrderBatch batch);
	}

	public interface IShippingReservationService
	{
		void Reserve(int orderId, decimal totalWeight);
	}

	public class OrderBatchProcessor
	{
		private readonly IOrderProcessor _orderProcessor;
		private readonly IShippingReservationService _shippingReservationService;

		public OrderBatchProcessor(IOrderProcessor orderProcessor, 
			IShippingReservationService shippingReservationService)
		{
			_orderProcessor = orderProcessor;
			_shippingReservationService = shippingReservationService;
		}

		public void ProcessBatches(IEnumerable<OrderBatch> orderBatches)
		{
			var expressOrders = orderBatches
				.Where(batch => batch.Type == OrderType.Express);

			var normalOrders = orderBatches
				.Where(batch => batch.Type == OrderType.Normal);

			ProcessOrderBatches(expressOrders);
			ProcessOrderBatches(normalOrders);
		}

		private void ProcessOrderBatches(IEnumerable<OrderBatch> orderBatches)
		{
			foreach (var orderBatch in orderBatches)
			{
				_orderProcessor.Process(orderBatch);
				_shippingReservationService.Reserve(orderBatch.Id, orderBatch.TotalWeight);
			}
		}
	}
}