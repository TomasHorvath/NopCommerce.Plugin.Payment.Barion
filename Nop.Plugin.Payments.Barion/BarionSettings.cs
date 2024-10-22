﻿using System;
using Nop.Core;
using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Barion
{
    public class BarionSettings : BaseEntity, ISettings 
    {
        /// <summary>
        /// The secret API key of the shop, generated by Barion. This lets the shop to authenticate through the Barion API, but does not provide access to the account owning the shop itself.
        /// </summary>
        public string POSKey { get; set; }
        /// <summary>
        /// API gateway url
        /// </summary>
        public string ApiUrl { get; set; }
        /// <summary>
        /// Barion Payee
        /// </summary>
        public string BarionPayee { get; set; }
        /// <summary>
        /// Use test enviroment
        /// </summary>
        public bool IsSandbox { get; set; }
        /// <summary>
        /// Log response data from Barion
        /// </summary>
        public bool LogTransaction { get; set; }
        /// <summary>
        /// Log all payment process (mainly for debug purpose)
        /// </summary>
        public bool LogPaymentProcess { get; set; }
        /// <summary>
        /// The URL where the payer should be redirected after the payment is completed or cancelled. The payment identifier is added to the query string part of this URL in the paymentId parameter. If not provided, the system will use the redirect URL assigned to the shop that started the payment.
        /// </summary>
        public string RedirectUrl { get; set; }
        /// <summary>
        /// The URL where the Barion system sends a request whenever there is a change in the state of the payment. The payment identifier is added to the query string part of this URL in the paymentId parameter.
        /// </summary>
        public string CallbackUrl { get; set; }
        /// <summary>
        /// Show Payment info during checkout
        /// </summary>
        public bool SkipPaymentInfo { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        public bool AdditionalFeePercentage { get; set; }

        /// <summary>
        /// Use payment with reservation
        /// </summary>
        public bool UseReservationPaymentType { get; set; }

        /// <summary>
        /// Period of time when amount is reserved (days)
        /// </summary>
        public int ReservationPeriod { get; set; }


        /// <summary>
        /// Mark order as completed when payment status changed to paid
        /// </summary>
        public bool MarkOrderCompletedAfterPaid { get; set; }
             
            

    }
}
