using AutoMapper;
using net.atos.daf.ct2.account.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace net.atos.daf.ct2.accountservice.Mapping
{
    public class AccountMapperConfig : Profile
    {
        public AccountMapperConfig()
        {
            CreateMap<MenuFeatureDto, MenuFeatureList>();
        }
    }
}
