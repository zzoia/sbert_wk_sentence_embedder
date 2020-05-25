using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TextClustering.Application;
using TextClustering.Application.Dto;
using TextClustering.Web.Helpers;
using TextClustering.Web.Models;

namespace TextClustering.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DatasetClusteringsController : ControllerBase
    {
        private readonly IDatasetClusteringService _datasetClusteringService;

        private readonly IMapper _mapper;

        private readonly IAuthenticationService _authenticationService;

        public DatasetClusteringsController(
            IDatasetClusteringService datasetClusteringService,
            IMapper mapper,
            IAuthenticationService authenticationService)
        {
            _datasetClusteringService = datasetClusteringService;
            _mapper = mapper;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            User currentUser = _authenticationService.GetCurrentUser();

            IReadOnlyCollection<DatasetClusteringDto> datasetClusterings =
                await _datasetClusteringService.GetAll(currentUser.Id, null);

            IReadOnlyCollection<DatasetClustering> datasets =
                _mapper.Map<IReadOnlyCollection<DatasetClusteringDto>,
                    IReadOnlyCollection<DatasetClustering>>(
                    datasetClusterings);

            return Ok(datasets);
        }

        [HttpPost]
        public async Task<IActionResult> Cluster([FromBody] DatasetClusterRequest request)
        {
            User currentUser = _authenticationService.GetCurrentUser();

            var clustering = await _datasetClusteringService.CreateAndClusterDataset(
                request.DatasetName, 
                request.DatasetDescription, 
                request.Texts,
                currentUser.Id);

            var model = _mapper.Map<DatasetClustering>(clustering);

            return Ok(model);
        }

        [HttpGet("{datasetClusteringId}")]
        public async Task<IActionResult> GetById(int datasetClusteringId)
        {
            User currentUser = _authenticationService.GetCurrentUser();

            IReadOnlyCollection<DatasetClusteringDto> datasetClusterings =
                await _datasetClusteringService.GetAll(currentUser.Id, datasetClusteringId);

            var clustering = datasetClusterings.Single();

            var models = _mapper.Map<DatasetClusteringDto, DatasetClustering>(clustering);

            return Ok(models);
        }

        [HttpGet("{datasetClusteringId}/clusters")]
        public async Task<IActionResult> GetClusters(int datasetClusteringId)
        {
            User currentUser = _authenticationService.GetCurrentUser();

            var clusters = await _datasetClusteringService.GetClusters(datasetClusteringId, currentUser.Id);
            var models = _mapper.Map<IReadOnlyCollection<ClusterDto>, IReadOnlyCollection<Cluster>>(clusters);

            return Ok(models);
        }
    }
}