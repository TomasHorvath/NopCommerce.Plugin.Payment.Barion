using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Nop.Core.Infrastructure.Mapper;

namespace Nop.Plugin.Payments.Barion.Infrastructure.Mappings.Automapper
{
    public class BarionMapperConfiguration : Profile, IOrderedMapperProfile
    {

        public BarionMapperConfiguration()
        {
            CreatAdminModelMaps();
        }

        /// <summary>
        ///
        /// </summary>
        protected virtual void CreatAdminModelMaps()
        {
            CreateMap<BarionSettings, Models.ConfigurationModel>()
                .ForMember(e => e.ActiveStoreScopeConfiguration, option => option.Ignore())
                .ForMember(e=>e.ApiUrl_OverrideForStore, options => options.Ignore())
                .ForMember(e=>e.BarionPayee_OverrideForStore, option=> option.Ignore())
                .ForMember(e => e.CallbackUrl_OverrideForStore, option => option.Ignore())
                .ForMember(e => e.CustomProperties, option => option.Ignore())
                .ForMember(e => e.Id, option => option.Ignore())
                .ForMember(e => e.IsSandbox_OverrideForStore, option => option.Ignore())
                .ForMember(e => e.LogPaymentProcess_OverrideForStore, option => option.Ignore())
                .ForMember(e => e.LogTransaction_OverrideForStore, option => option.Ignore())
                .ForMember(e => e.POSKey_OverrideForStore, option => option.Ignore())
                .ForMember(e => e.RedirectUrl_OverrideForStore, option => option.Ignore())
                .ReverseMap();


            CreateMap<Domain.BarionTransaction, Models.BarionTransactionModel>()
               .ReverseMap();
        }


        public int Order => 10;
    }
}
