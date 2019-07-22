using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace Nop.Plugin.Payments.Barion.Infrastructure.Mappings.EF
{

    /// <summary>
    /// Represents a AllowedIPAddressMap mapping configuration
    /// </summary>
    public partial class AllowedIPAddressMap : NopEntityTypeConfiguration<Domain.AllowedIPAddress>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Domain.AllowedIPAddress> builder)
        {
            builder.ToTable(nameof(Domain.AllowedIPAddress));
            builder.HasKey(rate => rate.Id);
        }

        #endregion
    }
}
