using System.Threading.Tasks;
using TextClustering.Domain.Entities;

namespace TextClustering.Application.Algorithm
{
    public interface IAlgorithmService
    {
        Task<DatasetClustering> GetClusteringResult(Dataset dataset);
    }
}