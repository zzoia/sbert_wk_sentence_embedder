const HOST = process.env.REACT_APP_BACKEND_URL;
const DATASET_CLUSTERINGS = "dataset-clusterings";

const routes = {
    LOGIN: HOST + "token",
    DATASET_CLUSTERINGS: HOST + DATASET_CLUSTERINGS,
    DATASET_CLUSTERING: datasetClusteringId => `${HOST + DATASET_CLUSTERINGS}/${datasetClusteringId}`,
    DATASET_CLUSTERS: datasetClusteringId => `${HOST + DATASET_CLUSTERINGS}/${datasetClusteringId}/clusters`
};

export default routes;