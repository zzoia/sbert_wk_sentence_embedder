using RestEase;
using System.Threading.Tasks;

namespace TextClustering.Application.Algorithm.Http
{
    public interface IClusteringApi
    {
        [Post("cluster")]
        Task<ClusterResponse> Cluster([Body] ClusterRequest request);
    }
}
