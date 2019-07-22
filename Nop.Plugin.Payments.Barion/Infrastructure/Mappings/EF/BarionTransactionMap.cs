using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;

namespace Nop.Plugin.Payments.Barion.Infrastructure.Mappings.EF
{

    /// <summary>
    /// Represents a BarionTransactionMap mapping configuration
    /// </summary>
    public partial class BarionTransactionMap : NopEntityTypeConfiguration<Domain.BarionTransaction>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Domain.BarionTransaction> builder)
        {
            builder.ToTable(nameof(Domain.BarionTransaction));
            builder.HasKey(rate => rate.Id);
            builder.Property(rate => rate.OrderTotal).HasColumnType("decimal(18, 4)");
        }

        #endregion
    }
}
