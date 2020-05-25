using AutoMapper;
using TextClustering.Application.Dto;
using TextClustering.Domain.Entities;

namespace TextClustering.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<User, Models.User>();

            // DatasetClustering
            CreateMap<DatasetClustering, Models.DatasetClustering>();
            CreateMap<DatasetClusteringDto, Models.DatasetClustering>()
                .AfterMap((src, dest, context) => context.Mapper.Map(src.DatasetClustering, dest));

            // Cluster
            CreateMap<Cluster, Models.Cluster>()
                .ForMember(dest => dest.Texts, opt => opt.MapFrom(src => src.ClusterDatasetTexts));
            CreateMap<ClusterDto, Models.Cluster>()
                .AfterMap((src, dest, context) => context.Mapper.Map(src.Cluster, dest));

            CreateMap<ClusterDatasetText, string>()
                .ConstructUsing(src => src.DatasetText.Text);

            CreateMap<Topic, Models.Topic>()
                .ForMember(dest => dest.Tokens, opt => opt.MapFrom(src => src.Tokens));

            CreateMap<TopicToken, string>()
                .ConstructUsing(src => src.Token);
        }
    }
}