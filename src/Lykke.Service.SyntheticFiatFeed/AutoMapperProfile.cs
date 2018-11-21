using AutoMapper;
using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Service.SyntheticFiatFeed.Client.Models.ExchangeCommission;
using Lykke.Service.SyntheticFiatFeed.Client.Models.Settings;
using Lykke.Service.SyntheticFiatFeed.Client.Models.TickPrice;
using Lykke.Service.SyntheticFiatFeed.Domain;
using Lykke.Service.SyntheticFiatFeed.RabbitMq;

namespace Lykke.Service.SyntheticFiatFeed
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ExchangeCommissionSetting, ExchangeCommissionSettingModel>(MemberList.Source);
            CreateMap<ExchangeCommissionSettingModel, ExchangeCommissionSetting>(MemberList.Destination);

            CreateMap<TickPriceSubscriber.TickPriceExt, TickPriceModel>(MemberList.Source)
                .ForMember(o => o.AssetPair, opt => opt.MapFrom(o => o.Asset))
                .ForSourceMember(o => o.BestAsk, opt => opt.DoNotValidate())
                .ForSourceMember(o => o.BestBid, opt => opt.DoNotValidate());

            CreateMap<TickPrice, TickPriceModel>(MemberList.Source)
                .ForMember(o => o.AssetPair, opt => opt.MapFrom(o => o.Asset));
            CreateMap<TickPriceModel, TickPrice>(MemberList.Destination)
                .ForMember(o => o.Asset, opt => opt.MapFrom(o => o.AssetPair));

            CreateMap<ISimBaseInstrumentSetting, SimBaseInstrumentSettingModel>(MemberList.Source);
            CreateMap<SimBaseInstrumentSetting, SimBaseInstrumentSettingModel>(MemberList.Source);
            CreateMap<SimBaseInstrumentSettingModel, SimBaseInstrumentSetting>(MemberList.Destination);

            CreateMap<ILinkedInstrumentSettings, LinkedInstrumentSettingsModel>(MemberList.Source);
            CreateMap<LinkedInstrumentSettingsModel, LinkedInstrumentSettings>(MemberList.Destination);
            CreateMap<LinkedInstrumentSettingsModel, ILinkedInstrumentSettings>(MemberList.Destination)
                .ConstructUsing(o =>  Mapper.Map<LinkedInstrumentSettingsModel, LinkedInstrumentSettings>(o));
        }
    }
}
