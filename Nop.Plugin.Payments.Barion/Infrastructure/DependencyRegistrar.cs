using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Services.Tax;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Payments.Barion.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<Services.BarionPaymentService>().As<Services.IBarionPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<Services.TransactionService>().As<Services.ITransactionService>().InstancePerLifetimeScope();
            builder.RegisterType<Factories.BarionModelFactory>().As<Factories.IBarionModelFactory>().InstancePerLifetimeScope();
            builder.RegisterType<Services.AllowedIpService>().As<Services.IAllowedIpService>().InstancePerLifetimeScope();

            //data context
            builder.RegisterPluginDataContext<Data.BarionPaymentsContext>("nop_object_context_barion_payment");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<Domain.BarionTransaction>>().As<IRepository<Domain.BarionTransaction>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_barion_payment"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Domain.AllowedIPAddress>>().As<IRepository<Domain.AllowedIPAddress>>()
              .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_barion_payment"))
              .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order => 1;
    }
}