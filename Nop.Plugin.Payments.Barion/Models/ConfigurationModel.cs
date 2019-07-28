using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Barion.Models
{
    public class ConfigurationModel : BaseNopEntityModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.POSKey")]
        public string POSKey { get; set; }
        public bool POSKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.ApiUrl")]
        public string ApiUrl { get; set; }
        public bool ApiUrl_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.BarionPayee")]
        public string BarionPayee { get; set; }
        public bool BarionPayee_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.IsSandbox")]
        public bool IsSandbox { get; set; }
        public bool IsSandbox_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.LogTransaction")]
        public bool LogTransaction { get; set; }
        public bool LogTransaction_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.LogPaymentProcess")]
        public bool LogPaymentProcess { get; set; }
        public bool LogPaymentProcess_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.RedirectUrl")]
        public string RedirectUrl { get; set; }
        public bool RedirectUrl_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.CallbackUrl")]
        public string CallbackUrl { get; set; }
        public bool CallbackUrl_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets an additional fee
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }

        public bool AdditionalFee_OverrideForStore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value.
        /// </summary>
        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }

        public bool AdditionalFeePercentage_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.UseReservationPaymentType")]
        public bool UseReservationPaymentType { get; set; }
        public bool UseReservationPaymentType_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.ReservationPeriod")]
        public int ReservationPeriod { get; set; }
        public bool ReservationPeriod_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Barion.Fields.MarkOrderCompletedAfterPaid")]
        public bool MarkOrderCompletedAfterPaid { get; set; }
        public bool MarkOrderCompletedAfterPaid_OverrideForStore { get; set; }


    }
}
