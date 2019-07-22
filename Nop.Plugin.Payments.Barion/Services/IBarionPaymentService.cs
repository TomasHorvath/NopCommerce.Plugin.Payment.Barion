using System;
using System.Collections.Generic;
using System.Text;
using BarionClientLibrary;
using BarionClientLibrary.Operations.FinishReservation;
using BarionClientLibrary.Operations.PaymentState;
using BarionClientLibrary.Operations.Refund;
using BarionClientLibrary.Operations.StartPayment;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Barion.Domain;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Barion.Services
{
    public interface IBarionPaymentService
    {
        BarionClientLibrary.BarionSettings GetBarionClientSettings(BarionSettings settings);
        StartPaymentOperationResult InitPayment(BarionClientLibrary.BarionSettings transactionSettings, BarionSettings storeSetting, Order order);
        RefundOperationResult RefundPayment(BarionClientLibrary.BarionSettings transactionSettings, Domain.BarionTransaction transaction, RefundPaymentRequest refundPaymentRequest);
        GetPaymentStateOperationResult GetPaymentState(BarionClientLibrary.BarionSettings transactionSettings, BarionTransaction transaction);
        FinishReservationOperationResult CapturePayment(BarionClientLibrary.BarionSettings transactionSettings, BarionTransaction transaction, CapturePaymentRequest capturePaymentRequest);
        FinishReservationOperationResult VoidPayment(BarionClientLibrary.BarionSettings transactionSettings, BarionTransaction transaction, VoidPaymentRequest voidPaymentRequest);
    }
}
